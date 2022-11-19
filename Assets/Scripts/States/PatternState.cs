using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class PatternState : State
    {
        public PatternState(GameManager manager, Piece[] lastSwappedPieces = null) : base(manager)
        {
            _lastSwappedPieces = lastSwappedPieces;
        }

        public override void StartAction()
        {
            Sequence seq = DOTween.Sequence();
            foreach(Piece p in gameManager._piecesParent.GetComponentsInChildren<Piece>())
            {
                AssignPatterns(p);
            }
            Debug.Log(_piecePatterns.Count);

            foreach (Piece p in _piecePatterns.Keys)
            {
                Debug.Log(p.transform.position);
            }

            SeparatePatterns();

            if (_piecePatterns.Count == 0 && _lastSwappedPieces != null) 
            {
                seq = gameManager.SwapPieces(_lastSwappedPieces[0], _lastSwappedPieces[1]);
                seq.OnComplete(() => { gameManager.SetState(new PlayerState(gameManager)); });
                return;
            }

            if (_piecePatterns.Count > 0)
            {
                gameManager.SetState(new DespawnState(gameManager, _piecePatterns, _separatePiecePatterns));
                return;
            }

            gameManager.SetState(new ShuffleState(gameManager));
        }

        #region Own methods

        Piece[] _lastSwappedPieces;
        Dictionary<Piece, List<Piece>> _piecePatterns = new Dictionary<Piece, List<Piece>>();
        List<Dictionary<Piece, List<Piece>>> _separatePiecePatterns = new List<Dictionary<Piece, List<Piece>>>();

        private void AssignPatterns(Piece piece)
        {
            List<Piece> patternPieces = new List<Piece>();
            List<Piece> directionPieces = new List<Piece>();
            PieceType type = piece._type;
            Vector3[,] directions = new Vector3[,]
            {
                { Vector3.up, Vector3.down },
                { Vector3.left,Vector3.right }
            };

            for (int i = 0; i < 2; i++)
            {
                directionPieces.Clear();

                for (int j = 0; j < 2; j++)
                {
                    Vector3 origin = piece.transform.position;
                    Vector3 direction = directions[i, j];

                    for (int u = 0; u < 7; u++)
                    {
                        Piece checkedPiece = gameManager.GetRayPiece(origin, direction);
                        if (checkedPiece == null) { break; }
                        if (checkedPiece._type != type) { break; }

                        origin = checkedPiece.transform.position;
                        directionPieces.Add(checkedPiece);
                    }
                }

                if (directionPieces.Count < 2) { continue; }
                foreach (Piece p in directionPieces)
                {
                    patternPieces.Add(p);
                }
            }

            if (patternPieces.Count < 2) { return; }
            if (!_piecePatterns.ContainsKey(piece)) { _piecePatterns.Add(piece, patternPieces); }
        }

        private void SeparatePatterns()
        {
            List<Piece> uniquePiecesChecklist = new List<Piece>();
            List<Piece> uniquePiecesIteration = new List<Piece>();
            Dictionary<Piece, List<Piece>> uniquePatterns = new Dictionary<Piece, List<Piece>>();

            foreach (KeyValuePair<Piece, List<Piece>> pair in _piecePatterns)
            {
                if (uniquePiecesChecklist.Contains(pair.Key)) { continue; }

                int baseMatchesCount = pair.Value.Count;
                Piece mostMatchedPiece = pair.Key;

                foreach (Piece p in pair.Value)
                {
                    int matchesCount = _piecePatterns[p].Count;

                    if (matchesCount > baseMatchesCount)
                    {
                        baseMatchesCount = matchesCount;
                        mostMatchedPiece = p;
                    }
                }

                uniquePiecesIteration.Clear();
                uniquePiecesIteration.Add(mostMatchedPiece);
                uniquePiecesIteration.AddRange(_piecePatterns[mostMatchedPiece]);

                uniquePatterns.Clear();
                foreach (Piece p in uniquePiecesIteration)
                {
                    uniquePatterns.Add(p, _piecePatterns[p]);
                }

                _separatePiecePatterns.Add(uniquePatterns);
                uniquePiecesChecklist.AddRange(uniquePiecesIteration);
            }
        }

        #endregion
    }
}