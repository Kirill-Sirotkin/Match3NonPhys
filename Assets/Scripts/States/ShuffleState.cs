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
                ISpecialPiece special = p.GetComponent<ISpecialPiece>();
                if (CheckForAvailableMoves(p.transform.position, _elbowPattern) ||
                    CheckForAvailableMoves(p.transform.position, _snowmanPattern) ||
                    CheckForAvailableMoves(p.transform.position, _cornerPattern) ||
                    CheckForAvailableMoves(p.transform.position, _cornerMirroredPattern) ||
                    special != null)
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

        private List<Vector3> _elbowPattern = new List<Vector3> { new Vector3(1f, 1f, 0f), new Vector3(2f, 0f, 0f) };
        private List<Vector3> _snowmanPattern = new List<Vector3> { new Vector3(1f, 0f, 0f), new Vector3(3f, 0f, 0f) };
        private List<Vector3> _cornerPattern = new List<Vector3> { new Vector3(1f, 0f, 0f), new Vector3(2f, 1f, 0f) };
        private List<Vector3> _cornerMirroredPattern = new List<Vector3> { new Vector3(1f, 0f, 0f), new Vector3(2f, -1f, 0f) };

        private bool CheckForAvailableMoves(Vector3 pos, List<Vector3> pattern)
        {
            Vector3 rayStart = new Vector3(0f, 0f, -1.5f);
            PieceType pieceType = gameManager.GetRayPiece(pos + rayStart, Vector3.forward)._type;

            List<Vector3> checkedPattern = new List<Vector3>(pattern);

            for (int i = 0; i < 4; i++)
            {
                bool isPatternTypeMatch = true;

                foreach(Vector3 vector in checkedPattern)
                {
                    if (!isPatternTypeMatch) { continue; }

                    Piece checkedPiece = gameManager.GetRayPiece(pos + rayStart + vector, Vector3.forward);
                    if (checkedPiece == null) { isPatternTypeMatch = false; continue; }

                    PieceType checkedPieceType = checkedPiece._type;
                    if (checkedPieceType != pieceType) { isPatternTypeMatch = false; }
                }

                if (isPatternTypeMatch) { return true; }

                checkedPattern = RotatePattern(checkedPattern);
            }

            return false;
        }
        private List<Vector3> RotatePattern(List<Vector3> pattern)
        {
            List<Vector3> rotatedPattern = new List<Vector3>();

            foreach(Vector3 vector in pattern)
            {
                rotatedPattern.Add(new Vector3(vector.y, vector.x * -1, vector.z));
            }

            return rotatedPattern;
        }
        private List<Vector3> MirrorPattern(List<Vector3> pattern)
        {
            List<Vector3> mirroredPattern = new List<Vector3>();

            foreach (Vector3 vector in pattern)
            {
                mirroredPattern.Add(new Vector3(vector.x, vector.y * -1, vector.z));
            }

            return mirroredPattern;
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
