using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        private Sequence MovePiecesDown()
        {
            Sequence seq = DOTween.Sequence();

            for (int i = -3; i < 4; i++)
            {
                for (int j = -4; j < 1; j++)
                {
                    // make ray
                    // move to lowest position
                }
            }

            return seq;
        }
    }
}
