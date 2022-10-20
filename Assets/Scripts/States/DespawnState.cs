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

            foreach(KeyValuePair<Piece, List<Piece>> p in _piecesToDespawn)
            {
                spawnPoints.Add(new Vector3(p.Key.transform.position.x, p.Key.transform.position.y + 5f, p.Key.transform.position.z));
                Debug.Log("main: " + p.Key.transform.position + " matched: " + p.Value.Count);
                // Determine if pattern is 4+ pieces
                // Check in separate function this piece and all its matches for largest matched pattern

                // If one of the pieces matches last swapped, spawn special there
                // If not, spawn randomly
                seq.Join(p.Key.Despawn());
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
