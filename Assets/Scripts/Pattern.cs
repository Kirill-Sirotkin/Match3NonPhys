using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class Pattern
    {
        public List<Piece> _piecesInPattern { get; private set; }

        #region Constructors
        public Pattern(List<Piece> pattern)
        {
            _piecesInPattern = pattern;
        }
        public Pattern(KeyValuePair<Piece,List<Piece>> pattern)
        {
            _piecesInPattern = new List<Piece>();
            _piecesInPattern.Add(pattern.Key);
            _piecesInPattern.AddRange(pattern.Value);
        }
        #endregion

        public static List<Pattern> MergePatternsList(List<Pattern> patterns, int backupIterationCount)
        {
            bool isOverlapAnywhere = false;
            List<Pattern> overlappingPatterns = new List<Pattern>();

            for (int i = 0; i < patterns.Count - 1; i++)
            {
                //if (i + 1 >= patterns.Count) { break; }

                for (int j = i + 1; j < patterns.Count; j++)
                {
                    if (IsPatternOverlap(patterns[i], patterns[j]))
                    {
                        isOverlapAnywhere = true;
                        overlappingPatterns.Add(patterns[i]);
                        overlappingPatterns.Add(patterns[j]);
                        break;
                    }
                }
            }

            if (!isOverlapAnywhere) { return patterns; }

            List<Pattern> returnList = patterns;
            returnList.Remove(overlappingPatterns[0]);
            returnList.Remove(overlappingPatterns[1]);
            returnList.AddRange(MergePatterns(overlappingPatterns[0], overlappingPatterns[1]));

            backupIterationCount++;
            if (backupIterationCount > 100) { Debug.Log("recursion limit reached"); return returnList; }

            return MergePatternsList(returnList, backupIterationCount);
        }
        public static List<Pattern> MergePatterns(Pattern pattern1, Pattern pattern2)
        {
            List<Pattern> returnList = new List<Pattern>();

            if (!IsPatternOverlap(pattern1, pattern2)) 
            {
                returnList.Add(pattern1);
                returnList.Add(pattern2);
                return returnList;
            }

            Pattern mergedPattern = new Pattern(MergeNonOverlappingPieces(pattern1, pattern2));
            returnList.Add(mergedPattern);

            return returnList;
        }
        private static bool IsPatternOverlap(Pattern pattern1, Pattern pattern2)
        {
            for (int i = 0; i < pattern1._piecesInPattern.Count; i++)
            {
                for (int j = 0; j < pattern2._piecesInPattern.Count; j++)
                {
                    if (Vector3.Distance(
                        pattern1._piecesInPattern[i].transform.position, 
                        pattern2._piecesInPattern[j].transform.position) < 0.1f) 
                    { return true; }
                }
            }

            return false;
        }
        private static List<Piece> MergeNonOverlappingPieces(Pattern pattern1, Pattern pattern2)
        {
            List<Piece> nonOverlappingPieces = new List<Piece>();

            foreach (Piece p in pattern1._piecesInPattern)
            {
                if (nonOverlappingPieces.Contains(p)) { continue; }
                nonOverlappingPieces.Add(p);
            }
            foreach (Piece p in pattern2._piecesInPattern)
            {
                if (nonOverlappingPieces.Contains(p)) { continue; }
                nonOverlappingPieces.Add(p);
            }

            return nonOverlappingPieces;
        }
    }
}