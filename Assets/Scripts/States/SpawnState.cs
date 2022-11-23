using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class SpawnState : State
    {
        public SpawnState(GameManager manager, List<Vector3> spawnPoints, Dictionary<Piece, int> specialSpawns = null, string seed = null) : base(manager)
        {
            _spawnPoints = new List<Vector3>(spawnPoints);
            _specialPieceSpawnPoints = specialSpawns;
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
                foreach (KeyValuePair<Piece, int> pair in _specialPieceSpawnPoints)
                {
                    GameObject g = SpawnPiece(pair.Key.transform.position, gameManager._piecesParent, pair.Key._type, pair.Value);
                    ISpecialPiece specialPiece = g.GetComponent<ISpecialPiece>();

                    if (specialPiece == null)
                    {
                        Debug.Log("Special Piece has no interface");
                        continue;
                    }

                    specialPiece.SetGameManager(gameManager);
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

            if (gameManager._lastSwappedPieces != null)
            {
                gameManager._lastSwappedPieces[0] = null;
                gameManager._lastSwappedPieces[1] = null;
            }
            gameManager._patterns.Clear();

            seq.OnComplete(() => { CleanUp(); gameManager.SetState(new PatternState(gameManager)); });
        }

        #region Own methods

        private List<Vector3> _spawnPoints;
        private Dictionary<Piece, int> _specialPieceSpawnPoints;
        private string _seed;

        private void CleanUp()
        {
            foreach (Transform child in gameManager._piecesParent)
            {
                if (child.gameObject.activeSelf) { continue; }
                Object.Destroy(child.gameObject);
            }
        }
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
                    Debug.Log("Unknown piece signature in seed. Spawning default piece");
                    break;
            }

            GameObject obj = Object.Instantiate(gameManager._pieces[index], pos, Quaternion.identity, parent);
            return obj;
        }
        private GameObject SpawnPiece(Vector3 pos, Transform parent, PieceType pieceType, int specialType)
        {
            int pieceTypeIndex;
            switch (pieceType)
            {
                case PieceType.Red:
                    pieceTypeIndex = 0;
                    break;
                case PieceType.Blue:
                    pieceTypeIndex = 1;
                    break;
                case PieceType.Yellow:
                    pieceTypeIndex = 2;
                    break;
                case PieceType.Purple:
                    pieceTypeIndex = 3;
                    break;
                case PieceType.Green:
                    pieceTypeIndex = 4;
                    break;
                default:
                    pieceTypeIndex = 0;
                    Debug.Log("Unknown piece type. Spawning default piece");
                    break;
            }

            int specialTypeIndex;
            if (specialType >= 5)
            {
                specialTypeIndex = 1;
            }
            else
            {
                specialTypeIndex = 0;
            }

            GameObject obj = Object.Instantiate(gameManager._specialPieces[specialTypeIndex, pieceTypeIndex], pos, Quaternion.identity, parent);
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
