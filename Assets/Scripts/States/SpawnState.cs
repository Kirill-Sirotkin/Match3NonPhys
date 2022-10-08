using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class SpawnState : State
    {
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

            Sequence seq = MovePiecesDown();
            seq.OnComplete(() => { gameManager.SetState(new PatternState(gameManager)); });
        }

        #region Own methods

        private List<Vector3> _spawnPoints;

        private GameObject SpawnPiece(Vector3 pos, Transform parent)
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
                Vector3 rayOrigin = new Vector3(i, -5f, 0f);

                for (int j = -4; j < 1; j++)
                {
                    Piece piece = gameManager.GetRayPiece(rayOrigin, Vector3.up);
                    rayOrigin = piece.transform.position;

                    Vector3 moveVector = new Vector3(i, j, 0f);
                    if (piece.transform.position == moveVector) { continue; }

                    seq.Join(piece.Move(moveVector));
                }
            }

            return seq;
        }

        #endregion
    }
}
