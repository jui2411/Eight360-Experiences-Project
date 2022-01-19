using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingController : MonoBehaviour
{
    public ThrowableAxe axe;

    public void ThrowAxe()
    {


        axe.thrown = true;

        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, 10))
            print("There is something in front of the object!");
    }

    public void PullBackAxe()
    {

    }

    public void CatchAxe()
    {

    }
}
