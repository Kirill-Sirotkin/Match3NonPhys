using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class Piece : MonoBehaviour, IClickable
    {
        [field: SerializeField] private GameObject _highlight;
        [field: SerializeField] private GameObject _visual;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //Move(new Vector3(transform.position.x, transform.position.y - 3f, 0f));
                Spin();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                transform.position = new Vector3(transform.position.x, 0f, 0f);
            }
        }

        public void Move(Vector3 pos)
        {
            transform.DOMove(pos, Random.Range(0.72f, 0.77f)).SetEase(Ease.OutBack);
        }
        public void Spin()
        {
            _visual.transform.DORotate(new Vector3(0f, 360f, 0f), 0.25f, RotateMode.FastBeyond360);
        }
        public void ToggleHighlight(bool b)
        {
            _highlight.SetActive(b);
        }
        public void ToggleHighlight()
        {
            _highlight.SetActive(!_highlight.activeSelf);
        }
        public void ClickAction()
        {
            ToggleHighlight();
        }
    }
}
