using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class Player : MonoBehaviour
    {
        private Piece _selectedPiece;

        private List<Piece> _pieces = new List<Piece>();
        private bool done = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                MouseRay();
            }
            if(_pieces.Count >= 3)
            {
                if (!done)
                {
                    done = true;
                    foreach (Piece p in _pieces)
                    {
                        p.Spin();
                    }
                }
            }
        }

        private void MouseRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, 100)) { return; }
            IClickable clickable = hit.transform.GetComponent<IClickable>();

            if (clickable == null) { return; }
            clickable.ClickAction();

            Piece piece = hit.transform.GetComponent<Piece>();

            if (piece == null) { return; }
            _pieces.Add(piece);
        }
    }

}