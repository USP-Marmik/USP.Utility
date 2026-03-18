using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
namespace USP.Utility
{
    /// <summary>
    /// Script by Vishal Lakhani 
    /// Email : usp.vishal@gmail.com, vlxmenblack@gmail.com
    /// Description: this script is created for bezier curve using line renderer, use line renderer as reference and the script which has been attached to it will follow on mouse down events
    /// might have bugs, and overlapping issue still there might and shall be fixed in future versions.
    /// </summary>
    public class DragAlongLineRenderer2D : MonoBehaviour
    {
        [Header("References")]
        public LineRenderer line;
        public Camera cam;

        [Header("Plane / 2D settings")]
        public float zPlane = 0f;              // The Z where your 2D world lives (usually 0) 
        public bool allowBackwards = false;    // if false, progress only increases

        [Header("Rotation")]
        public bool rotateToPath = true;
        public float rotationLerpSpeed = 20f; // higher the snappier
        public Vector2 spriteForwardAxis = Vector2.right;

        [Header("Quality")]
        [Tooltip("If line has few points, you can optionally densify it.")]
        public bool densify = false;
        [Tooltip("Points inserted per segment when densify is enabled.")]
        [Range(0, 20)] public int extraPointsPerSegment = 5;

        [Header("Auto Return To Start")]
        public bool returnToStartOnRelease = true;
        public float returnSpeedUnitsPerSec = 4f;   // speed along the line in world units/sec
        public float returnMinDuration = 0.15f;     // prevents instant snap on tiny distances
        public float returnMaxDuration = 2.0f;      // prevents very long returns
        Coroutine returnRoutine;

        [Header("Progress")]
        [Range(0f, 1f)] public float progress01;     // 0..1
        public UnityEvent<float> onProgress01;
        public bool StopInteractingAfterComplete;
        public float onFinishEventDelay = .5f;
        public UnityEvent OnFinish;
        public UnityEvent OnFinishNonDelay;
        //public LineRenderer myLine;



        private Vector3[] pts;               // line points in world space
        private float[] cumulativeLen;       // cumulative length along polyline
        private float totalLen;

        private bool dragging;

        private bool isComplete;

        private bool isPointerOnObject;

        public UnityEvent onReturnBack;

        [Header("Anti Skip")]
        public bool useProjectedDragging = true;
        public float maxStepDistancePerFrame = 0.5f; // safety clamp
        public float pointerToPathSensitivity = 1f;  // 1 = normal, tweak if needed

        private Vector3 lastPointerWorld;
        private bool hasLastPointerWorld;
        private float currentDistAlongPath;

        void Awake()
        {
            RebuildPathCache();
            currentDistAlongPath = Mathf.Clamp01(progress01) * totalLen;
            SnapToProgress(progress01);
            // myLine = GetComponent<LineRenderer>();
        }

        void OnValidate()
        {
            if (line != null && Application.isPlaying)
                RebuildPathCache();
        }

        // Call this if you change line vertices at runtime.
        public void RebuildPathCache()
        {
            if (line == null) return;

            int count = line.positionCount;
            if (count < 2) return;

            // Get line positions in WORLD space
            Vector3[] raw = new Vector3[count];
            line.GetPositions(raw);
            if (!line.useWorldSpace)
            {
                // convert local to world
                for (int i = 0; i < raw.Length; i++)
                    raw[i] = line.transform.TransformPoint(raw[i]);
            }

            pts = densify ? Densify(raw, extraPointsPerSegment) : raw;

            // Build cumulative lengths
            cumulativeLen = new float[pts.Length];
            cumulativeLen[0] = 0f;
            totalLen = 0f;

            for (int i = 1; i < pts.Length; i++)
            {
                totalLen += Vector3.Distance(pts[i - 1], pts[i]);
                cumulativeLen[i] = totalLen;
            }

            // Avoid divide by zero
            if (totalLen < 0.0001f) totalLen = 0.0001f;
        }

        void Update()
        {
            if (line == null || pts == null || pts.Length < 2 || (StopInteractingAfterComplete && isComplete)) return;

            if (PointerDown())
            {

                dragging = true;
                if (returnRoutine != null) { StopCoroutine(returnRoutine); returnRoutine = null; }
            }

            if (PointerUp())
            {
                dragging = false;

                if (returnToStartOnRelease && !isComplete)
                {
                    if (returnRoutine != null) StopCoroutine(returnRoutine);
                    returnRoutine = StartCoroutine(ReturnToStartRoutine());
                }
                // myLine.positionCount =0;
            }
            if (!dragging) return;

            Vector3 world = PointerToWorldOnZPlane();


            // Find closest point on polyline and its "distance along path"
            ClosestPointOnPolyline(world, out Vector3 closest, out float distAlong, out Vector3 tangentDir);

            float newProgress = Mathf.Clamp01(distAlong / totalLen);

            if (!allowBackwards)
                newProgress = Mathf.Max(progress01, newProgress);

            progress01 = newProgress;

            // Place object on the line at that progress
            transform.position = closest;
            // myLine.positionCount +=1;
            //  Vector3 pos = this.transform.position;
            // pos.z = -3;
            // myLine.SetPosition(myLine.positionCount-1,pos);



            if (rotateToPath)
            {
                // Angle of tangent in XY
                float pathAngle = Mathf.Atan2(tangentDir.y, tangentDir.x) * Mathf.Rad2Deg;

                // If your sprite doesn't face right by default, offset it using spriteForwardAxis
                float spriteAngle = Mathf.Atan2(spriteForwardAxis.y, spriteForwardAxis.x) * Mathf.Rad2Deg;

                float targetZ = pathAngle - spriteAngle;

                Quaternion targetRot = Quaternion.Euler(0f, 0f, targetZ);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationLerpSpeed);
            }

