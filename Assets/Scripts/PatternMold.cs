using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class PatternMold : MonoBehaviour
    {
        public List<List<Vector3>> _molds { get; private set; }
        private List<Vector3> _patternCoords;

        public PatternMold(List<Vector3> patternCoordinates)
        {
            _patternCoords = patternCoordinates;

            _molds = GeneratePatternMolds(_patternCoords);
        }
        public PatternMold(Vector3 patternCoordinate)
        {
            _patternCoords = new List<Vector3>();
            _patternCoords.Add(patternCoordinate);

            _molds = GeneratePatternMolds(_patternCoords);
        }
        public PatternMold(Vector3[] patternCoordinates)
        {
            _patternCoords = new List<Vector3>();
            foreach(Vector3 coordinate in patternCoordinates)
            {
                _patternCoords.Add(coordinate);
            }

            _molds = GeneratePatternMolds(_patternCoords);
        }

        private List<List<Vector3>> GeneratePatternMolds(List<Vector3> originalMold)
        {
            List<List<Vector3>> patternMolds = new List<List<Vector3>>();

            List<Vector3> mirroredOriginalMold = MirrorPattern(originalMold);
            for (int i = 0; i < 4; i++)
            {
                patternMolds.Add(RotatePattern(originalMold, i));
                patternMolds.Add(RotatePattern(mirroredOriginalMold, i));
            }

            return patternMolds;
        }

        private List<Vector3> MirrorPattern(List<Vector3> patternToMirror)
        {
            return MirrorPattern(patternToMirror, true, false);
        }
        private List<Vector3> MirrorPattern(List<Vector3> patternToMirror, bool xCoordinateMirror, bool yCoordinateMirror)
        {
            List<Vector3> mirroredPattern = new List<Vector3>();
            int xMultiplier = xCoordinateMirror ? -1 : 1;
            int yMultiplier = yCoordinateMirror ? -1 : 1;

            foreach(Vector3 coordInPattern in patternToMirror)
            {
                mirroredPattern.Add(new Vector3(
                    coordInPattern.x * xMultiplier,
                    coordInPattern.y * yMultiplier,
                    coordInPattern.z));
            }

            return mirroredPattern;
        }

        private List<Vector3> RotatePattern(List<Vector3> patternToRotate, int numberOfRotations)
        {
            List<Vector3> rotatedPattern = new List<Vector3>();

            foreach (Vector3 coordInPattern in patternToRotate)
            {
                Vector3 rotatedCoordinate = new Vector3(coordInPattern.x, coordInPattern.y, coordInPattern.z);

                for (int i = 0; i < numberOfRotations; i++)
                {
                    rotatedCoordinate = new Vector3(rotatedCoordinate.y, rotatedCoordinate.x * -1, rotatedCoordinate.z);
                }

                rotatedPattern.Add(rotatedCoordinate);
            }

            return rotatedPattern;
        }
    }
}
