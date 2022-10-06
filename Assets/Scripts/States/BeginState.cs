using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class BeginState : State
    {
        public BeginState(GameManager manager) : base(manager)
        {
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

            gameManager.SetState(new SpawnState(gameManager, spawnPoints));
        }
    }
}
