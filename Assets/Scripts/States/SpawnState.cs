using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class SpawnState : State
    {
        public SpawnState(GameManager manager, List<Vector3> spawnPoints, string seed = null) : base(manager)
        {
            _spawnPoints = new List<Vector3>(spawnPoints);
            _seed = seed;
        }

        public override void StartAction()
        {
            if (_seed == null)
            {
                foreach (Vector3 v in _spawnPoints)
                {
                    SpawnPiece(v, gameManager._piecesParent);
                }
            }
            else
            {
                for (int i = 0; i < _spawnPoints.Count; i++)
                {
                    SpawnPiece(_spawnPoints[i], gameManager._piecesParent, _seed[i]);
                }
            }

            Sequence seq = MovePiecesDown();
            seq.OnComplete(() => { gameManager.SetState(new PatternState(gameManager)); });
        }

        #region Own methods

        private List<Vector3> _spawnPoints;
        private string _seed;

        private GameObject SpawnPiece(Vector3 pos, Transform parent)
        {
            int index = Random.Range(0, gameManager._pieces.GetLength(0));
            GameObject obj = Object.Instantiate(gameManager._pieces[index], pos, Quaternion.identity, parent);

            return obj;
        }

        private GameObject SpawnPiece(Vector3 pos, Transform parent, char pieceType)
        {
            int index;
            switch (pieceType)
            {
                case 'r':
                    index = 0;
                    break;
                case 'b':
                    index = 1;
                    break;
                case 'y':
                    index = 2;
                    break;
                case 'p':
                    index = 3;
                    break;
                case 'g':
                    index = 4;
                    break;
                default:
                    index = 0;
                    Debug.Log("Uknown piece signature in seed. Spawning default piece");
                    break;
            }

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
