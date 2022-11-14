using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class SpawnState : State
    {
        public SpawnState(GameManager manager, List<Vector3> spawnPoints, string seed = null, Dictionary<Piece, int> specials = null) : base(manager)
        {
            _spawnPoints = new List<Vector3>(spawnPoints);
            if (specials != null)
            {
                _specials = new Dictionary<Piece, int>(specials);
            }
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

                if (_specials != null && _specials.Count > 0)
                {
                    foreach (KeyValuePair<Piece, int> pair in _specials)
                    {
                        SpawnSpecialPiece(pair.Key.transform.position, gameManager._piecesParent, pair);
                    }
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
        private Dictionary<Piece, int> _specials;
        private string _seed;

        private GameObject SpawnPiece(Vector3 pos, Transform parent)
        {
            int index = Random.Range(0, gameManager._regularPieces.GetLength(0));
            GameObject obj = Object.Instantiate(gameManager._regularPieces[index], pos, Quaternion.identity, parent);
            obj.GetComponent<Piece>().SetGameManager(gameManager);

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

            GameObject obj = Object.Instantiate(gameManager._regularPieces[index], pos, Quaternion.identity, parent);
            obj.GetComponent<Piece>().SetGameManager(gameManager);
            return obj;
        }
        private GameObject SpawnSpecialPiece(Vector3 pos, Transform parent, KeyValuePair<Piece, int> pair)
        {
            int array;
            int index;
            switch (pair.Key._type)
            {
                case PieceType.Red:
                    index = 0;
                    if (pair.Value == 4) { array = 0; }
                    else { array = 1; }
                    break;
                case PieceType.Blue:
                    index = 1;
                    if (pair.Value == 4) { array = 0; }
                    else { array = 1; }
                    break;
                case PieceType.Yellow:
                    index = 2;
                    if (pair.Value == 4) { array = 0; }
                    else { array = 1; }
                    break;
                case PieceType.Purple:
                    index = 3;
                    if (pair.Value == 4) { array = 0; }
                    else { array = 1; }
                    break;
                case PieceType.Green:
                    index = 4;
                    if (pair.Value == 4) { array = 0; }
                    else { array = 1; }
                    break;
                default:
                    index = 0;
                    array = 0;
                    Debug.Log("Uknown special piece. Spawning default special piece");
                    break;
            }

            GameObject obj;
            if (array == 1)
            {
                obj = Object.Instantiate(gameManager._bombPieces[index], pos, Quaternion.identity, parent);
            }
            else
            {
                obj = Object.Instantiate(gameManager._lightningPieces[index], pos, Quaternion.identity, parent);
            }
            obj.GetComponent<Piece>().SetGameManager(gameManager);
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
