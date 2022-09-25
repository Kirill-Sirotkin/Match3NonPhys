using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class Piece : MonoBehaviour, IClickable
    {
        [field: SerializeField] public PieceType _type { get; private set; }
        [field: SerializeField] private GameObject _highlight;
        [field: SerializeField] private GameObject _visual;

        public Tween Move(Vector3 pos)
        {
            transform.DOKill();
            ToggleHighlight(false);

            return transform.DOMove(pos, 0.5f).SetEase(Ease.OutBack);
        }
        public Tween Spin()
        {
            transform.DOKill();
            ToggleHighlight(false);

            return _visual.transform.DORotate(new Vector3(0f, 360f, 0f), 0.25f, RotateMode.FastBeyond360);
        }
        public Tween Shrink()
        {
            transform.DOKill();
            ToggleHighlight(false);

            return _visual.transform.DOScale(Vector3.zero, 0.35f).SetEase(Ease.InBack);
        }
        public Sequence Despawn()
        {
            transform.DOKill();
            ToggleHighlight(false);

            Sequence seq = DOTween.Sequence();
            seq.Append(Spin());
            seq.Join(Shrink());
            seq.OnComplete(()=> 
            {
                transform.DOKill();
                Destroy(gameObject);
            });

            return seq;
        }
        public void ToggleHighlight(bool b)
        {
            _highlight.SetActive(b);
        }
        public void ToggleHighlight()
        {
            _highlight.SetActive(!_highlight.activeSelf);
        }
        public void ClickAction()
        {
            ToggleHighlight();
        }
    }

    public enum PieceType
    {
        Red,
        Blue,
        Yellow,
        Green,
        Purple
    }
}
