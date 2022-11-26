using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class PatternMold : MonoBehaviour
    {
        public List<Vector3> _pieceCoords { get; private set; }

        public PatternMold(List<Vector3> pieceCoordinates)
        {
            _pieceCoords = pieceCoordinates;
        }
        public PatternMold(Vector3 pieceCoordinate)
        {
            _pieceCoords = new List<Vector3>();
            _pieceCoords.Add(pieceCoordinate);
        }

        private List<Vector3> MirrorPattern(List<Vector3> patternToMirror)
        {
            return new List<Vector3>();
        }
    }
}
