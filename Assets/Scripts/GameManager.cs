using System.Collections.Generic;
using UnityEngine;

public class GameManager : Manager<GameManager>
{
    [Header("Spawn Configuration")]
    [SerializeField] private BaseEntitySpawnConfig spawnConfig;
    Dictionary<Team, List<BaseEntity>> entitiesByTeam = new ();
    public HealthBar healthBarPrefab1;
    public HealthBar healthBarPrefab2;
    //public List<BaseEntity> allEntitiesPrefab;
    //int unitesPerTeam = 7;
    private new void Awake()
    {
        base.Awake();
    }

    public void InstantiateUnits()
    {
        entitiesByTeam.Add(Team.Team1, new List<BaseEntity>());
        entitiesByTeam.Add(Team.Team2, new List<BaseEntity>());

        SpawnTeam(Team.Team1);
        SpawnTeam(Team.Team2);

    }

    private void SpawnTeam(Team team)
    {
        var spawns = team == Team.Team1 ? spawnConfig.team1Spawns : spawnConfig.team2Spawns;

        foreach (var spawn in spawns)
        {
            if (spawn.team != team) continue;

            HealthBar healthBar = Instantiate(team == Team.Team1? healthBarPrefab1:healthBarPrefab2);
            BaseEntity newEntity = Instantiate(spawn.GetPrefab());
            Node spawnNode = GridManager.Instance.GetNodeByIndex(spawn.nodeIndex);
            healthBar.Setup(newEntity);

            if (spawnNode == null)
            {
                Debug.LogError($"No node at index {spawn.nodeIndex}");
                Destroy(newEntity.gameObject);
                continue;
            }

            newEntity.Setup(team, spawnNode);
            entitiesByTeam[team].Add(newEntity);
        }
    }

    public List<BaseEntity> GetEntitiesAgains(Team team) {
        if (team == Team.Team1)
            return entitiesByTeam[Team.Team2];
        else return entitiesByTeam[Team.Team1];
}
    public BaseEntity GetEntityAtNode(Node node, Team team) {
        BaseEntity tempEntity = entitiesByTeam[team].Find(
                entity => entity.currentNode == node
            );
        return tempEntity;
    }
}


public enum Team
{
    Team1,
    Team2
}

public enum UnitType
{
    Tank,
    JetFigher,
    Rocket
}