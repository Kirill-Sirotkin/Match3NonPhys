using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class LightningPiece : Piece, ISpecialPiece, IClickable
    {
        public Sequence SpecialMove()
        {
            Sequence seq = DOTween.Sequence();
            return seq;
        }
        public void ClickAction()
        {
            SpecialMove();
        }
    }
}
