using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tank : BaseEntity
{
    [SerializeField] private Projectile projectilePrefab;
    public override void Setup(Team team, Node spawnNode)
    {
        base.Setup(team, spawnNode);
        //this.range = 2;
        this.deathCountDown = 200;
        //this.movementSpeed = 0.5f;
    }

    protected override void Attack()
    {
        //base.Attack();
        if (!canAttack) return;
        if (target != null && target.currentNode != null) { 
            Projectile newProjectiile = Instantiate(projectilePrefab.GetComponent<Projectile>());
            newProjectiile.Setup(this.transform.position,target.currentNode);
        }
        else
        {
            Debug.Log("condition false to attack");
        }
        StartCoroutine(this.AttackCooldown());
    }
}