            onProgress01?.Invoke(progress01);

            if (!isComplete && progress01 == 1)
            {
                isComplete = true;
                DOVirtual.DelayedCall(onFinishEventDelay, () => { OnFinish?.Invoke(); });
                OnFinishNonDelay?.Invoke();
            }
        }

        public void SnapToProgress(float p01)
        {
            if (pts == null || pts.Length < 2) return;

            p01 = Mathf.Clamp01(p01);
            currentDistAlongPath = p01 * totalLen;

            Vector3 pos = PointAtDistance(currentDistAlongPath);
            transform.position = pos;
            progress01 = p01;
            onProgress01?.Invoke(progress01);
        }

        // --- Core math: closest point on polyline segments ---
        void ClosestPointOnPolyline(Vector3 p, out Vector3 closest, out float distAlongPath, out Vector3 tangentDir)
        {
            float bestDistSq = float.PositiveInfinity;
            closest = pts[0];
            distAlongPath = 0f;
            tangentDir = (pts[1] - pts[0]).normalized;

            for (int i = 0; i < pts.Length - 1; i++)
            {
                Vector3 a = pts[i];
                Vector3 b = pts[i + 1];

                Vector3 c = ClosestPointOnSegment(a, b, p, out float t);
                float dSq = (p - c).sqrMagnitude;

                if (dSq < bestDistSq)
                {
                    bestDistSq = dSq;
                    closest = c;

                    float segLen = Vector3.Distance(a, b);
                    distAlongPath = cumulativeLen[i] + (t * segLen);

                    Vector3 dir = (b - a);
                    tangentDir = (dir.sqrMagnitude < 1e-8f) ? tangentDir : dir.normalized;
                }
            }
        }

