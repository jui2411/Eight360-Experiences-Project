using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{

    public bool invertAngle;
    public float AngleDiff = 2f;

    private Vector3 orgRot;
    private float closeAng = 0f;
    private float closeAngEst = 0f;
    private Rigidbody rb;
    private HingeJoint hj;
    [SerializeField] private UnityEvent doorClosedEvent;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        hj = GetComponent<HingeJoint>();

        orgRot = transform.localEulerAngles;

        closeAng = 0f;
        closeAngEst = closeAng + AngleDiff;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckDoor()
    {
        if((!invertAngle && hj.angle < closeAngEst) || (invertAngle && hj.angle > -closeAngEst))
        {
            CloseDoor();
        }

        Debug.Log(closeAngEst + " -Compare- " + hj.angle);
    }

    private void CloseDoor()
    {
        transform.localEulerAngles = orgRot;
        rb.velocity = Vector3.zero;
        doorClosedEvent.Invoke();
        Debug.Log("Door is CLosed");
    }

    
}
