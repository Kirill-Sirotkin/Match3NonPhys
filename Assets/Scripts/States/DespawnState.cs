using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class DespawnState : State
    {
        public DespawnState(GameManager manager, Dictionary<Piece, List<Piece>> piecesToDespawn, List<Dictionary<Piece, List<Piece>>> separatePiecePatterns) : base(manager)
        {
            _piecesToDespawn = piecesToDespawn;
            _separatePiecePatterns = separatePiecePatterns;
        }

        public override void StartAction()
        {
            Sequence seq = DOTween.Sequence();
            List<Vector3> spawnPoints = new List<Vector3>();

            foreach (Dictionary<Piece, List<Piece>> dic in _separatePiecePatterns)
            {
                _patterns.Add(new Pattern(dic));
            }

            foreach (Pattern pat in _patterns)
            {
                Debug.Log("Uniqes in pat: " + pat._uniquePatternPieces.Count);

                Debug.Log("------------");
                foreach (Piece p in pat._uniquePatternPieces)
                {
                    Debug.Log("p: " + p.transform.position);
                }
                Debug.Log("------------");

                Debug.Log("Individual matches in pat: " + pat._patternPiecesMatches.Count);
                foreach (KeyValuePair<Piece, List<Piece>> pair in pat._patternPiecesMatches)
                {
                    Debug.Log("matches: " + (pair.Value.Count + 1));
                    Debug.Log("main: " + pair.Key.transform.position);
                    foreach (Piece p in pair.Value)
                    {
                        Debug.Log("coords: " + p.transform.position);
                    }
                }
            }

            for (int i = 0; i < _patterns.Count; i++)
            {
                if (i + 1 >= _patterns.Count) { break; }

                Debug.Log(Pattern.PatternsOverlap(_patterns[i], _patterns[i + 1]));
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
        List<Dictionary<Piece, List<Piece>>> _separatePiecePatterns;
        List<Pattern> _patterns = new List<Pattern>();

        private void CleanUp()
        {
            foreach (Transform child in gameManager._piecesParent)
            {
                if (child.gameObject.activeSelf) { continue; }
                Object.Destroy(child.gameObject);
            }
        }

        #endregion
    }
}
