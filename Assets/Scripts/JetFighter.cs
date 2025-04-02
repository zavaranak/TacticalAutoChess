using UnityEngine;
using System.Collections.Generic;

public class JetFighter : BaseEntity
{
    [SerializeField] private Projectile projectilePrefab;

    public override void Setup(Team team, Node spawnNode)
    {
        base.Setup(team, spawnNode);
        this.rangeType = RangeType.LongRange;
    }


    //protected override void FindTarget()
    //{
    //    var enemies = GameManager.Instance.GetEntitiesAgains(myTeam);

    //    float maxDistance = 0f;
    //    BaseEntity tempEnemy = null;
    //    if (enemies.Count > 0)
    //    {
    //        foreach (BaseEntity enemy in enemies)
    //        {
    //            if (enemy.ended) continue;
    //            Node enemyPosition = enemy.currentNode;
    //            if (enemyPosition == null) continue;
    //            if (Vector3.Distance(enemy.transform.position, transform.position) > maxDistance)
    //            {
    //                maxDistance = Vector3.Distance(enemy.transform.position, transform.position);
    //                tempEnemy = enemy;
    //            }
    //        }
    //    }
    //    if (tempEnemy == null)
    //    {
    //        Debug.Log(myTeam + "WON");
    //        ended = true;
    //        return;
    //    }
    //    this.target = tempEnemy;

    //}

    protected override void Attack()
    {
        if (!canAttack) return;
        if (target != null && target.currentNode != null)
        {
            Projectile newProjectiile = Instantiate(projectilePrefab.GetComponent<Projectile>());
            newProjectiile.Setup(this.transform.position, target.currentNode, myTeam, baseDamage);
        }
        else
        {
            Debug.Log("condition false to attack");
        }
        StartCoroutine(this.AttackCooldown());
    }

    protected override List<Node> GetPath(Node start, Node end)
    {
        return GridManager.Instance.GetAirPath(start, end);
    }
}
