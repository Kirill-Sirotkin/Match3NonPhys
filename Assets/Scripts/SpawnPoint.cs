using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class SpawnPoint : MonoBehaviour
    {
        public Vector3 _coords { get; private set; }
        public PieceSpecialType _specialType { get; private set; }
        public PieceType _pieceType { get; private set; }

        public SpawnPoint(Vector3 coordinates)
        {
            _coords = coordinates;
            _specialType = PieceSpecialType.Regular;
            _pieceType = PieceType.Random;
        }
        public SpawnPoint(Vector3 coordinates, PieceSpecialType pieceSpecialType, PieceType pieceType)
        {
            _coords = coordinates;
            _specialType = pieceSpecialType;
            _pieceType = pieceType;
        }
    }
}
