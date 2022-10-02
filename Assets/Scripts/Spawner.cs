using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class Spawner : MonoBehaviour
    {
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
        public void MovePiecesDown()
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

            foreach (Vector3 v in startVectors)
            {
                Vector3 newStartVector = Vector3.zero;
                List<Vector3> emptySpaces = new List<Vector3>();
                bool newStartFound = false;

                for (int i = 0; i < 5; i++)
                {
                    Vector3 pos = v + new Vector3(0f, i, -1.05f);
                    Vector3 direction = Vector3.forward;
                    RaycastHit hit;

                    if (!Physics.Raycast(pos, direction, out hit, 2f)) 
                    {
                        if (!newStartFound) { newStartVector = new Vector3(v.x, i, v.z); newStartFound = true; }
                        emptySpaces.Add(new Vector3(v.x, i, v.z));
                    }
                }

                Debug.Log(newStartVector);
            }
        }
    }
}
