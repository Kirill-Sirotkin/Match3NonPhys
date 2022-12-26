using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class SpawnState : State
    {
        public SpawnState(GameManager manager, List<SpawnPoint> spawnPoints) : base(manager)
        {
            _spawnPoints = spawnPoints;
        }

        public override void StartAction()
        {
            foreach(SpawnPoint spawnPoint in _spawnPoints)
            {
                GameObject g = SpawnPiece(spawnPoint._coords, spawnPoint._colorType, spawnPoint._specialType, gameManager._piecesParent);
                ISpecialPiece specialPiece = g.GetComponent<ISpecialPiece>();

                if (specialPiece == null) { continue; }
                specialPiece.SetGameManager(gameManager);
            }

            Sequence seq = MovePiecesDown();
            ClearLastSwappedPieces();

            seq.OnComplete(() => { CleanUp(); gameManager.SetState(new PatternState(gameManager)); });
        }

        #region Own methods

        private List<SpawnPoint> _spawnPoints;

        private void CleanUp()
        {
            foreach (Transform child in gameManager._piecesParent)
            {
                if (child.gameObject.activeSelf) { continue; }
                Object.Destroy(child.gameObject);
            }
        }
        private GameObject SpawnPiece(Vector3 pos, PieceType colorType, PieceSpecialType specialType, Transform parent)
        {
            GameObject obj = 
                Object.Instantiate(gameManager._prefabsManager.GetPiecePrefab(specialType, colorType), 
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

            gameManager._soundManager.PlaySound("PiecesSlideDown");

            return seq;
        }
        private void ClearLastSwappedPieces()
        {
            if (gameManager._lastSwappedPieces != null)
            {
                gameManager._lastSwappedPieces[0] = null;
                gameManager._lastSwappedPieces[1] = null;
            }
            gameManager._patterns.Clear();
        }

        #endregion
    }
}
