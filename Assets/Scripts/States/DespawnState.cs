using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class DespawnState : State
    {
        public DespawnState(GameManager manager, Dictionary<Piece, List<Piece>> piecesToDespawn) : base(manager)
        {
            _piecesToDespawn = piecesToDespawn;
        }

        public override void StartAction()
        {
            Sequence seqRegular = DOTween.Sequence();
            Sequence seqSpecial = DOTween.Sequence();
            Dictionary<Piece, int> specialPieces = new Dictionary<Piece, int>();

            List<Piece> regularPieces = new List<Piece>();
            List<Vector3> spawnPoints = new List<Vector3>();

            foreach(KeyValuePair<Piece, List<Piece>> pair in _piecesToDespawn)
            {
                if (specialPieces.ContainsKey(pair.Key)) { Debug.Log("already in specials"); continue; }
                if (regularPieces.Contains(pair.Key)) { Debug.Log("already in regulars"); continue; }


                int maxCount = MaxMatchCount(pair, out Piece maxCountPiece);
                bool sameCount = IsSameMatchCount(pair);

                List<Piece> keyValueList = new List<Piece>();
                keyValueList.Add(maxCountPiece);
                keyValueList.AddRange(_piecesToDespawn[maxCountPiece]);
                Debug.Log("key value list length: " + keyValueList.Count);

                Piece swappedPiece = SwappedPiece(keyValueList);

                Debug.Log(keyValueList.Contains(maxCountPiece));

                if (maxCount < 3)
                {
                    regularPieces.AddRange(keyValueList);
                    continue;
                }

                if (swappedPiece != null)
                {
                    specialPieces.Add(swappedPiece, maxCount);
                    keyValueList.Remove(swappedPiece);
                    regularPieces.AddRange(keyValueList);
                    continue;
                }

                if (!sameCount)
                {
                    specialPieces.Add(maxCountPiece, maxCount);
                    keyValueList.Remove(maxCountPiece);
                    regularPieces.AddRange(keyValueList);
                    continue;
                }

                Piece randomPiece = keyValueList[Random.Range(0, keyValueList.Count)];
                specialPieces.Add(randomPiece, maxCount);
                keyValueList.Remove(randomPiece);
                regularPieces.AddRange(keyValueList);

                // Special pieces animation first, then all other despawns
                //spawnPoints.Add(new Vector3(pair.Key.transform.position.x, pair.Key.transform.position.y + 5f, pair.Key.transform.position.z));
                //seqRegular.Join(pair.Key.Despawn());
            }

            List<Piece> allDespawnPieces = new List<Piece>();
            allDespawnPieces.AddRange(regularPieces);
            allDespawnPieces.AddRange(specialPieces.Keys);
            allDespawnPieces = new List<Piece>(AreAllPiecesSet(allDespawnPieces));

            Debug.Log("all despawns: " + allDespawnPieces.Count);

            foreach (Piece p in regularPieces)
            {
                spawnPoints.Add(new Vector3(
                    p.transform.position.x,
                    p.transform.position.y + 5f,
                    p.transform.position.z));
                seqRegular.Join(p.Despawn());
            }
            foreach (Piece p in specialPieces.Keys)
            {
                seqRegular.Join(p.Despawn());
            }
            Debug.Log("number of specials: " + specialPieces.Count + ", regulars: " + spawnPoints.Count);


            seqRegular.OnComplete(() => { CleanUp(); gameManager.SetState(new SpawnState(gameManager, spawnPoints, null, specialPieces)); });
        }

        #region Own methods

        Dictionary<Piece, List<Piece>> _piecesToDespawn;

        private void CleanUp()
        {
            foreach (Transform child in gameManager._piecesParent)
            {
                if (child.gameObject.activeSelf) { continue; }
                Object.Destroy(child.gameObject);
            }
        }
        private int MaxMatchCount(KeyValuePair<Piece, List<Piece>> pair, out Piece piece)
        {
            int maxCount = pair.Value.Count;
            piece = pair.Key;

            foreach (Piece p in pair.Value)
            {
                if (_piecesToDespawn[p].Count > maxCount)
                {
                    maxCount = _piecesToDespawn[p].Count;
                    piece = p;
                }
            }

            return maxCount;
        }
        private bool IsSameMatchCount(KeyValuePair<Piece, List<Piece>> pair)
        {
            int count = pair.Value.Count;

            foreach (Piece p in pair.Value)
            {
                if (_piecesToDespawn[p].Count != count)
                {
                    return false;
                }
            }

            return true;
        }
        private Piece SwappedPiece(List<Piece> pieces)
        {
            foreach(Piece p in pieces)
            {
                if (p == gameManager._lastSwappedPieces[0] ||
                    p == gameManager._lastSwappedPieces[1])
                {
                    return p;
                }
            }

            return null;
        }
        private List<Piece> AreAllPiecesSet(List<Piece> allPiecesToDespawn)
        {
            List<Piece> newPieces = new List<Piece>();
            List<Piece> piecesList = new List<Piece>(allPiecesToDespawn);
            foreach (Piece p in allPiecesToDespawn)
            {
                BombPiece bomb = p.gameObject.GetComponent<BombPiece>();
                LightningPiece lightning = p.gameObject.GetComponent<LightningPiece>();
                if (bomb != null)
                {
                    newPieces.AddRange(bomb.SpecialMove());
                    continue;
                }
                if (lightning != null)
                {
                    newPieces.AddRange(lightning.SpecialMove());
                    continue;
                }
            }

            if (newPieces.Count > 0)
            {
                piecesList.AddRange(newPieces);
                piecesList = new List<Piece>(AreAllPiecesSet(piecesList));
            }

            return piecesList;
        }

        #endregion
    }
}
