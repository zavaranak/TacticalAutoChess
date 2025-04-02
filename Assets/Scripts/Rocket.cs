using UnityEngine;
using System.Collections.Generic;
public class Rocket : BaseEntity
{
    [SerializeField] private Projectile projectilePrefab;
    public override void Setup(Team team, Node spawnNode)
    {   
        base.Setup(team, spawnNode);
        range = 10;
        canAttack = false;
        Attack();
    }


    protected override void Update()
    {
        if (ended) return;
        if (baseHealth <= 0) { OnDeath(); return; };
        FindTarget();
        if (canAttack && InRange)
        {
            DirectHit();
        }
        if(canAttack && Vector3.Distance(transform.position,target.currentNode.worldPosition) < 0.1f)
        {
            target.TakeDammage(baseDamage);
            List<Node> surroundingAreas = GridManager.Instance.GetNeighbors(target.currentNode);

            Team enemySide = myTeam == Team.Team1 ? Team.Team2 : Team.Team1;
            foreach (Node area in surroundingAreas)
            { 
                BaseEntity enemy = GameManager.Instance.GetEntityAtNode(area,enemySide);
                if (enemy != null)
                {
                    enemy.TakeDammage(baseDamage);
                }
            }
            
            base.OnDeath();
        }
    }

    protected override void Attack()
    {
        StartCoroutine(this.AttackCooldown());
    }

    private void DirectHit()
    {
        currentNode.SetOccupied(false);
        List<Node> path = GetPath(currentNode,target.currentNode);
        if (path.Count > 1) { SetCurrentNode(path[1]); MoveTowards(path[1]); }
        else
        {
            MoveTowards(target.currentNode);
        }
    }

    protected override List<Node> GetPath(Node start, Node end)
    {
        return GridManager.Instance.GetAirPath(start, end);
    }
}

