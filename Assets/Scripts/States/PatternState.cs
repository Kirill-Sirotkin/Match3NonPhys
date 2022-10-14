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

            if (_piecePatterns.Count == 0 && _lastSwappedPieces != null) 
            {
                Debug.Log("swap");
                seq = gameManager.SwapPieces(_lastSwappedPieces[0], _lastSwappedPieces[1]);
                seq.OnComplete(() => { gameManager.SetState(new PlayerState(gameManager)); });
                return;
            }
            // Check for special pieces to be spawned

            if (_piecePatterns.Count > 0)
            {
                gameManager.SetState(new DespawnState(gameManager, _piecePatterns));
                return;
            }

            gameManager.SetState(new ShuffleState(gameManager));
        }

        #region Own methods

        Piece[] _lastSwappedPieces;
        Dictionary<Piece, List<Piece>> _piecePatterns = new Dictionary<Piece, List<Piece>>();

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

            patternPieces.Add(piece);
            if (patternPieces.Count < 3) { return; }

            List<Piece> matches = new List<Piece>(patternPieces);
            matches.RemoveAt(matches.Count - 1);

            if (!_piecePatterns.ContainsKey(piece)) { _piecePatterns.Add(piece, matches); }
        }

        #endregion
    }
}