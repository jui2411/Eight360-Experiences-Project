using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;

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
}