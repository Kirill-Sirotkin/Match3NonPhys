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
            _differentiatedPatterns = new List<List<Piece>>();
        }

        public override void StartAction()
        {
            Sequence seq = DOTween.Sequence();
            List<Vector3> spawnPoints = new List<Vector3>();

            DifferentiatePatterns();
            Debug.Log("UNIQUE PATTERNS COUNT: " + _differentiatedPatterns.Count);

            foreach (List<Piece> uniquePattern in _differentiatedPatterns)
            {
                Debug.Log("PATTERN");
                foreach (Piece p in uniquePattern)
                {
                    Debug.Log("coords: " + p.transform.position);
                }

                if (uniquePattern.Count > 3)
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
        List<List<Piece>> _differentiatedPatterns;

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
            Dictionary<Piece, List<Piece>> uniquePatterns = new Dictionary<Piece, List<Piece>>();

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

                uniquePatterns.Add(mostMatchedPiece, _piecesToDespawn[mostMatchedPiece]);
            }

            foreach (KeyValuePair<Piece, List<Piece>> pair in uniquePatterns)
            {
                List<Piece> patternList = new List<Piece>();
                patternList.Add(pair.Key);
                patternList.AddRange(pair.Value);

                _differentiatedPatterns.Add(patternList);
            }
        }

        #endregion
    }
}
