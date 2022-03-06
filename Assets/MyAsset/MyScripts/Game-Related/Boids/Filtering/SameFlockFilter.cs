using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Filter/Same Flock")]
public class SameFlockFilter : ContextFilter
{
    public override List<Transform> Filter(BoidAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            BoidAgent itemAgent = item.GetComponent<BoidAgent>();
            if (itemAgent != null && itemAgent.AgentBoids == agent.AgentBoids)
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }
}