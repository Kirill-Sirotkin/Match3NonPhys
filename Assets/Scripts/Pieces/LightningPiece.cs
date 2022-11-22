using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class LightningPiece : Piece, ISpecialPiece
    {
        public void SetGameManager(GameManager gm)
        {
            _gameManager = gm;
        }
        public Sequence SpecialMoveAnimation()
        {
            Sequence seq = DOTween.Sequence();
            Sequence subSeq = DOTween.Sequence();

            subSeq.Append(ScaleAnimation(0.75f, 3f));
            subSeq.Join(Twist());
            subSeq.Append(ScaleAnimation(3f, 0.75f));
            subSeq.Join(Twist());

            seq.Join(_visual.transform.DOMoveZ(-0.25f, 0.001f));
            seq.Append(subSeq);
            seq.Append(_visual.transform.DOMoveZ(0f, 0.001f));

            return seq;
        }
        public List<Piece> SpecialMovePieces()
        {
            List<Piece> piecesToDespawn = new List<Piece>();

            foreach(Piece p in _gameManager._piecesParent.GetComponentsInChildren<Piece>())
            {
                if (p.transform.position == transform.position) continue;
                if (p._type == _type) { piecesToDespawn.Add(p); }
            }

            return piecesToDespawn;
        }
    }
}
