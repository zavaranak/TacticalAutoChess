using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 7f;
    protected Node destination;
    protected bool ready = false;
    protected bool hitTarget = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Setup(Vector3 initPosition, Node targetPosition)
    {
        
        this.transform.up = (targetPosition.worldPosition - initPosition);
        this.destination = targetPosition;
        this.transform.position = initPosition;
        this.ready = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (destination != null)
        {
            if (!destination.IsOccupied)
            {
                Debug.Log("Target moved from destination");
            }
            FireAtDestination();
        }
    }

    virtual protected void FireAtDestination()
    {
    
    this.transform.position = Vector3.MoveTowards(
    this.transform.position,
    this.destination.worldPosition,
    speed * Time.deltaTime);
       // Destroy if reached target
    if (transform.position == destination.worldPosition)
        {
            
            hitTarget = true;
            Destroy(gameObject);
        }
    }
}
