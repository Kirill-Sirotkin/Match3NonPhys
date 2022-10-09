using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            foreach(Piece p in gameManager._piecesParent.GetComponentsInChildren<Piece>())
            {
                AssignPatterns(p);
            }
            Debug.Log("pieces to despawn: " + _matchedPieces.Count);
            Debug.Log("piece patterns: " + _piecePatterns.Count);
            foreach(KeyValuePair<Piece,List<Piece>> entry in _piecePatterns)
            {
                Debug.Log("For piece: " + entry.Key.gameObject.name + " at " + entry.Key.transform.position + ", matches are: " + entry.Value.Count);
            }
        }

        #region Own methods

        Piece[] _lastSwappedPieces;
        Dictionary<int, Piece> _matchedPieces = new Dictionary<int, Piece>();
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

            if (patternPieces.Count > 4) { Debug.Log("5 or more pieces matched! at: " + piece.transform.position); }
            else if (patternPieces.Count == 4) { Debug.Log("4 pieces matched! at: " + piece.transform.position); }

            //Debug.Log("for piece: " + piece.name + " pattern is " + patternPieces.Count + " (" + piece.transform.position + ")");

            List<Piece> matches = new List<Piece>(patternPieces);
            matches.RemoveAt(matches.Count - 1);

            if (!_piecePatterns.ContainsKey(piece))
            {
                _piecePatterns.Add(piece, matches);
            }

            //foreach (Piece p in patternPieces)
            //{
            //    if (_matchedPieces.ContainsKey(p.gameObject.GetInstanceID())) { continue; }
            //    _matchedPieces.Add(p.gameObject.GetInstanceID(), p);
            //}
        }

        #endregion
    }
}