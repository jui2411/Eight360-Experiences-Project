using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionFind : MonoBehaviour
{
    TurretController player;
    public GameObject FX;
    public LayerMask damageLayer;


    private void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<TurretController>();
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == damageLayer) {
            player.TakeDamage(0.1f);
            GameObject FracturePS = Instantiate(FX, transform.position, transform.rotation);
            this.gameObject.SetActive(false);
        } else
        {
            Debug.Log("WEIRD");
        }
    }
}
