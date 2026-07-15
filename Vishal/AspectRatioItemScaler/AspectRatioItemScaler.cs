// SCRIPT BY VISHAL LAKHANI ( USP.VISHAL@GMAIL.COM)
using UnityEngine;
using UnityEngine.UIElements;
// SCRIPT BY VISHAL LAKHANI ( USP.VISHAL@GMAIL.COM)
public class AspectRatioItemScaler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float tabSize, mobileSize;
    private void Awake()
    {
        UpdateSize();
    }

    [ContextMenu("Update Position")]
    void UpdateSize()
    {
        if (IsWideAspect)
        {
            this.transform.localScale = Vector2.one * mobileSize;
        }
        else
        {
            this.transform.localScale = Vector2.one * tabSize;
        }
    }

    public bool IsWideAspect
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var size = UnityEditor.Handles.GetMainGameViewSize();

                float aspect = size.x / size.y;

                return aspect <= 1.29f || aspect >= 1.36f;
            }
#endif

            float runtimeAspect = (float)Screen.width / Screen.height;

            return runtimeAspect <= 1.29f || runtimeAspect >= 1.36f;
        }
    }
}



// SCRIPT BY VISHAL LAKHANI ( USP.VISHAL@GMAIL.COM)