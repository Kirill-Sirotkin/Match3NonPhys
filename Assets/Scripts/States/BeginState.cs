using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class BeginState : State
    {
        public BeginState(GameManager manager, string seed = null) : base(manager)
        {
            _seed = seed;
        }

        public override void StartAction()
        {
            List<Vector3> spawnPoints = new List<Vector3>();

            for (int i = 0; i < 5; i++)
            {
                for (int j = -3; j <= 3; j++)
                {
                    Vector3 spawn = new Vector3(j, i, 0f);
                    spawnPoints.Add(spawn);
                }
            }

            if (!IsSeedCorrect()) { gameManager.SetState(new SpawnState(gameManager, spawnPoints)); return; }
            gameManager.SetState(new SpawnState(gameManager, spawnPoints, null, _seed));
        }

        #region Own methods

        private string _seed;

        private bool IsSeedCorrect()
        {
            if (_seed == null) { return false; }
            if (_seed.Length != 35) { return false; }
            return true;
        }

        #endregion
    }
}
