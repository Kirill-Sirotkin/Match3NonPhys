using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Additional
using DG.Tweening;

namespace Match3NonPhys
{
    public class Piece : MonoBehaviour, IClickable
    {
        [field: SerializeField] public PieceType _type { get; private set; }
        [field: SerializeField] protected GameObject _highlight;
        [field: SerializeField] protected GameObject _visual;
        public bool _isIdle { get; protected set; } = true;
        protected GameManager _gameManager = null;

        public Tween Move(Vector3 pos)
        {
            ActivatePieceAnimation();
            return transform.DOMove(pos, 0.5f).SetEase(Ease.OutBack)
                .OnComplete(()=> { _isIdle = true; });
        }
        public Tween Spin()
        {
            ActivatePieceAnimation();
            return _visual.transform.DORotate(new Vector3(0f, 360f, 0f), 0.25f, RotateMode.FastBeyond360)
                .OnComplete(() => { _isIdle = true; });
        }
        public Tween Shrink()
        {
            ActivatePieceAnimation();
            return _visual.transform.DOScale(Vector3.zero, 0.35f).SetEase(Ease.InBack)
                .OnComplete(() => { _isIdle = true; });
        }
        public Tween Twist()
        {
            ActivatePieceAnimation();
            return _visual.transform.DORotate(new Vector3(0f, 0f, 360f), 0.25f, RotateMode.FastBeyond360)
                .OnComplete(() => { _isIdle = true; });
        }
        public Sequence ScaleAnimation(float horizontalScale, float verticalScale)
        {
            ActivatePieceAnimation();

            Sequence seq = DOTween.Sequence();

            Vector3 startScale = transform.localScale;

            seq.Join(_visual.transform.DOScale(
                new Vector3(
                    transform.localScale.x * horizontalScale,
                    transform.localScale.y * verticalScale,
                    transform.localScale.z),
                0.125f));

            seq.Append(_visual.transform.DOScale(
                new Vector3(
                    startScale.x,
                    startScale.y,
                    startScale.z),
                0.125f));

            return seq
                .OnComplete(() => { _isIdle = true; });
        }
        public Sequence Despawn()
        {
            ActivatePieceAnimation();

            Sequence seq = DOTween.Sequence();
            seq.Append(Spin());
            seq.Join(Shrink());
            seq.OnComplete(()=> 
            {
                transform.DOKill();
                _isIdle = true;
                gameObject.SetActive(false);
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
        private void ActivatePieceAnimation()
        {
            transform.DOKill();
            _isIdle = false;
            ToggleHighlight(false);
        }
    }

    public enum PieceType
    {
        Red,
        Blue,
        Yellow,
        Green,
        Purple,
        Random
    }

    public enum PieceSpecialType
    {
        Regular,
        Bomb,
        Lightning
    }
}
