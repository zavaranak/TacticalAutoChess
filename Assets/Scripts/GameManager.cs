using System.Collections.Generic;
using UnityEngine;

public class GameManager : Manager<GameManager>
{
    [Header("Spawn Configuration")]
    [SerializeField] private BaseEntitySpawnConfig spawnConfig;
    Dictionary<Team, List<BaseEntity>> entitiesByTeam = new ();
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

        //for (int i = 0; i < unitesPerTeam; i++)
        //{
        //    //team1 unit 
        //    int randomIndex = UnityEngine.Random.Range(0, allEntitiesPrefab.Count - 1);
            //BaseEntity newEntity = Instantiate(allEntitiesPrefab[randomIndex]);
        //    entitiesByTeam[Team.Team1].Add(newEntity);

        //    newEntity.Setup(Team.Team1, GridManager.Instance.GetFreeNode(Team.Team1));

        //    //team2 unit
        //    int randomIndex2 = 0;
        //    if (randomIndex<allEntitiesPrefab.Count -1 )
        //    {
        //        randomIndex2 = randomIndex + 1;
        //    }
        //    BaseEntity newEntity2 = Instantiate(allEntitiesPrefab[randomIndex2]);
        //    entitiesByTeam[Team.Team2].Add(newEntity2);
        //    newEntity2.Setup(Team.Team2, GridManager.Instance.GetFreeNode(Team.Team2));

        //}
    }

    private void SpawnTeam(Team team)
    {
        var spawns = team == Team.Team1 ? spawnConfig.team1Spawns : spawnConfig.team2Spawns;

        foreach (var spawn in spawns)
        {
            if (spawn.team != team) continue;

            BaseEntity newEntity = Instantiate(spawn.GetPrefab());
            Node spawnNode = GridManager.Instance.GetNodeByIndex(spawn.nodeIndex);

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