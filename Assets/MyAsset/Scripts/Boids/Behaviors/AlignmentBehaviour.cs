using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Behaviour/Alignment")]
public class AlignmentBehaviour : FilteredBoidBehaviour
{
    public override Vector3 calculateMove(BoidAgent agent, List<Transform> context, BoidManager boids)
    {
        // If no neighbours, maintain current alignment
        if (context.Count == 0)
        {
            return agent.transform.forward;
        }

        // add all points together and average
        Vector3 alignmentMove = Vector3.zero;


        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            alignmentMove += item.forward;
        }

        alignmentMove /= context.Count;

        return alignmentMove;
    }
}