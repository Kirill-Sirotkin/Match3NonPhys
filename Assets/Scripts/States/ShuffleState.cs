using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class ShuffleState : State
    {
        public ShuffleState(GameManager manager) : base(manager)
        {
        }

        public override void StartAction()
        {
            bool movesExist = false;

            foreach (Piece p in gameManager._piecesParent.GetComponentsInChildren<Piece>())
            {
                if (CheckForAvailableMoves(p.transform.position, _pattern1) ||
                    CheckForAvailableMoves(p.transform.position, _pattern2) ||
                    CheckForAvailableMoves(p.transform.position, _pattern3) ||
                    CheckForAvailableMoves(p.transform.position, _pattern4))
                {
                    movesExist = true;
                    break;
                }
            }

            if (movesExist) { gameManager.SetState(new PlayerState(gameManager)); return; }
            Sequence seq = Shuffle();
            seq.OnComplete(() => { gameManager.SetState(new PatternState(gameManager)); });
        }

        #region Own methods

        private List<Vector3> _pattern1 = new List<Vector3> { new Vector3(1f, 1f, 0f), new Vector3(2f, 0f, 0f) };
        private List<Vector3> _pattern2 = new List<Vector3> { new Vector3(1f, 0f, 0f), new Vector3(3f, 0f, 0f) };
        private List<Vector3> _pattern3 = new List<Vector3> { new Vector3(1f, 0f, 0f), new Vector3(2f, 1f, 0f) };
        private List<Vector3> _pattern4 = new List<Vector3> { new Vector3(1f, 0f, 0f), new Vector3(2f, -1f, 0f) };

        private bool CheckForAvailableMoves(Vector3 pos, List<Vector3> pattern)
        {
            List<Vector3> rotatingPattern = new List<Vector3>(pattern);
            Vector3 rayStart = new Vector3(0f, 0f, -1.5f);
            PieceType pieceType = gameManager.GetRayPiece(pos + rayStart, Vector3.forward)._type;

            for (int i = 0; i < 4; i++)
            {
                Piece checkPiece1 = gameManager.GetRayPiece(pos + rotatingPattern[0] + rayStart, Vector3.forward);
                Piece checkPiece2 = gameManager.GetRayPiece(pos + rotatingPattern[1] + rayStart, Vector3.forward);
                if (checkPiece1 != null && checkPiece2 != null &&
                    checkPiece1._type == pieceType && checkPiece2._type == pieceType)
                {
                    return true;
                }

                rotatingPattern[0] = new Vector3(rotatingPattern[0].y, -1 * rotatingPattern[0].x);
                rotatingPattern[1] = new Vector3(rotatingPattern[1].y, -1 * rotatingPattern[1].x);
            }

            return false;
        }

        private Sequence Shuffle()
        {
            Sequence seq = DOTween.Sequence();
            List<Piece> pieces = new List<Piece>();
            foreach (Piece p in gameManager._piecesParent.GetComponentsInChildren<Piece>())
            {
                pieces.Add(p);
            }

            List<Piece> shuffledPieces = new List<Piece>();
            List<Piece> copiedPieces = new List<Piece>(pieces);
            for (int i = 0; i < pieces.Count; i++)
            {
                int index = Random.Range(0, copiedPieces.Count);

                shuffledPieces.Add(copiedPieces[index]);
                copiedPieces.RemoveAt(index);
            }

            for (int i = 0; i < pieces.Count; i++)
            {
                seq.Join(pieces[i].Move(shuffledPieces[i].transform.position));
            }

            return seq;
        }

        #endregion
    }
}
