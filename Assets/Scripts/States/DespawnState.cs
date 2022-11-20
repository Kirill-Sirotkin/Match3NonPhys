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
            List<Piece> piecesToDespawn = new List<Piece>();

            foreach(Pattern pat in _patterns)
            {
                piecesToDespawn.AddRange(pat._piecesInPattern);
            }
            foreach(Piece p in piecesToDespawn)
            {
                spawnPoints.Add(new Vector3(p.transform.position.x, p.transform.position.y + 5f, p.transform.position.z));
                seq.Join(p.Despawn());
            }

            seq.OnComplete(() => { CleanUp(); gameManager.SetState(new SpawnState(gameManager, spawnPoints)); });
        }

        #region Own methods

        List<Pattern> _patterns;

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
