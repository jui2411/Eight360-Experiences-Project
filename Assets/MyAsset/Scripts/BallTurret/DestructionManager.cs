using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionManager : MonoBehaviour
{
    public Transform SpawnPoint;
    public Transform DestPoint;
    public float SpawnPointRadius = 5f;
    public float DestMaxRadius = 5f;
    public float DestMinRadius = 5f;

    public GameObject spawnGO;

    public int amountToSpawn = 20;
    public List<GameObject> targets;
    public float targetSpeed;

    public GameObject marker;

    public float minScaleMultiplier = 0.5f;
    public float maxScaleMultiplier = 2f;
    float elapsed = 0f;
    float deadTime = 5f;

    //Debugging
    public Camera cam;
    public Transform MarkerCanvas;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 destCenter = DestPoint.position;
        foreach (GameObject GO in targets)
        {
            // Target
            // Move our position a step closer to the target.
            float speed = targetSpeed;
            float step = speed * Time.deltaTime; // calculate distance to move
            Vector3 destPos = RandomCircle(destCenter, DestMinRadius, DestMaxRadius);
            GO.transform.position = Vector3.MoveTowards(GO.transform.position, destPos, step);
            Debug.DrawRay(GO.transform.position, destPos);
        }

        elapsed += Time.deltaTime;
        if (elapsed >= deadTime)
        {
            elapsed = elapsed % deadTime;
            SpawnTarget();
        }
    }

    [ContextMenu("SPAWN")]
    public void SpawnTarget()
    {
        Vector3 spawnCenter = SpawnPoint.position;
        for (int i = 0; i < amountToSpawn; i++)
        {
            // Spawn
            Vector3 spawnPos = RandomCircle(spawnCenter, 0, SpawnPointRadius);
            Quaternion spawnRot = Quaternion.FromToRotation(Vector3.forward, spawnCenter - DestPoint.position);
            GameObject target = Instantiate(spawnGO, spawnPos, spawnRot);
            targets.Add(target);

            TargetWaypoint newMarker = Instantiate(marker, spawnPos, Quaternion.identity).GetComponent<TargetWaypoint>();
            newMarker.target = target.transform;
            newMarker.cam = cam;
            newMarker.transform.SetParent(MarkerCanvas);
        }

        
    }

    Vector3 RandomCircle(Vector3 center, float _MinRadius, float _MaxRadius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + _MaxRadius * Mathf.Sin(ang * Mathf.Deg2Rad);
        if(pos.x < center.x + _MinRadius)
        {
            pos.x = center.x + _MinRadius;
        }
        pos.y = center.y; // + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

}
