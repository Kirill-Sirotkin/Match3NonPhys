using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class ShuffleState : State
    {
        public ShuffleState(GameManager manager) : base(manager)
        {
        }

        public override void StartAction()
        {

        }

        #region Own methods

        private Vector3[] _pattern1 = new Vector3[] { new Vector3(1f, -1f, 0f), new Vector3(2f, 0f, 0f) };
        private Vector3[] _pattern2 = new Vector3[] { new Vector3(1f, 0f, 0f), new Vector3(3f, 0f, 0f) };
        private Vector3[] _pattern3 = new Vector3[] { new Vector3(1f, 0f, 0f), new Vector3(2f, 1f, 0f) };
        private Vector3[] _pattern4 = new Vector3[] { new Vector3(1f, 0f, 0f), new Vector3(2f, -1f, 0f) };



        #endregion
    }
}
