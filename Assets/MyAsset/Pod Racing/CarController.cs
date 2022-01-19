using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody sphere; 
    public float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 18;

    private float speedInput, turnInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speedInput = 0f;
        if(Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel * 1000f;
        } else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
        }
        transform.position = sphere.transform.position; //Move to where the sphere (rigidbody) is
    }

    private void FixedUpdate()
    {
        
        if(Mathf.Abs(speedInput) > 0)
        {
            sphere.AddForce(transform.forward * speedInput);
        }
    }
}
