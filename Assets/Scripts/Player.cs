using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class Player : MonoBehaviour
    {
        private Piece _selectedPiece;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                MouseRay();
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
        }
    }

}