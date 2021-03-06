using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeGardeners
{
    public abstract class HazardSpawner : MonoBehaviour
    {
        protected Vector3 spawnPosition;
        protected Vector3 despawnPosition;
        protected float hazardDuration = 0f;
        protected float timeBetweenSpawns;
        protected float spawnObjMoveSpeed;
    
        public Vector3 SpawnPosition { set => spawnPosition = value; }
        public Vector3 DespawnPosition { set => despawnPosition = value; }
        public float HazardDuration { set => hazardDuration = value; }

        public void InitSpawner(Vector3 spawnLocation, Vector3 despawnLocation, float hazardDuration, float timeBetweenSpawns, float spawnObjMoveSpeed)
        {
            spawnPosition = spawnLocation;
            despawnPosition = despawnLocation;

            var despawnVector = despawnPosition - spawnPosition;

            if (despawnVector.y > 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            if (despawnVector.y < 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

            this.hazardDuration = hazardDuration;
            this.timeBetweenSpawns = timeBetweenSpawns;
            this.spawnObjMoveSpeed = spawnObjMoveSpeed;
        }
    }
}
