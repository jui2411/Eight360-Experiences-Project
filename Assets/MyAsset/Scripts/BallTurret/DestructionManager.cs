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
    public Vector2 targetSpeedRange;

    public GameObject marker;

    public float minScale = 0.5f;
    public float maxScale = 2f;
    float elapsed = 0f;
    public float deadTime = 5f;

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
        foreach (GameObject GO in targets)
        {
            // Target
            // Move our position a step closer to the target.
            GO.GetComponent<Target>().MoveToDestination();
            
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
        Vector3 destCenter = DestPoint.position;

        for (int i = 0; i < amountToSpawn; i++)
        {
            // Spawn
            Vector3 spawnPos = RandomPointInAnnulus(spawnCenter, 10f, SpawnPointRadius);
            //Vector3 spawnPos = spawnCenter;
            //Quaternion spawnRot = Quaternion.FromToRotation(Vector3.forward, spawnCenter);
            GameObject target = Instantiate(spawnGO, new Vector3 (spawnPos.x, spawnCenter.y, spawnPos.z), Quaternion.identity);

            //Scaling (Object)
            float newScale = Random.Range(minScale, maxScale);
            target.transform.localScale = new Vector3(newScale, newScale, newScale);

            //Scaling (Trail)
            var PSmain = target.GetComponentInChildren<ParticleSystem>().main;
            PSmain.startSize = newScale;

            //Parenting
            target.transform.SetParent(SpawnPoint);
            targets.Add(target);

            // Destination Point
            Vector3 destPos = RandomPointInAnnulus(destCenter, DestMinRadius, DestMaxRadius);
            target.GetComponent<Target>().SetDestination(new Vector3(destPos.x , destCenter.y, destPos.z));

            // Set speed
            target.GetComponent<Target>().speed = Random.Range(targetSpeedRange.x, targetSpeedRange.y);

            //Waypoint Script
            //TargetWaypoint newMarker = Instantiate(marker, spawnPos, Quaternion.identity).GetComponent<TargetWaypoint>();
            //newMarker.target = target.transform;
            //newMarker.cam = cam;
            //newMarker.transform.SetParent(MarkerCanvas);
        }

        
    }

    Vector3 RandomCircle(Vector3 center, float _MinRadius, float _MaxRadius)
    {
        // V1
        float ang = Random.Range(0, 360);
        Vector3 pos;
        pos.x = center.x + _MaxRadius * Mathf.Sin(ang * Mathf.Deg2Rad);
        //if (pos.x < center.x + _MinRadius)
        //{
        //    pos.x = center.x + _MinRadius;
        //}
        pos.y = center.y + _MaxRadius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;

        // V2
        //Vector3 newPos = center + (Vector3)Random.insideUnitCircle * _MaxRadius;
        //if(newPos.x < _MinRadius)
        //{
        //    newPos.x = _MinRadius;
        //}

        //if (newPos.y < _MinRadius)
        //{
        //    newPos.y = _MinRadius;
        //}

        //return newPos;

        // V3

    }


    public Vector3 RandomPointInAnnulus(Vector3 origin, float minRadius, float maxRadius)
    {

        var randomDirection = (Random.insideUnitSphere).normalized;

        var randomDistance = Random.Range(minRadius, maxRadius);

        var point = origin + randomDirection * randomDistance;

        //Vector3 newPos = new Vector3(point.x, origin.y, point.y);

        return point;
    }


    //[System.Serializable]
    //public struct SpawnedTarget
    //{
    //    public GameObject targetPrefab;
    //    public float size;
    //    public Vector3 destPos;

    //    public SpawnedTarget(GameObject _prefab, float _size, Vector3 _destPos)
    //    {
    //        targetPrefab = _prefab;
    //        size = _size;
    //        destPos = _destPos;
    //    }


    //}

}
