using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    public GameObject shieldFX;

    // Start is called before the first frame update
    void Start()
    {
        shieldFX.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage()
    {
        StartCoroutine(ActivateShield());
    }

    IEnumerator ActivateShield()
    {
        shieldFX.SetActive(true);
        yield return new WaitForSeconds(1f);
        shieldFX.SetActive(false);
    }
}
