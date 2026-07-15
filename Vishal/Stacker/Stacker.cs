using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace USP.MiniGame.recognitionPatterns
{
    /// <summary>
    /// Script by Vishal Lakhani
    /// Email: usp.vishal@gmail.com
    /// Description:
    /// </summary>
    public class Stacker : MonoBehaviour
    {

        public List<Piece> Objects;
        List<string> reference;
        public float stackheight = .5f;
        public float duration = .3f;
        Vector3 firstPos;

        public void Reposition(bool fast = false)
        {
            int index = 0;

            foreach (var x in Objects)
            {
                if (!fast)
                {
                    x.transform.DOMove(firstPos + new Vector3(0, stackheight * index, 0), duration);
                }
                else
                {
                    x.transform.position = firstPos + new Vector3(0, stackheight * index, 0);
                }
                index++;
            }
        }

        public void Reposition()
        {
            int index = 0;

            foreach (var x in Objects)
            {

                x.transform.DOMove(firstPos + new Vector3(0, stackheight * index, 0), duration);

                index++;
            }
        }

        void Awake()
        {
            firstPos = Objects[0].transform.position;
            reference = new();
            UtilityEventsManager.OnItemRemoved += OnItemRemoved;
            UtilityEventsManager.OnGoBack += OnGoBack;
            int index = 0;
            foreach (var x in Objects)
            {
                x.StackIndex = index;
                reference.Add(x.GetInstanceID().ToString());
                index++;
            }
            Reposition(fast: true);

        }

        void OnItemRemoved(Piece p)
        {
            if (Objects.Contains(p))
                Objects.Remove(p);
            Reposition();
        }

        void OnGoBack(Piece p)
        {
            Debug.Log("Go back Called");
            if (reference.Contains(p.GetInstanceID().ToString()) && !Objects.Contains(p))
            {
                int index = p.StackIndex;
                Objects.Insert(index, p);
            }
            CancelInvoke(nameof(Reposition));
            Invoke(nameof(Reposition), .3f);
            // Invoke(nameof(Reposition), .5f);
        }

        void OnDisable()
        {
            UtilityEventsManager.OnItemRemoved -= OnItemRemoved;
            UtilityEventsManager.OnItemRemoved -= OnGoBack;
        }
    }
}