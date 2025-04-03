using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpawnConfig", menuName = "Game/Spawn Config")]
public class BaseEntitySpawnConfig : ScriptableObject
{
    [System.Serializable]
    public class SpawnPoint
    {
        //public UnitType unitType;
        public GameObject unitPrefab;
        public int nodeIndex;
        public Team team;

        public BaseEntity GetPrefab()
        {
            if (unitPrefab != null && unitPrefab.TryGetComponent<BaseEntity>(out var entity))
                return entity;

            Debug.LogError($"Prefab missing BaseEntity component");
            return null;
        }
    }

    public List<SpawnPoint> team1Spawns = new List<SpawnPoint>();
    public List<SpawnPoint> team2Spawns = new List<SpawnPoint>();
}
