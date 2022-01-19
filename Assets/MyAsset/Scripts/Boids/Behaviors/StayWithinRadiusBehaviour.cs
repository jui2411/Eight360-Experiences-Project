using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Behaviour/StayWithinRadius")]
public class StayWithinRadiusBehaviour : BoidBehaviour
{

    public Vector3 center;
    public float radius = 15f;

    public override Vector3 calculateMove(BoidAgent agent, List<Transform> context, BoidManager boids)
    {
        Vector3 centerOffset = center - agent.transform.position;
        float t = centerOffset.magnitude / radius;

        // If within 10% of the radius
        if (t < 0.9f)
        {
            return Vector3.zero;
        }

        return centerOffset * t * t;
    }
}