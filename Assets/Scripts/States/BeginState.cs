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
            List<SpawnPoint> spawnPoints;

            if (IsSeedCorrect(_seed)) 
            { spawnPoints = GenerateInitialSpawnPoints(_seed); }
            else 
            { spawnPoints = GenerateInitialSpawnPoints(); }

            gameManager.SetState(new SpawnState(gameManager, spawnPoints));
        }

        #region Own methods

        private string _seed;

        private bool IsSeedCorrect(string seed)
        {
            if (seed == null) { return false; }
            if (seed.Length != 35) { return false; }
            return true;
        }
        private List<SpawnPoint> GenerateInitialSpawnPoints()
        {
            List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

            for (int i = 0; i < 5; i++)
            {
                for (int j = -3; j <= 3; j++)
                {
                    Vector3 spawnCoords = new Vector3(j, i, 0f);
                    spawnPoints.Add(new SpawnPoint(spawnCoords));
                }
            }

            return spawnPoints;
        }
        private List<SpawnPoint> GenerateInitialSpawnPoints(string seed)
        {
            List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
            int seedPieceIndex = 0;

            for (int i = 0; i < 5; i++)
            {
                for (int j = -3; j <= 3; j++)
                {
                    Vector3 spawnCoords = new Vector3(j, i, 0f);
                    PieceType colorType = ParseSeedPiece(seed[seedPieceIndex]);

                    seedPieceIndex++;
                    spawnPoints.Add(new SpawnPoint(spawnCoords, PieceSpecialType.Regular, colorType));
                }
            }

            return spawnPoints;
        }
        private PieceType ParseSeedPiece(char seedPiece)
        {
            switch (seedPiece)
            {
                case 'r':
                    return PieceType.Red;
                case 'b':
                    return PieceType.Blue;
                case 'y':
                    return PieceType.Yellow;
                case 'p':
                    return PieceType.Purple;
                case 'g':
                    return PieceType.Green;
                default:
                    Debug.Log("Unknown seed piece signature. Spawning default Red piece");
                    return PieceType.Red;
            }
        }

        #endregion
    }
}
