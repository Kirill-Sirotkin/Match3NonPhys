using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class PlayerState : State
    {
        public PlayerState(GameManager manager) : base(manager)
        {
        }

        public override void StartAction()
        {
            gameManager._takeInput = true;
        }
        public override void SwitchState()
        {
            gameManager._takeInput = false;
            gameManager.SetState(new PatternState(gameManager));
        }
    }
}
