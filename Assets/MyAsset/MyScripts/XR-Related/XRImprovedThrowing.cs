using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRImprovedThrowing : XRGrabInteractable
{
    private bool isGrabbing;
    private int currentVelocityFrameStep = 0;
    private Vector3[] velocityFrames = new Vector3[5];
    private Vector3[] angularVelocityFrames = new Vector3[5];

    private Rigidbody rb;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //if (isGrabbing) VelocityUpdate();
        VelocityUpdate();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        isGrabbing = true;
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        
        isGrabbing = false;

        GrabRelease();
        AddVelocityHistory();
        ResetVelocityHistory();

        base.OnSelectExiting(args);
    }

    private void GrabRelease()
    {
        Vector3 controllerCenterOfMass = rb.centerOfMass;
        Vector3 controllerVelocityCross = Vector3.Cross(rb.angularVelocity, transform.position - controllerCenterOfMass);
        Vector3 fullThrowVelocity = rb.velocity + controllerVelocityCross;
        rb.velocity = fullThrowVelocity * 100;
        rb.angularVelocity = rb.angularVelocity;
    }

    private void VelocityUpdate()
    {
        // increment the current frame step
        currentVelocityFrameStep++;

        // if the current frame index is greater than the max number of steps
        if (currentVelocityFrameStep >= velocityFrames.Length)
        {
            // reset steps when it goes over the value
            currentVelocityFrameStep = 0;
        }

        // set the velocity at the current rame step to equal the current 
        velocityFrames[currentVelocityFrameStep] = rb.velocity;
        angularVelocityFrames[currentVelocityFrameStep] = rb.angularVelocity;
    }

    // adds our velocity frame steps to the rigidbody to allow smooth out
    // throwing when sudden stops are made by the user
    private void AddVelocityHistory()
    {
        Vector3 velocityAverage = GetVectorAverage(velocityFrames);
        if(velocityAverage != null)
        {
            rb.velocity = velocityAverage;
        }

        Vector3 angularVelocityAverage = GetVectorAverage(angularVelocityFrames);
        if (angularVelocityAverage != null)
        {
            rb.angularVelocity = angularVelocityAverage;
        }
    }

    // resets our velocity frames when we exceed the number of frames we
    // want to store
    private void ResetVelocityHistory()
    {
        // first reset the current step to 0
        currentVelocityFrameStep = 0;
        // prevent null
        if (velocityFrames != null && velocityFrames.Length > 0)
        {
            // reset the frame step arrays by reinitializing
            velocityFrames = new Vector3[velocityFrames.Length];
            angularVelocityFrames = new Vector3[velocityFrames.Length];
        }
    }

    Vector3 GetVectorAverage(Vector3[] vectors)
    {
        // floats to store the postional data within
        float x = 0f;
        float y = 0f;
        float z = 0f;

        // how many vectors we have; we will divide by this
        int numVectors = 0;

        // run through our positions
        for(int i = 0; i < vectors.Length; i++)
        {
            if (vectors[i] != null)
            {
                // set x y z to equal the x y z values of the vector at our index
                x += vectors[i].x;
                y += vectors[i].y;
                z += vectors[i].z;

                // increment the number of vectors we have
                numVectors++;
            }
        }

        if(numVectors > 0)
        {
            // Get our average, only if numVectors isn't null
            Vector3 average = new Vector3(x / numVectors, y / numVectors, z / numVectors);
            return average;
        }

        Vector3 noVelocity = Vector3.zero;
        return noVelocity;
    }
}
