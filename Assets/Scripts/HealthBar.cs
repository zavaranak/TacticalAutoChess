using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Vector3 offset;
    private BaseEntity target;
    public void Setup(BaseEntity target)
    {
       this.target = target;
       SetHealth(1f);
    }
    private void Update()
    {
        if (target != null)
        {
            transform.position = target.transform.position + offset;
            float value = target.currentHealth / target.baseHealth;
           
            SetHealth(value>=0?value:0);
            //transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.Log("target is null");
        }
    }

    public void SetHealth(float health)
    {
        if(health == 0)
        {
            Destroy(gameObject);
        }
        slider.value = health;
    }

}