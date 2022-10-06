using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public abstract class State
    {
        protected GameManager gameManager;
        public State(GameManager manager)
        {
            gameManager = manager;
        }

        public virtual void StartAction() { }
    }
}
