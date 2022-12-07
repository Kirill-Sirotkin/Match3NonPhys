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
                case PieceType.Random:
                    pieceColorIndex = Random.Range(0, 5);
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