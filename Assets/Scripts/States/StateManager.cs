using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public abstract class StateManager : MonoBehaviour
    {
        protected State _state;

        public void SetState(State state)
        {
            _state = state;
            _state.StartAction();
        }
    }
}
