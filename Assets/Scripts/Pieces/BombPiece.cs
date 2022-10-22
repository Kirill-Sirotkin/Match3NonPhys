using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class BombPiece : Piece, ISpecialPiece, IClickable
    {
        public List<Piece> SpecialMove()
        {
            List<Piece> pieces = new List<Piece>();

            Piece p;
            if (_manager.GetRayPiece(transform.position, Vector3.up) != null)
            {
                p = _manager.GetRayPiece(transform.position, Vector3.up);
                if (p != null) { pieces.Add(p); }
            }

            p = _manager.GetRayPiece(transform.position, Vector3.right);
            if (p != null) { pieces.Add(p); }

            p = _manager.GetRayPiece(transform.position, Vector3.down);
            if (p != null) { pieces.Add(p); }

            p = _manager.GetRayPiece(transform.position, Vector3.left);
            if (p != null) { pieces.Add(p); }

            return pieces;
        }
        public void ClickAction()
        {
            SpecialMove();
            ToggleHighlight();
        }
    }
}
