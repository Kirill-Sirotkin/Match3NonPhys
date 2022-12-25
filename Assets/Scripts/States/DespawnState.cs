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

            Sequence specialSeq = DOTween.Sequence();
            Sequence regularSeq = DOTween.Sequence();

            List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
            List<Piece> allPieces = Pattern.GetPiecesFromPatterns(_patterns);

            spawnPoints.AddRange(GetSpawnPointsFromInitialPatterns(_patterns));

            int initialAllPiecesCount = allPieces.Count;
            allPieces = GetPiecesFromSpecialMove(allPieces);

            AssignAnimations(allPieces, regularSeq, specialSeq);
            UpdateScores(allPieces);

            allPieces.RemoveRange(0, initialAllPiecesCount);
            spawnPoints.AddRange(GetSpawnPoints(allPieces));

            seq.Append(specialSeq);
            seq.Append(regularSeq);

            seq.OnComplete(() => { gameManager.SetState(new SpawnState(gameManager, spawnPoints)); });
        }

        #region Own methods

        List<Pattern> _patterns;

        private void UpdateScores(List<Piece> pieces)
        {
            int score = 0;

            foreach(Piece piece in pieces)
            {
                if (piece.GetComponent<ISpecialPiece>() != null)
                {
                    score += 50;
                }

                score += 50;
            }

            gameManager.AddScore(score);
        }
        private void AssignAnimations(List<Piece> pieces, Sequence regularSeq, Sequence specialSeq)
        {
            foreach(Piece piece in pieces)
            {
                ISpecialPiece specialInterface = piece.GetComponent<ISpecialPiece>();

                if (specialInterface != null)
                {
                    specialSeq.Join(specialInterface.SpecialMoveAnimation());
                }

                regularSeq.Join(piece.Despawn());
            }
        }
        private List<SpawnPoint> GetSpawnPoints(List<Piece> pieces)
        {
            List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

            foreach(Piece piece in pieces)
            {
                spawnPoints.Add(new SpawnPoint(new Vector3(
                    piece.transform.position.x,
                    piece.transform.position.y + 5,
                    piece.transform.position.z
                    )));
            }

            return spawnPoints;
        }
        private List<SpawnPoint> GetSpawnPointsFromInitialPatterns(List<Pattern> patterns)
        {
            List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

            foreach(Pattern pattern in patterns)
            {
                Piece specialPieceSpawn = GetSpecialPieceSpawn(pattern._piecesInPattern);

                switch (pattern._piecesInPattern.Count)
                {
                    case 4:
                        spawnPoints.Add(new SpawnPoint(specialPieceSpawn.transform.position, PieceSpecialType.Bomb, specialPieceSpawn._type));
                        pattern._piecesInPattern.Remove(specialPieceSpawn);
                        break;
                    case 5:
                        spawnPoints.Add(new SpawnPoint(specialPieceSpawn.transform.position, PieceSpecialType.Lightning, specialPieceSpawn._type));
                        pattern._piecesInPattern.Remove(specialPieceSpawn);
                        break;
                    default:
                        break;
                }
            }

            List<Piece> regularPieces = Pattern.GetPiecesFromPatterns(patterns);
            spawnPoints.AddRange(GetSpawnPoints(regularPieces));

            return spawnPoints;
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
        private Piece GetSpecialPieceSpawn(List<Piece> pieces)
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
        private List<Piece> GetPiecesFromSpecialMove(List<Piece> initialPieces, int startIndex = 0, int recursionLimit = 0)
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