        static Vector3 ClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 p, out float t)
        {
            Vector3 ab = b - a;
            float abLenSq = ab.sqrMagnitude;

            if (abLenSq < 1e-8f)
            {
                t = 0f;
                return a;
            }

            t = Vector3.Dot(p - a, ab) / abLenSq;
            t = Mathf.Clamp01(t);
            return a + ab * t;
        }

        Vector3 PointAtDistance(float dist)
        {
            dist = Mathf.Clamp(dist, 0f, totalLen);

            // find segment containing this distance
            for (int i = 0; i < pts.Length - 1; i++)
            {
                float d0 = cumulativeLen[i];
                float d1 = cumulativeLen[i + 1];

                if (dist >= d0 && dist <= d1)
                {
                    float segLen = d1 - d0;
                    float t = (segLen < 1e-6f) ? 0f : (dist - d0) / segLen;
                    return Vector3.Lerp(pts[i], pts[i + 1], t);
                }
            }

            return pts[pts.Length - 1];
        }

        Vector3[] Densify(Vector3[] raw, int extraPerSeg)
        {
            if (extraPerSeg <= 0) return raw;

            int segs = raw.Length - 1;
            int newCount = raw.Length + segs * extraPerSeg;
            Vector3[] dense = new Vector3[newCount];

            int idx = 0;
            for (int i = 0; i < raw.Length - 1; i++)
            {
                Vector3 a = raw[i];
                Vector3 b = raw[i + 1];

                dense[idx++] = a;

                for (int k = 1; k <= extraPerSeg; k++)
                {
                    float t = k / (float)(extraPerSeg + 1);
                    dense[idx++] = Vector3.Lerp(a, b, t);
                }
            }

            dense[idx++] = raw[raw.Length - 1];
            return dense;
        }

        public Vector3 GetPositionByProgress(float val)
        {
            if (pts == null || pts.Length < 2)
                return Vector3.zero;

            val = Mathf.Clamp01(val);

            float targetDist = val * totalLen;

            // find which segment this distance falls into
            for (int i = 0; i < pts.Length - 1; i++)
            {
                float d0 = cumulativeLen[i];
                float d1 = cumulativeLen[i + 1];

                if (targetDist >= d0 && targetDist <= d1)
                {
                    float segLen = d1 - d0;
                    float t = segLen > 0f ? (targetDist - d0) / segLen : 0f;

                    return Vector3.Lerp(pts[i], pts[i + 1], t);
                }
            }

            // fallback (should only happen at val = 1)
            return pts[pts.Length - 1];
        }

        // --- Pointer helpers (touch + mouse) ---
        static Vector3 PointerPos()
        {
            if (Input.touchCount > 0) return Input.GetTouch(0).position;
            return Input.mousePosition;
        }

        void OnMouseDown()
        {
            isPointerOnObject = true;
            UtilityEventsManager.OnUserInteracted?.Invoke(this, new UtilityEventsManager.UserInteracted(gameObject));
        }

        void OnMouseUp()
        {
            isPointerOnObject = false;
        }

        void OnMouseExit()
        {
            isPointerOnObject = false;
        }

        bool PointerDown()
        {
            /* if (Input.touchCount > 0) return Input.GetTouch(0).phase == TouchPhase.Began;
             return Input.GetMouseButtonDown(0);*/
            return isPointerOnObject;
        }

        bool onPointerUpCalled;
        bool PointerUp()
        {

            /* if (Input.touchCount > 0)
             {
                 var ph = Input.GetTouch(0).phase;
                 return ph == TouchPhase.Ended || ph == TouchPhase.Canceled;
             }
             return Input.GetMouseButtonUp(0);*/
            /* if (isPointerOnObject)
             {
                 isPointerOnObject= false;
                 return true;
             }else
             {
                 return false;
             }*/


            if (isPointerOnObject)
            {
                onPointerUpCalled = false;
                return false;
            }
            if (onPointerUpCalled)
            {
                return false;
            }
            else
            {
                onPointerUpCalled = true;
                return true;
            }
            //return !isPointerOnObject;
        }

        Vector3 PointerToWorldOnZPlane()
        {
            Ray ray = cam.ScreenPointToRay(PointerPos());

            float denom = ray.direction.z;
            if (Mathf.Abs(denom) < 1e-6f)
            {
                // Fallback if camera is parallel to plane
                Vector3 w = cam.ScreenToWorldPoint(new Vector3(PointerPos().x, PointerPos().y, cam.nearClipPlane));
                w.z = zPlane;
                return w;
            }

            float t = (zPlane - ray.origin.z) / denom;
            Vector3 p = ray.origin + ray.direction * t;
            p.z = zPlane;
            return p;
        }

        bool isFirstTime = true;
        System.Collections.IEnumerator ReturnToStartRoutine()
        {
            // distance along path right now
            float startDist = progress01 * totalLen;
            float endDist = 0f;
            if (!isFirstTime)
            {
                onReturnBack?.Invoke();
            }
            else
            {
                isFirstTime = false;
            }
            float distToTravel = Mathf.Abs(startDist - endDist);

            // duration based on units/sec speed
            float duration = distToTravel / Mathf.Max(0.001f, returnSpeedUnitsPerSec);
            duration = Mathf.Clamp(duration, returnMinDuration, returnMaxDuration);

            float tTime = 0f;

            while (tTime < 1f)
            {
                tTime += Time.deltaTime / duration;
                float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(tTime));

                float curDist = Mathf.Lerp(startDist, endDist, eased);

                // Move
                Vector3 pos = PointAtDistance(curDist);
                transform.position = pos;

                // Update progress
                progress01 = Mathf.Clamp01(curDist / totalLen);
                onProgress01?.Invoke(progress01);

                // Rotate to tangent (2D) if enabled
                if (rotateToPath)
                {
                    Vector3 tan = TangentAtDistance(curDist);
                    if (tan.sqrMagnitude > 1e-8f)
                    {
                        float pathAngle = Mathf.Atan2(tan.y, tan.x) * Mathf.Rad2Deg;
                        float spriteAngle = Mathf.Atan2(spriteForwardAxis.y, spriteForwardAxis.x) * Mathf.Rad2Deg;
                        float targetZ = pathAngle - spriteAngle;

                        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetZ);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationLerpSpeed);
                    }
                }

                yield return null;
            }

            // Ensure exact start
            progress01 = 0f;
            transform.position = pts[0];
            onProgress01?.Invoke(progress01);
            returnRoutine = null;
        }

        Vector3 TangentAtDistance(float dist)
        {
            dist = Mathf.Clamp(dist, 0f, totalLen);

            for (int i = 0; i < pts.Length - 1; i++)
            {
                float d0 = cumulativeLen[i];
                float d1 = cumulativeLen[i + 1];

                if (dist >= d0 && dist <= d1)
                {
                    Vector3 dir = pts[i + 1] - pts[i];
                    return (dir.sqrMagnitude < 1e-8f) ? Vector3.right : dir.normalized;
                }
            }

            // fallback: last segment direction
            Vector3 last = pts[pts.Length - 1] - pts[pts.Length - 2];
            return (last.sqrMagnitude < 1e-8f) ? Vector3.right : last.normalized;
        }
        public Vector3 GetLastPointWorld()
        {
            if (line == null || line.positionCount == 0)
                return Vector3.zero;

            int lastIndex = line.positionCount - 1;
            Vector3 pos = line.GetPosition(lastIndex);

            if (!line.useWorldSpace)
                pos = line.transform.TransformPoint(pos);

            return pos;
        }
    }
}