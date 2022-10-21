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
        }

        public override void StartAction()
        {
            Sequence seq = DOTween.Sequence();
            List<Vector3> spawnPoints = new List<Vector3>();

            foreach(KeyValuePair<Piece, List<Piece>> pair in _piecesToDespawn)
            {
                spawnPoints.Add(new Vector3(pair.Key.transform.position.x, pair.Key.transform.position.y + 5f, pair.Key.transform.position.z));
                

                // Series of checks:
                // is this piece maximum in it's pattern?
                // are all pieces same count?
                // what is the maximum count?
                // is this piece swapped?

                int maxCount = pair.Value.Count;
                bool isMaxCount = true;
                bool areAllSameCount = true;
                bool isPieceSwapped = false;

                if (pair.Key == gameManager._lastSwappedPieces[0] ||
                    pair.Key == gameManager._lastSwappedPieces[1])
                {
                    isPieceSwapped = true;
                }

                foreach (Piece p in pair.Value)
                {
                    if (_piecesToDespawn[p].Count != maxCount)
                    {
                        areAllSameCount = false;
                    }
                    if (_piecesToDespawn[p].Count > maxCount)
                    {
                        maxCount = _piecesToDespawn[p].Count;
                        isMaxCount = false;
                    }
                }

                if (isMaxCount && !areAllSameCount)
                {
                    // ?????? If one of the pieces matches last swapped, spawn special there
                    // ?????? If not, spawn randomly
                    Debug.Log("max count piece is at: " + pair.Key.transform.position + ", count: " + pair.Value.Count);
                }
                if (areAllSameCount) { Debug.Log("all the same"); }

                Debug.Log("main: " + pair.Key.transform.position + " matched: " + pair.Value.Count);



                seq.Join(pair.Key.Despawn());
            }

            seq.OnComplete(() => { CleanUp(); gameManager.SetState(new SpawnState(gameManager, spawnPoints)); });
        }

        #region Own methods

        Dictionary<Piece, List<Piece>> _piecesToDespawn;

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
