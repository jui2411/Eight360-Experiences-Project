using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableAxe : MonoBehaviour
{
    public bool thrown = false;
    public float rotationSpeed;


    void Update()
    {

        if (thrown)
        {
            transform.localEulerAngles += -Vector3.forward * rotationSpeed * Time.deltaTime;
        }

    }

    

    
}


