using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    [Tooltip("\"Fractured\" is the object that this will break into")]
    public GameObject fractured;
    public float breakForce = 100f;
    public GameObject FractureFX;

    private float scaling = 0;

    private void Start()
    {
        scaling = transform.localScale.x;
    }

    public void FractureObject()
    {
        GameObject fracturedGO = Instantiate(fractured, transform.position, transform.rotation); //Spawn in the broken version
        fracturedGO.transform.localScale = new Vector3(scaling, scaling, scaling);

        GameObject FracturePS = Instantiate(FractureFX, transform.position, transform.rotation);
        FracturePS.transform.localScale = new Vector3(scaling, scaling, scaling);

        foreach (Rigidbody rb in fracturedGO.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - transform.position).normalized * breakForce;
            rb.AddForce(force);
        }
        //gameObject.SetActive(false); //Destroy the object to stop it getting in the way
    }
}
