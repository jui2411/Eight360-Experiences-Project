using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionFind : MonoBehaviour
{
    TurretController player;
    public GameObject FX;


    private void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<TurretController>();
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        //if (!collision.gameObject.CompareTag("Health")) { return; }

        //if (Vector3.Distance(transform.position, TurretController.instance.gameObject.transform.position) < 1000){
        //    player.TakeDamage(0.1f);
        //    GameObject FracturePS = Instantiate(FX, transform.position, transform.rotation);
        //    FracturePS.transform.localScale = transform.localScale;


        //}
        //this.gameObject.SetActive(false);
        //return;
        if (collision.gameObject.CompareTag("Health"))
        {
            player.TakeDamage(0.1f);
            GameObject FracturePS = Instantiate(FX, transform.position, transform.rotation);
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("WEIRD");
        }
    }
}
