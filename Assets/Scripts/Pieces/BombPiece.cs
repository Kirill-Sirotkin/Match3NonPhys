using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class BombPiece : Piece, ISpecialPiece, IClickable
    {
        public Sequence SpecialMove()
        {
            Sequence seq = DOTween.Sequence();

            seq.Join(_manager.GetRayPiece(transform.position, Vector3.up).Despawn());
            seq.Join(_manager.GetRayPiece(transform.position, Vector3.right).Despawn());
            seq.Join(_manager.GetRayPiece(transform.position, Vector3.down).Despawn());
            seq.Join(_manager.GetRayPiece(transform.position, Vector3.left).Despawn());

            return seq;
        }
        public void ClickAction()
        {
            SpecialMove();
        }
    }
}
