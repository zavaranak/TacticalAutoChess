using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BaseEntity : MonoBehaviour
{
    public RangeType rangeType = RangeType.ShortRange;
    public Vector3 currentDirection;
    public SpriteRenderer spriteRenderer;

    protected Animator animator;
    //public HealthBar healthBar;
    public float baseDamage ;
    public float baseHealth;
    public float currentHealth;
    public float range = 3f;

    public float attackCooldown;
    public float movementSpeed = 1f;

    protected BaseEntity target;

    protected Team myTeam;

    public Node currentNode;

    protected Node destination;

    protected Node startingPosition;

    protected bool moving = false;

    public bool ended = false;
    protected bool InPosition => Vector3.Distance(transform.position, currentNode.worldPosition) <= 0.05f;
    protected bool InRange => target!=null && target.currentNode!=null && Vector3.Distance(transform.position, target.transform.position) <= range;
    protected bool Attacking => InRange && (target.transform.position- transform.position) == currentDirection;

    protected bool canAttack = true;

    void Awake()
    {
    }
    protected virtual void Update()
    {
        if (ended) { return; };
        if (currentHealth <= 0) { OnDeath();return; };
        if (Attacking) {
            Attack();
        };

        if (currentNode == null)
        {
            return;
        }

        FindTarget();

        if (InRange) {
            if (!Attacking) {
                GetInPosition();
            } 
        }else
        {   
            GetInRange();
        }

    }
    public virtual void Setup(Team team, Node spawnNode)
    {
        

        if (spawnNode == null)
        {
            Debug.Log("spawnNode is null at Setup");
        }
        myTeam = team;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        startingPosition = spawnNode;
        if (myTeam == Team.Team1)
        {
            currentDirection = Vector3.up;
            transform.up = currentDirection;
        }
        else
        { 
            currentDirection = Vector3.up;
            transform.up = currentDirection;
            SelfRotate(Vector3.down);
        }

        SetCurrentNode(spawnNode);
        transform.position = currentNode.worldPosition;
        currentHealth = baseHealth;
        currentNode.SetOccupied(true);
        //healthBar = Instantiate(healthBar, transform);
        //healthBar.Setup(transform);
    }
    protected virtual void GetInPosition()
    {
        Vector3 directionToEnemy = (target.transform.position - transform.position);
        SelfRotate(directionToEnemy);
        if (!InPosition)
        { Vector3 direction = (currentNode.worldPosition - transform.position);
            transform.position += movementSpeed * Time.deltaTime * direction.normalized;
        }
        if (InPosition)
        {
            transform.position = currentNode.worldPosition;
        }


    }
    protected virtual void FindTarget()
    {
        BaseEntity tempEnemy = rangeType == RangeType.ShortRange ? FindNearestEnemy() : rangeType == RangeType.LongRange ? FindFurthestEmeny() : null;

        if (tempEnemy == null)
        {
            Debug.Log(myTeam + "WON");
            ended = true;
            return;
        }
        target = tempEnemy;
        
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
        Vector3 direction = (_destination.worldPosition - transform.position);

        if (direction.sqrMagnitude <= 0.005f)
        {
            transform.position = _destination.worldPosition;
            return true;
        }
        SelfRotate(direction);
        
        transform.position += movementSpeed * Time.deltaTime * direction.normalized;
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
            candidates = candidates.OrderBy(x => Vector3.Distance(x.worldPosition, transform.position)).ToList();
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

            var path = GetPath(currentNode, destination);
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

        currentNode.SetOccupied(false);
        currentNode = null;
        spriteRenderer.enabled = false;
        Debug.Log("one unit down from" + myTeam);
        ended = true;
    }

    protected virtual void Attack() {
        //override
    }

    public  virtual void TakeDammage(float damage)
    {
        Debug.Log(currentNode.index + " hit by enemy");
        currentHealth -= damage;
        //healthBar.SetHealth((currentHealth / baseHealth)>= 0 ? currentHealth / baseHealth : 0);
    }

    public IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    protected virtual List<Node> GetPath(Node start, Node end)
    {
        return new List<Node>() { };
    }

    protected BaseEntity FindFurthestEmeny()
    {
        var enemies = GameManager.Instance.GetEntitiesAgains(myTeam);
        float maxDistance = 0f;
        BaseEntity tempEnemy = null;
        if (enemies.Count > 0)
        {
            foreach (BaseEntity enemy in enemies)
            {
                if (enemy.ended) continue;
                Node enemyPosition = enemy.currentNode;
                if (enemyPosition == null) continue;
                if (Vector3.Distance(enemy.transform.position, transform.position) > maxDistance)
                {
                    maxDistance = Vector3.Distance(enemy.transform.position, transform.position);
                    tempEnemy = enemy;
                }
            }
        }
        return tempEnemy;
    }

    protected BaseEntity FindNearestEnemy()
    {
        var enemies = GameManager.Instance.GetEntitiesAgains(myTeam);

        float minDistance = Mathf.Infinity;
        BaseEntity tempEnemy = null;
        if (enemies.Count > 0)
        {
            foreach (BaseEntity enemy in enemies)
            {
                if (enemy.ended) continue;
                Node enemyPosition = enemy.currentNode;
                if (enemyPosition == null) continue;
                if (Vector3.Distance(enemy.transform.position, transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(enemy.transform.position, transform.position);
                    tempEnemy = enemy;
                }
            }
        }
        return tempEnemy;
    }
}

public enum RangeType
{
    ShortRange,
    LongRange
}