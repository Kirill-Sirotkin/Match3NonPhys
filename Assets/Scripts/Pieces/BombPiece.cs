using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class BombPiece : Piece, IClickable, ISpecialPiece
    {
        public GameManager _gameManager { get; private set; } = null;

        public void SetGameManager(GameManager gm)
        {
            _gameManager = gm;
        }
        public new void ClickAction()
        {
            ToggleHighlight();
            SpecialMoveAnimation();
        }
        public Sequence SpecialMoveAnimation()
        {
            Sequence seq = DOTween.Sequence();

            transform.DOKill();
            transform.localScale = transform.parent.localScale;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

            seq.Join(transform.DOMoveZ(-0.25f, 0.001f));
            seq.Join(ScaleAnimation(0.75f, 3f));
            seq.Append(ScaleAnimation(3f, 0.75f));
            seq.Append(transform.DOMoveZ(0f, 0.001f));

            return seq;
        }
        private Sequence ScaleAnimation(float horizontalScale, float verticalScale)
        {
            Sequence seq = DOTween.Sequence();

            Vector3 startScale = transform.localScale;

            seq.Join(transform.DOScale(
                new Vector3(
                    transform.localScale.x * horizontalScale, 
                    transform.localScale.y * verticalScale, 
                    transform.localScale.z), 
                0.125f));

            seq.Append(transform.DOScale(
                new Vector3(
                    startScale.x,
                    startScale.y,
                    startScale.z),
                0.125f));

            return seq;
        }
        public List<Piece> SpecialMovePieces()
        {
            List<Piece> piecesToDespawn = new List<Piece>();

            Vector3[] directions = new Vector3[]
            {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right
            };

            foreach(Vector3 dir in directions)
            {
                Piece piece = _gameManager.GetRayPiece(transform.position, dir);
                if (piece != null)
                {
                    piecesToDespawn.Add(piece);
                }
            }

            return piecesToDespawn;
        }
    }
}
