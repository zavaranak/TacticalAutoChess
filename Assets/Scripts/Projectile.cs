using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Team against;
    private float damage; 
    readonly float speed = 4f;
    protected Node destination;
    protected bool hit;

    public void Setup(Vector3 initPosition, Node targetPosition, Team myTeam, float damage)
    {  
        transform.up = (targetPosition.worldPosition - initPosition);
        destination = targetPosition;
        transform.position = initPosition;
        against = myTeam == Team.Team1?Team.Team2:Team.Team1;
        hit = false;
        this.damage = damage;
    }

    void Update()
    {
        if (!hit && destination !=null)
        {
            OnWay();
        }
    }

    private void OnWay()
    {
      transform.position = Vector3.MoveTowards(transform.position,destination.worldPosition,speed *Time.deltaTime);

      if (transform.position == destination.worldPosition)
        {
            OnHit();
        }
    }

    private void OnHit() {
        hit = true;
        BaseEntity target = GameManager.Instance.GetEntityAtNode(destination,against);
        if (target != null)
        {
            target.TakeDammage(damage);
        }
        Destroy(gameObject);
    }
}
