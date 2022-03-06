using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Behaviour/Avoidance")]
public class AvoidanceBehaviour : BoidBehaviour
{

    public float boundsRadius = 1.5f;
    public float collisionAvoidDst = 2f;
    public LayerMask obstacleMask;

    //Smoothing
    //public float agentSmoothTime = 0.5f;
    //Vector3 currentVelocity;

    public override Vector3 calculateMove(BoidAgent agent, List<Transform> context, BoidManager boids)
    {
        // If no neighbours, return no adjustment
        if (context.Count == 0)
        {
            return Vector3.zero;
        }



        // add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0; // No of things to avoid



        // If there is context filter assigned, then run filter function
        //List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        //foreach (Transform item in filteredContext)
        //{
        //    if (Vector3.SqrMagnitude(item.position - agent.transform.position) < boids.SquareAvoidanceRadius)
        //    {
        //        nAvoid++;
        //        avoidanceMove += agent.transform.position - item.position;
        //    }
        //}

        //if (nAvoid > 0)
        //{
        //    avoidanceMove /= nAvoid;
        //}

        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = agent.transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(agent.transform.position, dir);
            //Debug.DrawRay(agent.transform.position, dir * collisionAvoidDst, Color.green);

            // shows missed ray and return average of those directions
            if (!Physics.SphereCast(ray, boundsRadius, collisionAvoidDst, obstacleMask))
            {
                //Debug.DrawRay(agent.transform.position, dir * collisionAvoidDst, Color.red);
                avoidanceMove += dir;
            }
            else
            {
                avoidanceMove += agent.transform.forward;
                
            }
        }

        if (rayDirections.Length > 0)
        {
            avoidanceMove /= rayDirections.Length;
        }

        
        //avoidanceMove = Vector3.SmoothDamp(agent.transform.forward, avoidanceMove, ref currentVelocity, agentSmoothTime);

        return avoidanceMove;
    }
}