using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class PiecePrefabsManager : MonoBehaviour
    {
        [field: SerializeField] private GameObject[] regularPieces;
        [field: SerializeField] private GameObject[] bombPieces;
        [field: SerializeField] private GameObject[] lightningPieces;

        private void Start()
        {
            if (regularPieces.GetLength(0) != 5) { Debug.Log("Regular pieces array wrong dimensions!"); }
            if (bombPieces.GetLength(0) != 5) { Debug.Log("Bomb pieces array wrong dimensions!"); }
            if (lightningPieces.GetLength(0) != 5) { Debug.Log("Lightning pieces array wrong dimensions!"); }
        }

        public GameObject GetPiecePrefab(PieceSpecialType specialType)
        {
            int pieceColorIndex;
            pieceColorIndex = Random.Range(0, 5);

            switch (pieceColorIndex)
            {
                case 0:
                    return GetPiecePrefab(specialType, PieceType.Red);
                case 1:
                    return GetPiecePrefab(specialType, PieceType.Blue);
                case 2:
                    return GetPiecePrefab(specialType, PieceType.Yellow);
                case 3:
                    return GetPiecePrefab(specialType, PieceType.Purple);
                case 4:
                    return GetPiecePrefab(specialType, PieceType.Green);
                default:
                    return GetPiecePrefab(specialType, PieceType.Red);
            }
        }
        public GameObject GetPiecePrefab(PieceSpecialType specialType, PieceType colorType)
        {
            int pieceColorIndex;

            switch (colorType)
            {
                case PieceType.Red:
                    pieceColorIndex = 0;
                    break;
                case PieceType.Blue:
                    pieceColorIndex = 1;
                    break;
                case PieceType.Yellow:
                    pieceColorIndex = 2;
                    break;
                case PieceType.Purple:
                    pieceColorIndex = 3;
                    break;
                case PieceType.Green:
                    pieceColorIndex = 4;
                    break;
                default:
                    pieceColorIndex = 0;
                    break;
            }

            switch (specialType)
            {
                case PieceSpecialType.Regular:
                    return regularPieces[pieceColorIndex];
                case PieceSpecialType.Bomb:
                    return bombPieces[pieceColorIndex];
                case PieceSpecialType.Lightning:
                    return lightningPieces[pieceColorIndex];
                default:
                    return regularPieces[pieceColorIndex];
            }
        }
    }
}