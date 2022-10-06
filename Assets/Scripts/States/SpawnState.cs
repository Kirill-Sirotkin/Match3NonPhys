using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class SpawnState : State
    {
        private List<Vector3> _spawnPoints;

        public SpawnState(GameManager manager, List<Vector3> spawnPoints) : base(manager)
        {
            _spawnPoints = new List<Vector3>(spawnPoints);
        }

        public override void StartAction()
        {
            foreach (Vector3 v in _spawnPoints)
            {
                SpawnPiece(v, gameManager._piecesParent);
            }

            gameManager.SetState(new PatternState(gameManager));
        }

        public GameObject SpawnPiece(Vector3 pos, Transform parent)
        {
            int index = Random.Range(0, gameManager._pieces.GetLength(0));
            GameObject obj = Object.Instantiate(gameManager._pieces[index], pos, Quaternion.identity, parent);

            return obj;
        }
    }
}
