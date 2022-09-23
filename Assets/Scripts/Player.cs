using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] private GameManager _manager;

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
            _manager.PassSelection(hit);
        }
    }

}