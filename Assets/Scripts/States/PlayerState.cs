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
        public override void SwitchState(int stateIndex)
        {
            gameManager._takeInput = false;

            switch (stateIndex)
            {
                case 0:
                    gameManager.SetState(new PatternState(gameManager, gameManager._lastSwappedPieces));
                    break;
                case 1:
                    gameManager.SetState(new DespawnState(gameManager, gameManager._patterns));
                    break;
                default:
                    Debug.Log("Unknown state index");
                    break;
            }
        }
    }
}
