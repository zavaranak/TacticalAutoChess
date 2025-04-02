using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BaseEntity : MonoBehaviour
{
    public Vector3 currentDirection;
    public SpriteRenderer spriteRenderer;
    public int baseDamage = 1;
    public int baseHealth = 1;
    public float range = 3f;

    public float attackSpeed = 1f;
    public float movementSpeed = 1f;

    protected BaseEntity target;

    protected Team myTeam;

    protected Node currentNode;

    protected Node destination;

    protected Node startingPosition;

    protected bool moving = false;

    protected bool done = false;
    protected bool InPosition => Vector3.Distance(this.transform.position, this.currentNode.worldPosition) <= 0.05f;
    protected bool InRange => target!=null && target.currentNode!=null && Vector3.Distance(this.transform.position, target.transform.position) <= range;
    protected bool Attacking => InRange && (target.transform.position- this.transform.position) == currentDirection;

    protected int deathCountDown = 100;


    void Awake()
    {
    }
    protected virtual void Update()
    {
        if (done) return;
        if (Attacking) {
            deathCountDown -= 1;
            if (deathCountDown == 0)
            {
                OnDeath();
            }
        };

        if (currentNode == null)
        {
            Debug.Log("current node is null at Update");
            return;
        }

        this.FindTarget(); 
       
        if (InRange) {
            
            if (!Attacking) {
                if (target != null && target.currentNode != null)
                { 
                Debug.Log("Ready to attack from " + this.currentNode.index + " to " + target.currentNode.index) ; this.GetInPosition(); }
                }
            else
            {
                if (target != null && target.currentNode !=null)
                {
                    Debug.Log("Attack from " + this.currentNode.index);
                    Debug.Log(" to " + target.currentNode.index);
                }

            }
        }else
        {
            this.GetInRange();
        }

    }
    public virtual void Setup(Team team, Node spawnNode)
    {
        if (spawnNode == null)
        {
            Debug.Log("spawnNode is null at Setup");
        }
        myTeam = team;
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        this.startingPosition = spawnNode;
        if (myTeam == Team.Team1)
        {
            currentDirection = Vector3.up;
            this.transform.up = currentDirection;
        }
        else
        { 
            currentDirection = Vector3.up;
            this.transform.up = currentDirection;
            SelfRotate(Vector3.down);
        }

        SetCurrentNode(spawnNode);
        transform.position = currentNode.worldPosition;
        currentNode.SetOccupied(true);
    }
    protected virtual void GetInPosition()
    {
        Vector3 directionToEnemy = (this.target.transform.position - this.transform.position);
        SelfRotate(directionToEnemy);
        if (!this.InPosition)
        { Vector3 direction = (this.currentNode.worldPosition - this.transform.position);
            this.transform.position += movementSpeed * Time.deltaTime * direction.normalized;
        }
        if (this.InPosition)
        {
            this.transform.position = this.currentNode.worldPosition;
        }


    }
    protected virtual void FindTarget()
    {
        var enemies = GameManager.Instance.GetEntitiesAgains(myTeam);

        //find closest enemy:
        float minDistance = Mathf.Infinity;
        BaseEntity tempEnemy = null;
        if (enemies.Count > 0)
        {
            foreach (BaseEntity enemy in enemies)
            {
                Node enemyPosition = enemy.currentNode;
                if (enemyPosition == null) continue;
                if (Vector3.Distance(enemy.transform.position, this.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(enemy.transform.position, this.transform.position);
                    tempEnemy = enemy;
                }
            }
        }
        if (tempEnemy == null)
        {
            Debug.Log(myTeam + "WON");
            done = true;
            return;
        }
        this.target = tempEnemy;
        
    }

   public virtual void SetCurrentNode(Node node)

    {
        if (node == null)
        {
            Debug.LogError("Trying to set null node!");
            return;
        }
        currentNode = node;
    }

    protected virtual bool MoveTowards(Node _destination)
    {
        Vector3 direction = (_destination.worldPosition - this.transform.position);

        if (direction.sqrMagnitude <= 0.005f)
        {
            this.transform.position = _destination.worldPosition;
            // animator.SetBool("walking", false);
            return true;
        }
        SelfRotate(direction);
        
        this.transform.position += movementSpeed * Time.deltaTime * direction.normalized;
        // animator.SetBool("walking", true);
        return false;
    }

    protected virtual void SelfRotate(Vector3 direction)
    {
        float angle = Vector3.SignedAngle(currentDirection, direction, Vector3.forward); // Get signed angle
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward); // Create rotation
        transform.rotation *= targetRotation;
        currentDirection = direction;  
    }
    protected virtual void GetInRange()
    {
        if (target == null)
            return;

        if (!moving)
        {
            destination = null;
            List<Node> candidates = GridManager.Instance.GetNeighbors(target.currentNode);
            candidates = candidates.OrderBy(x => Vector3.Distance(x.worldPosition, this.transform.position)).ToList();
            for (int i = 0; i < candidates.Count; i++)
            {
                if (!candidates[i].IsOccupied)
                {
                    destination = candidates[i];
                    break;
                }
            }
            if (destination == null)
                return;

            var path = GridManager.Instance.GetPath(currentNode, destination);
            if (path == null || path.Count <= 1)
                return;

            if (path[1].IsOccupied)
                return;

            if (currentNode.IsOccupied)currentNode.SetOccupied(false);

            path[1].SetOccupied(true);
            destination = path[1];
            SetCurrentNode(destination);
        }

        moving = !MoveTowards(destination);
    }

    protected virtual void OnDeath()
    {
        this.currentNode.SetOccupied(false);
        this.currentNode = null;
        spriteRenderer.enabled = false;
        Debug.Log("one unit down from" + myTeam);
        done = true;
    }

    protected virtual  void Attack() { 
    
    }
}

