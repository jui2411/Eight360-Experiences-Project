using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public enum Direction { X, Y, Z , nX, nY, nZ};
    public Direction spawnDir;

    public CinemachineDollyCart dollyCart;

    public float spawnDist = 30f;
    public float tileLength = 30f;

    //private variables
    private Transform holder;

    //Dolly Variables
    private CinemachineSmoothPath smoothPath;
    private int nWaypoint = 5;
    private int iWaypoint = 0;

    private void Awake()
    {
        GenerateHolder();
        SetupDollyPath();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnTile(0);
        SpawnTile(0);
        SpawnTile(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateHolder()
    {
        //Create Holder
        string holderName = "Generated Trench";
        if (transform.Find(holderName)) DestroyImmediate(transform.Find(holderName).gameObject);

        holder = new GameObject(holderName).transform;
        holder.transform.SetParent(transform);
    }

    public void SpawnTile(int tileIndex)
    {

        Vector3 dir = Vector3.zero;
        if (spawnDir == Direction.X) dir = Vector3.right;
        if (spawnDir == Direction.Y) dir = Vector3.up;
        if (spawnDir == Direction.Z) dir = Vector3.forward;
        if (spawnDir == Direction.nX) dir = Vector3.left;
        if (spawnDir == Direction.nY) dir = Vector3.down;
        if (spawnDir == Direction.nZ) dir = Vector3.back;

        GameObject newTile;
        newTile = Instantiate(tilePrefabs[tileIndex], transform.position + (dir * spawnDist), transform.rotation);
        AddDollyTrackWaypoint(transform.position + (dir * spawnDist));
        spawnDist += tileLength;
    }

    private void SetupDollyPath()
    {
        smoothPath = new GameObject("Generated DollyTrack").AddComponent<CinemachineSmoothPath>();
        smoothPath.m_Looped = false;
        smoothPath.transform.SetParent(transform);
        smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[nWaypoint];

        if (dollyCart)
        {
            dollyCart.m_Path = smoothPath;
        } else
        {
            Debug.LogError("Path Not Found");
        }

        smoothPath.m_Waypoints[iWaypoint] = new CinemachineSmoothPath.Waypoint();

    }

    private void AddDollyTrackWaypoint(Vector3 _pos)
    {
        //smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[nWaypoint + 1];
        smoothPath.m_Waypoints[iWaypoint].position = _pos;
        iWaypoint++;
    }
}
