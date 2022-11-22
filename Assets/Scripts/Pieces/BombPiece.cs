using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class BombPiece : Piece, ISpecialPiece
    {
        public void SetGameManager(GameManager gm)
        {
            _gameManager = gm;
        }
        public Sequence SpecialMoveAnimation()
        {
            Sequence seq = DOTween.Sequence();

            seq.Join(_visual.transform.DOMoveZ(-0.25f, 0.001f));
            seq.Join(ScaleAnimation(0.75f, 3f));
            seq.Append(ScaleAnimation(3f, 0.75f));
            seq.Append(_visual.transform.DOMoveZ(0f, 0.001f));

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
