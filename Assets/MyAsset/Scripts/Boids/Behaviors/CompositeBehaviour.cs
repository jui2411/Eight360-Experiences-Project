using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Behaviour/Composite")]
public class CompositeBehaviour : BoidBehaviour
{
    public BoidBehaviour[] behaviors;
    public float[] weights;

    public override Vector3 calculateMove(BoidAgent agent, List<Transform> context, BoidManager boids)
    {
        if (weights.Length != behaviors.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        Vector3 move = Vector3.zero;

        //iterate through behaviours
        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector3 partialMove = behaviors[i].calculateMove(agent, context, boids) * weights[i];

            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }

}