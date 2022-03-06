using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Events : MonoBehaviour
{
    public float health = 10;
    public event Action OnPlayerDeath;

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            if(OnPlayerDeath != null)
            {
                OnPlayerDeath();
            }
        }
        Destroy(gameObject);
    }
}
