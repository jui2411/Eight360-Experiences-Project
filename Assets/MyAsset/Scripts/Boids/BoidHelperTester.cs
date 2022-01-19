using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidHelperTester : MonoBehaviour
{
    public float boundsRadius = 5f;
    public float collisionAvoidDst = 5f;
    public LayerMask obstacleMask;

    private Ray ray;
    private Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            dir = transform.TransformDirection(rayDirections[i]);
            ray = new Ray(transform.position, dir);
            Debug.DrawRay(transform.position, dir, Color.green);
            
            // shows missed ray and return average of those directions
            if (!Physics.SphereCast(ray, boundsRadius, collisionAvoidDst, obstacleMask))
            {
                
            }
        }

        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position + dir);
        Gizmos.DrawWireSphere(ray.origin, boundsRadius);
        //Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(ray.origin, collisionAvoidDst);
    }
}
