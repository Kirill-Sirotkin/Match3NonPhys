using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class Spawner : MonoBehaviour
    {
        [field: SerializeField] private GameManager _manager;
        [field: SerializeField] private GameObject _redPiece;
        [field: SerializeField] private GameObject _bluePiece;
        [field: SerializeField] private GameObject _yellowPiece;
        [field: SerializeField] private GameObject _purplePiece;
        [field: SerializeField] private GameObject _greenPiece;
        private GameObject[] _pieces;

        private void Awake()
        {
            _pieces = new GameObject[] { _redPiece, _bluePiece, _yellowPiece, _purplePiece, _greenPiece };
        }

        public GameObject SpawnPiece(Vector3 pos, Transform parent)
        {
            int index = Random.Range(0, _pieces.GetLength(0));
            GameObject obj = Instantiate(_pieces[index], pos, Quaternion.identity, parent);

            return obj;
        }
        public Sequence MovePiecesDown()
        {
            Vector3[] startVectors = new Vector3[] { 
                new Vector3(-3f, -4f, 0f),
                new Vector3(-2f, -4f, 0f),
                new Vector3(-1f, -4f, 0f),
                new Vector3(0f, -4f, 0f),
                new Vector3(1f, -4f, 0f),
                new Vector3(2f, -4f, 0f),
                new Vector3(3f, -4f, 0f)
            };
            Sequence seq = DOTween.Sequence();

            foreach (Vector3 v in startVectors)
            {
                List<Vector3> emptySpaces = new List<Vector3>();
                List<Piece> piecesToMove = new List<Piece>();

                for (int i = 0; i < 10; i++)
                {
                    Vector3 pos = v + new Vector3(0f, i, -1.05f);
                    Vector3 direction = Vector3.forward;
                    RaycastHit hit;

                    if (!Physics.Raycast(pos, direction, out hit, 2f)) 
                    { 
                        if (pos.y < 0.5f) { emptySpaces.Add(new Vector3(v.x, v.y + i, v.z)); }
                        continue; 
                    }
                    Piece piece = hit.transform.GetComponent<Piece>();
                    if (piece != null)
                    {
                        piecesToMove.Add(piece);

                        if (pos.y > 0.5f) { continue; }
                        emptySpaces.Add(new Vector3(v.x, v.y + i, v.z));
                    }
                }

                if (piecesToMove.Count != emptySpaces.Count) { Debug.Log("no equality on line: " + v.x + " pieces: " + piecesToMove.Count + " empty: " + emptySpaces.Count); }

                for (int i = 0; i < piecesToMove.Count; i++)
                {
                    seq.Join(piecesToMove[i].Move(emptySpaces[i]));
                }
            }
            seq.OnComplete(()=> { _manager.CheckForPatterns(); });

            return seq;
        }
    }
}