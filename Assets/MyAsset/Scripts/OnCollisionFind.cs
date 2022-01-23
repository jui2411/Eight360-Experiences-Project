using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionFind : MonoBehaviour
{
    TurretController player;
    private void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<TurretController>();
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        player.TakeDamage(0.1f);
    }
}
