using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class DespawnState : State
    {
        public DespawnState(GameManager manager, Dictionary<Piece, List<Piece>> piecesToDespawn) : base(manager)
        {
            _piecesToDespawn = piecesToDespawn;
            _differentiatedPatterns = new Dictionary<Piece, List<Piece>>();
        }

        public override void StartAction()
        {
            Sequence seq = DOTween.Sequence();
            List<Vector3> spawnPoints = new List<Vector3>();

            DifferentiatePatterns();

            foreach (KeyValuePair<Piece, List<Piece>> pair in _differentiatedPatterns)
            {
                List<Piece> patternPiecesList = new List<Piece>();
                patternPiecesList.Add(pair.Key);
                patternPiecesList.AddRange(pair.Value);

                Debug.Log("most match for: " + pair.Key.transform.position + "; matches: " + pair.Value.Count);
                foreach (Piece p in pair.Value)
                {
                    Debug.Log(p.transform.position);
                }

                if (pair.Value.Count > 2)
                {
                    Debug.Log("SPECIAL PATTERN");
                }


                Debug.Log("---------------");
            }

            foreach (Piece p in _piecesToDespawn.Keys)
            {
                spawnPoints.Add(new Vector3(p.transform.position.x, p.transform.position.y + 5f, p.transform.position.z));
                seq.Join(p.Despawn());
            }

            seq.OnComplete(() => { CleanUp(); gameManager.SetState(new SpawnState(gameManager, spawnPoints)); });
        }

        #region Own methods

        Dictionary<Piece, List<Piece>> _piecesToDespawn;
        Dictionary<Piece, List<Piece>> _differentiatedPatterns;

        private void CleanUp()
        {
            foreach (Transform child in gameManager._piecesParent)
            {
                if (child.gameObject.activeSelf) { continue; }
                Object.Destroy(child.gameObject);
            }
        }

        private void DifferentiatePatterns()
        {
            List<Piece> addedPieces = new List<Piece>();

            foreach (KeyValuePair<Piece, List<Piece>> pair in _piecesToDespawn)
            {
                if (addedPieces.Contains(pair.Key)) { continue; }

                int baseMatchesCount = pair.Value.Count;
                Piece mostMatchedPiece = pair.Key;

                foreach (Piece p in pair.Value)
                {
                    int matchesCount = _piecesToDespawn[p].Count;

                    if (matchesCount > baseMatchesCount)
                    {
                        baseMatchesCount = matchesCount;
                        mostMatchedPiece = p;
                    }
                }

                addedPieces.Add(mostMatchedPiece);
                addedPieces.AddRange(_piecesToDespawn[mostMatchedPiece]);

                _differentiatedPatterns.Add(mostMatchedPiece, _piecesToDespawn[mostMatchedPiece]);
            }
        }

        #endregion
    }
}
