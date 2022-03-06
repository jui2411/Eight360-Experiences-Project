using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Behaviour/SteeredCohesion")]
public class SteeredCohesionBehaviour : FilteredBoidBehaviour
{
    Vector3 currentVelocity;
    public float agentSmoothTime = 0.5f;//0.5f means half a sec

    public override Vector3 calculateMove(BoidAgent agent, List<Transform> context, BoidManager boids)
    {
        // If no neighbours, return no adjustment
        if (context.Count == 0)
        {
            return Vector3.zero;
        }

        // add all points together and average
        Vector3 cohesionMove = Vector3.zero;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            cohesionMove += item.position;
        }

        cohesionMove /= context.Count;

        // Create offset from agent position
        cohesionMove -= agent.transform.position;

        cohesionMove = Vector3.SmoothDamp(agent.transform.forward, cohesionMove, ref currentVelocity, agentSmoothTime);
        return cohesionMove;
    }
}
