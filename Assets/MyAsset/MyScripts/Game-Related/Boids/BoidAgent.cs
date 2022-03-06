using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Collider detection for detecting neighbours
[RequireComponent(typeof(Collider))]
public class BoidAgent : MonoBehaviour
{

    //Edit Later

    //Unity has a deprecated variable inside of the component class
    //itself called collider so you can't just call it directly 
    //othrewise it causes an override sitation and unity doesn't like it
    //Hence, use { get { return agentCollider} }
    // This means that will be able to access this collider without
    // ever being ble to assign to it. The only time we want to assign to it
    // is when its actually first starting up.
    [HideInInspector]
    public Vector3 currentVelocity; //Essentially direction

    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    BoidManager agentBoids;
    public BoidManager AgentBoids { get { return agentBoids; } }

    // Debug
    public enum GizmoType { Never, SelectedOnly, Always }
    public Color colour;
    public GizmoType showNeighbourRegion;

    [HideInInspector]
    public bool debugDirection = true;
    private float neighbourRadius = 0f;


    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
        GetNeighbourRadius();
    }

    public void Initialize(BoidManager selectedBoids)
    {
        agentBoids = selectedBoids;
    }

    public void Move(Vector3 velocity)
    {

        this.transform.position += velocity * Time.deltaTime; // Every velocity(Vector3) unit of space travelled per frame
        this.transform.rotation = Quaternion.LookRotation(velocity); // Rotate towards where object is moving towards

        //DEMO ONLY
        if (debugDirection) Debug.DrawRay(transform.position, velocity, Color.green);
        currentVelocity = velocity;
    }

    public void InitializeSpeed(float velocity)
    {

    }


    private void OnDrawGizmos()
    {
        if (showNeighbourRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showNeighbourRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    void DrawGizmos()
    {

        Gizmos.color = new Color(colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere(transform.position, neighbourRadius);
    }

    public void GetNeighbourRadius()
    {
        neighbourRadius = FindObjectOfType<BoidManager>().neighborRadius;
    }
}
