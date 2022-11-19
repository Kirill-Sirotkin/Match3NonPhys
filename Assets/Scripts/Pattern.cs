using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class Pattern
    {
        public List<Piece> _uniquePatternPieces { get; private set; }
        public Dictionary<Piece, List<Piece>> _patternPiecesMatches { get; private set; }

        public Pattern(Dictionary<Piece, List<Piece>> pattern)
        {
            _patternPiecesMatches = pattern;
            _uniquePatternPieces = new List<Piece>();

            foreach (KeyValuePair<Piece, List<Piece>> pair in _patternPiecesMatches)
            {
                List<Piece> pieces = new List<Piece>();
                pieces.Add(pair.Key);
                pieces.AddRange(pair.Value);

                foreach (Piece p in pieces)
                {
                    if (_uniquePatternPieces.Contains(p)) { continue; }
                    _uniquePatternPieces.Add(p);
                }
            }
        }

        public static bool PatternsOverlap(Pattern pattern1, Pattern pattern2)
        {
            for (int i = 0; i < pattern1._uniquePatternPieces.Count; i++)
            {
                for (int j = 0; j < pattern2._uniquePatternPieces.Count; j++)
                {
                    if (pattern1._uniquePatternPieces[i] == pattern2._uniquePatternPieces[j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}