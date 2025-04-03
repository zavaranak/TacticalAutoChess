using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Rocket : BaseEntity
{
    private Node latestDestination = null;
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
        if (target == null) FindTarget();
        if (target !=null && latestDestination ==null)
                latestDestination = target.currentNode;
        if (canAttack)
        {
            DirectHit();
        }
        if (Vector3.Distance(transform.position, latestDestination.worldPosition) < 0.2f)
        {
            canAttack = false;
            StartCoroutine(Explode());
            if (target!=null && !target.ended) target.TakeDammage(baseDamage);
            List<Node> surroundingAreas = GridManager.Instance.GetNeighbors(latestDestination);

            Team enemySide = myTeam == Team.Team1 ? Team.Team2 : Team.Team1;
            foreach (Node area in surroundingAreas)
            {
                BaseEntity enemy = GameManager.Instance.GetEntityAtNode(area, enemySide);
                if (enemy != null && !enemy.ended)
                {
                    enemy.TakeDammage(baseDamage);
                }
            }

            
        }
    }

    protected override void FindTarget()
    {
        base.FindTarget();
       
    }
    protected override void Attack()
    {
        StartCoroutine(this.AttackCooldown());
    }

    private void DirectHit()
    {
        currentNode.SetOccupied(false);
        List<Node> path = GetPath(currentNode, latestDestination);
        if (path.Count > 1) { SetCurrentNode(path[1]); MoveTowards(path[1]); }
        else
        {
            MoveTowards(latestDestination);
        }
    }

    protected override List<Node> GetPath(Node start, Node end)
    {
        return GridManager.Instance.GetAirPath(start, end);
    }

    private IEnumerator Explode()
    {
        base.currentHealth = 0;
        animator.SetTrigger("Explode");
        yield return new WaitForSeconds(2f);
        //base.OnDeath();
        ended = true;
    }
}

