using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class LightningPiece : Piece, ISpecialPiece, IClickable
    {
        public List<Piece> SpecialMove()
        {
            List<Piece> pieces = new List<Piece>();

            foreach(Piece p in _manager._piecesParent.GetComponentsInChildren<Piece>())
            {
                if (p._type == _type && p.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    pieces.Add(p);
                }
            }

            return pieces;
        }
        public void ClickAction()
        {
            //SpecialMove();
            ToggleHighlight();
        }
    }
}
