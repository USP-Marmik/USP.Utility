using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace USP.MiniGame.recognitionPatterns
{
    /// <summary>
    /// Script by Vishal Lakhani
    /// Email: usp.vishal@gmail.com
    /// Description:
    /// </summary>
    public class StackItemDrop : MonoBehaviour
    {
        #region Variables
        public Transform startPlace;
        public Transform EndPlace;
        public List<GameObject> ItemsInHand;
        public int maxCount;
        public float size = .5f;
        public float duration = .3f;
        public bool useCustomRepositionAtEnd;
        public float repositionSize = .5f;
        public UnityEvent OnReposition;

        public int setLayer = 10;
        #endregion

        #region Unity Methods

        public void CustomReposition()
        {
            DOVirtual.DelayedCall(.5f, () =>
            {
                int count = 0;
                foreach (var x in ItemsInHand)
                {
                    x.transform.DOMove(new Vector3(EndPlace.transform.position.x,
                    EndPlace.transform.position.y + repositionSize * count,
                    EndPlace.transform.position.z - (count * .1f)), duration).SetDelay(.1f * count);
                    count++;
                }
                OnReposition?.Invoke();
            });
        }

        public void Take(GameObject itm, Vector3 ogScale)
        {
            if (startPlace)
            {
                itm.transform.DOMove(startPlace.position, duration);


                itm.transform.DOMove(new Vector3(EndPlace.transform.position.x, EndPlace.transform.position.y + size * ItemsInHand.Count, EndPlace.transform.position.z), duration).SetDelay(duration);
                itm.transform.DOScale(ogScale, duration).SetEase(Ease.OutBounce).SetDelay(duration * 2);
                if (!ItemsInHand.Contains(itm))
                {
                    ItemsInHand.Add(itm);

                }
            }
            else
            {
                itm.transform.DOMove(new Vector3(EndPlace.transform.position.x, EndPlace.transform.position.y + size * ItemsInHand.Count, EndPlace.transform.position.z), duration);
                itm.transform.DOScale(ogScale, duration).SetEase(Ease.OutBounce).SetDelay(duration);
                if (!ItemsInHand.Contains(itm))
                {
                    ItemsInHand.Add(itm);
                }
            }
            var piece = itm.GetComponent<Piece>();
            if (piece)
            {
                piece.isComplete = true;
            }
            var itmSR = itm.GetComponent<SpriteRenderer>();
            var itmSRchild = itm.transform.GetChild(0).GetComponent<SpriteRenderer>();
            if (itmSRchild)
                itmSRchild.sortingOrder = setLayer + 1;
            if (itmSR)
                itmSR.sortingOrder = setLayer;

            if (useCustomRepositionAtEnd)
            {
                if (ItemsInHand.Count >= maxCount)
                {
                    CustomReposition();
                }
            }


        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion
    }
}