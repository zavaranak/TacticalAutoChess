using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tank : BaseEntity
{
    public override void Setup(Team team, Node spawnNode)
    {
        base.Setup(team, spawnNode);
        //this.range = 2;
        this.deathCountDown = 50;
        //this.movementSpeed = 0.5f;
    }
}
