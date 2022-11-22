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
            Debug.Log("Special piece activated: Bomb piece, " + transform.position);
            Debug.Log("Pieces to despawn: ");

            foreach (Piece p in SpecialMovePieces())
            {
                Debug.Log(p.transform.position);
            }
            Debug.Log("------------");
        }
        public Sequence SpecialMoveAnimation()
        {
            Sequence seq = DOTween.Sequence();
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
