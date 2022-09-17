using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class Piece : MonoBehaviour
    {
        [field: SerializeField] private float _moveTime;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(new Vector3(transform.position.x, transform.position.y - 3f, 0f));
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                transform.position = new Vector3(transform.position.x, 0f, 0f);
            }
        }

        public void Move(Vector3 pos)
        {
            transform.DOMove(pos, _moveTime).SetEase(Ease.OutBack);
        }
    }
}
