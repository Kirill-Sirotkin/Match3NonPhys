using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class DespawnState : State
    {
        public DespawnState(GameManager manager, List<Pattern> patterns) : base(manager)
        {
            _patterns = patterns;
        }

        public override void StartAction()
        {
            Sequence seq = DOTween.Sequence();
            List<Vector3> spawnPoints = new List<Vector3>();
            Dictionary<Piece, int> specialPiecesSpawnPoints = new Dictionary<Piece, int>();
            List<Piece> piecesToDespawn = new List<Piece>();

            foreach(Pattern pat in _patterns)
            {
                List<Piece> patternPieces = new List<Piece>(pat._piecesInPattern);

                if (patternPieces.Count > 3)
                {
                    Piece specialPiece = GetSpecialPiecePosition(patternPieces);
                    specialPiecesSpawnPoints.Add(specialPiece, patternPieces.Count);
                    patternPieces.Remove(specialPiece);
                }

                piecesToDespawn.AddRange(patternPieces);
            }

            List<Piece> fullPieceList = new List<Piece>();
            fullPieceList.AddRange(specialPiecesSpawnPoints.Keys);
            fullPieceList.AddRange(piecesToDespawn);

            int startIndex = fullPieceList.Count;

            fullPieceList = GetPiecesFromSpecialMove(fullPieceList, 0, 0);

            for (int i = startIndex; i < fullPieceList.Count; i++)
            {
                piecesToDespawn.Add(fullPieceList[i]);
            }

            Sequence specialSeq = DOTween.Sequence();
            Sequence regularSeq = DOTween.Sequence();
            foreach (Piece p in fullPieceList)
            {
                ISpecialPiece special = p.GetComponent<ISpecialPiece>();
                if (special != null)
                {
                    specialSeq.Join(special.SpecialMoveAnimation());
                }

                regularSeq.Join(p.Despawn());
            }

            foreach (Piece p in piecesToDespawn)
            {
                spawnPoints.Add(new Vector3(p.transform.position.x, p.transform.position.y + 5f, p.transform.position.z));
            }

            seq.Append(specialSeq);
            seq.Append(regularSeq);

            seq.OnComplete(() => 
            {
                UpdateScore(spawnPoints.Count, specialPiecesSpawnPoints.Count);
                gameManager.SetState(new SpawnState(gameManager, spawnPoints, specialPiecesSpawnPoints)); 
            });
        }

        #region Own methods

        List<Pattern> _patterns;

        private void UpdateScore(int regulars, int specials)
        {
            int score = regulars * 50 + specials * 75;
            gameManager.AddScore(score);
        }
        private Piece SwappedPieceInPattern(List<Piece> pieces)
        {
            if (gameManager._lastSwappedPieces == null)
            {
                return null;
            }

            if (gameManager._lastSwappedPieces[0] == null ||
                gameManager._lastSwappedPieces[1] == null)
            {
                return null;
            }

            foreach(Piece p in pieces)
            {
                if (p.transform.position == gameManager._lastSwappedPieces[0].transform.position ||
                    p.transform.position == gameManager._lastSwappedPieces[1].transform.position)
                {
                    return p;
                }
            }

            return null;
        }
        private Piece GetSpecialPiecePosition(List<Piece> pieces)
        {
            Piece swappedPiece = SwappedPieceInPattern(pieces);

            if (swappedPiece == null)
            {
                return pieces[Random.Range(0, pieces.Count)];
            }

            return swappedPiece;
        }

        // This method is fairly complicated, so I gotta make a comment.
        // This is a recursive method, it takes in an initial list of pieces, 
        // an index where to start looping through that list and a limit variable,
        // just in case.
        // First, it goes through all pieces in a parameter list, starting from a specified index.
        // (upon first call, it starts looping throught the entire parameter list)
        // Then, it checks each piece, whether it is special or not. If not, continue.
        // If yes, get a list of all pieces that are destroyed by the special move,
        // then loop through all of them, checking if they already exist in the entire parameter list.
        // (to avoid duplicates)
        // Finally, if there were no pieces added (meaning that no special pieces were affected by special moves),
        // end the recursion returning the final list.
        // If some pieces were added, then we need to check if any of them are special, hence the recursion.
        // The new start index is the end of the parameter list. Then new pieces are added to the parameter list.
        // This way, upon the new recurion call, it will start checking from the new pieces, avoiding infinite calls.
        private List<Piece> GetPiecesFromSpecialMove(List<Piece> initialPieces, int startIndex, int recursionLimit)
        {
            if (recursionLimit > 100)
            {
                Debug.Log("Recursion limit reached!");
                return initialPieces;
            }

            List<Piece> additionalPieces = new List<Piece>();

            for (int i = startIndex; i < initialPieces.Count; i++)
            {
                ISpecialPiece special = initialPieces[i].GetComponent<ISpecialPiece>();
                if (special == null) continue;

                List<Piece> piecesFromSpecialMove = special.SpecialMovePieces();
                foreach(Piece p in piecesFromSpecialMove)
                {
                    if (initialPieces.Contains(p)) { continue; }
                    additionalPieces.Add(p);
                }
            }

            if (additionalPieces.Count < 1)
            {
                return initialPieces;
            }

            startIndex = initialPieces.Count;
            initialPieces.AddRange(additionalPieces);

            recursionLimit++;
            return GetPiecesFromSpecialMove(initialPieces, startIndex, recursionLimit);
        }

        #endregion
    }
}
