using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public GameObject destMarker;
    [HideInInspector]
    public Vector3 destPos;
    [HideInInspector]
    public float speed = 10f;

    private void Update()
    {
        if(destPos != Vector3.zero)
        {
            
            Debug.DrawLine(transform.position, destPos);
        }

        transform.rotation = Quaternion.FromToRotation(Vector3.forward, destPos);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Fracture fractureScript = GetComponent<Fracture>();

        if(fractureScript != null)
        {
            fractureScript.FractureObject();
        }

        gameObject.SetActive(false);
    }

    public void SetDestination(Vector3 _destPos)
    {
        destPos = _destPos;

        if (destMarker)
        {
            GameObject marker = Instantiate(destMarker, destPos, transform.rotation);
        }
    }

    public void MoveToDestination() 
    {
        float step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, destPos, step);


    }
}