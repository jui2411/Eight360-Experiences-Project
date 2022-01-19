using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    public LayerMask mask;

    private void CallRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 100, mask, QueryTriggerInteraction.Ignore))
        {
            print(hitInfo.collider.gameObject);
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        } else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.green);
        }
    }
}

struct Point
{
    public float x;
    public float y;

    public Point (float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
