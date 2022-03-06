using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public BoidAgent agentPrefab;
    List<BoidAgent> agents = new List<BoidAgent>();
    public BoidBehaviour behaviour;

    [Range(1, 500)]
    public int spawnCount = 250;
    public float spawnRadius = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float minSpeed = 1f;
    [Range(0.01f, 100f)]
    public float maxSpeed = 5f;
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    public enum GizmoType { Never, SelectedOnly, Always }
    public Color colour;
    public GizmoType showSpawnRegion;

    public bool debugAllAgentDirection = true;

    float squareMaxSpeed;
    float squareNeighbourRadius;
    float squareAvoidanceRadius;

    

    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighbourRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareAvoidanceRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        SpawnBoids();

    }

    // Update is called once per frame
    void Update()
    {
        foreach (BoidAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent); // List of context that exists in the neighbour radius

            //DEMO ONLY
            //agent.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.red, context.Count/6f); // Debug Purpose



            Vector3 move = behaviour.calculateMove(agent, context, this);
            move *= driveFactor; // drivefactor for speedier movement
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed; // limit speed to max speed OLD
                
            }
            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(BoidAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
        foreach (Collider c in contextColliders)
        {
            // Avoid finding agent's own collider
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    private void SpawnBoids()
    {
        BoidAgent[] newAgents = new BoidAgent[spawnCount];
        for (int i = 0; i < spawnCount; i++)
        {
            var randomVector = UnityEngine.Random.insideUnitSphere * spawnRadius;
            var spawnPos = transform.position + randomVector;
            var spawnRot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            newAgents[i] = Instantiate(agentPrefab, spawnPos, spawnRot, transform);
            agents.Add(newAgents[i]);
            newAgents[i].name = "Agent" + i;
            newAgents[i].Initialize(this);

            if (debugAllAgentDirection)
            {
                newAgents[i].debugDirection = true;
            }
            else
            {
                newAgents[i].debugDirection = false;
            }
            //newAgents[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }

    private void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    void DrawGizmos()
    {

        Gizmos.color = new Color(colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }

}