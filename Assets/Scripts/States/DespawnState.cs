using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class DespawnState : State
    {
        public DespawnState(GameManager manager, List<Pattern> patterns) : base(manager)
        {
            _patterns = patterns;
        }

        public override void StartAction()
        {
            Sequence seq = DOTween.Sequence();
            List<Vector3> spawnPoints = new List<Vector3>();
            List<Piece> piecesToDespawn = new List<Piece>();

            foreach(Pattern pat in _patterns)
            {
                piecesToDespawn.AddRange(pat._piecesInPattern);
                if (pat._piecesInPattern.Count > 3)
                {
                    Debug.Log("Special pattern: " + pat._piecesInPattern.Count);
                    Debug.Log("Position for piece: " + GetSpecialPiecePosition(pat._piecesInPattern));
                }
            }
            foreach(Piece p in piecesToDespawn)
            {
                spawnPoints.Add(new Vector3(p.transform.position.x, p.transform.position.y + 5f, p.transform.position.z));
                seq.Join(p.Despawn());
            }

            seq.OnComplete(() => { CleanUp(); gameManager.SetState(new SpawnState(gameManager, spawnPoints)); });
        }

        #region Own methods

        List<Pattern> _patterns;

        private void CleanUp()
        {
            foreach (Transform child in gameManager._piecesParent)
            {
                if (child.gameObject.activeSelf) { continue; }
                Object.Destroy(child.gameObject);
            }
        }
        private Piece SwappedPieceInPattern(List<Piece> pieces)
        {
            if (gameManager._lastSwappedPieces == null)
            {
                return null;
            }

            if (gameManager._lastSwappedPieces[0] == null ||
                gameManager._lastSwappedPieces[1] == null)
            {
                return null;
            }

            foreach(Piece p in pieces)
            {
                if (p.transform.position == gameManager._lastSwappedPieces[0].transform.position ||
                    p.transform.position == gameManager._lastSwappedPieces[1].transform.position)
                {
                    return p;
                }
            }

            return null;
        }
        private Vector3 GetSpecialPiecePosition(List<Piece> pieces)
        {
            Piece swappedPiece = SwappedPieceInPattern(pieces);

            if (swappedPiece == null)
            {
                return pieces[Random.Range(0, pieces.Count)].transform.position;
            }

            return swappedPiece.transform.position;
        }

        #endregion
    }
}
