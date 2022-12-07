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
                if (_specialPieceSpawnPoints != null)
                {
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
            GameObject obj = 
                Object.Instantiate(gameManager._prefabsManager.GetPiecePrefab(PieceSpecialType.Regular), 
                pos, Quaternion.identity, parent);

            return obj;
        }
        private GameObject SpawnPiece(Vector3 pos, Transform parent, char pieceType)
        {
            PieceType colorType;
            switch (pieceType)
            {
                case 'r':
                    colorType = PieceType.Red;
                    break;
                case 'b':
                    colorType = PieceType.Blue;
                    break;
                case 'y':
                    colorType = PieceType.Yellow;
                    break;
                case 'p':
                    colorType = PieceType.Purple;
                    break;
                case 'g':
                    colorType = PieceType.Green;
                    break;
                default:
                    colorType = PieceType.Red;
                    Debug.Log("Unknown piece signature in seed. Spawning default piece");
                    break;
            }

            GameObject obj = 
                Object.Instantiate(gameManager._prefabsManager.GetPiecePrefab(PieceSpecialType.Regular, colorType), 
                pos, Quaternion.identity, parent);
            return obj;
        }
        private GameObject SpawnPiece(Vector3 pos, Transform parent, PieceType pieceType, int specialType)
        {
            switch (pieceType)
            {
                case PieceType.Red:
                    break;
                case PieceType.Blue:
                    break;
                case PieceType.Yellow:
                    break;
                case PieceType.Purple:
                    break;
                case PieceType.Green:
                    break;
                default:
                    Debug.Log("Unknown piece type. Spawning default piece");
                    break;
            }

            PieceSpecialType pieceSpecialType;
            if (specialType >= 5)
            {
                pieceSpecialType = PieceSpecialType.Lightning;
            }
            else
            {
                pieceSpecialType = PieceSpecialType.Bomb;
            }

            GameObject obj = 
                Object.Instantiate(gameManager._prefabsManager.GetPiecePrefab(pieceSpecialType, pieceType), 
                pos, Quaternion.identity, parent);
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
