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

            foreach(Piece p in piecesToDespawn)
            {
                spawnPoints.Add(new Vector3(p.transform.position.x, p.transform.position.y + 5f, p.transform.position.z));
                seq.Join(p.Despawn());
            }
            foreach(Piece p in specialPiecesSpawnPoints.Keys)
            {
                seq.Join(p.Despawn());
            }

            seq.OnComplete(() => { gameManager.SetState(new SpawnState(gameManager, spawnPoints, specialPiecesSpawnPoints)); });
        }

        #region Own methods

        List<Pattern> _patterns;

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

        #endregion
    }
}
