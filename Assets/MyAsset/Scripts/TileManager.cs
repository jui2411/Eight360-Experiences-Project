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
    public float tileLength = 30f;
    public int iteration = 0;
    public int objectsInFront = 3;

    //Waypoint Variables
    public Vector3 waypointOffset;
    public bool reversedTrack = false;

    // Object Pooling
    public int nObjectPooling = 10;

    //private variables
    private Transform holder;
    private Transform usedTileholder;
    private Transform unusedTileholder;
    private float spawnDist = 0f;

    //Dolly Variables
    private CinemachineSmoothPath smoothPath;
    private int iWaypoint = 0;

    Vector3 dir = Vector3.zero;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateHolder();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float milestone = 0;
        if(dollyCart.m_Position >= milestone)
        {
            BuildTile(0);
            milestone++;
        }
    }

    private void GenerateHolder()
    {
        //Create Holder
        string holderName = "Generated Trench";
        if (transform.Find(holderName)) DestroyImmediate(transform.Find(holderName).gameObject);

        holder = new GameObject(holderName).transform;
        holder.transform.SetParent(transform);
    }



    public void BuildTile(int tileIndex)
    {
        GameObject newTile;
        newTile = Instantiate(tilePrefabs[0], transform.position + (dir * spawnDist), transform.rotation);
        newTile.transform.SetParent(holder.transform);

    }

    private void SetupDollyPath()
    {
        string trackName = "Generated DollyTrack";
        if (transform.Find(trackName)) DestroyImmediate(transform.Find(trackName).gameObject);
        smoothPath = new GameObject(trackName).AddComponent<CinemachineSmoothPath>();
        smoothPath.m_Looped = false;
        smoothPath.transform.SetParent(transform);
        smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[objectsInFront];

        if (dollyCart)
        {
            dollyCart.m_Path = smoothPath;
        } else
        {
            Debug.LogError("Path Not Found");
        }

        iWaypoint = 0;
        smoothPath.m_Waypoints[iWaypoint] = new CinemachineSmoothPath.Waypoint();
    }

    private void AddDollyTrackWaypoint(Vector3 _pos)
    {
        smoothPath.m_Waypoints[iWaypoint].position = _pos + waypointOffset;
        iWaypoint++;

    }

    void CreatePath()
    {
        BuildTile(0);
        AddDollyTrackWaypoint(transform.position + (dir * spawnDist));
    }

    public void TestGenerateTiles()
    {

        if (spawnDir == Direction.X) dir = Vector3.right;
        if (spawnDir == Direction.Y) dir = Vector3.up;
        if (spawnDir == Direction.Z) dir = Vector3.forward;
        if (spawnDir == Direction.nX) dir = Vector3.left;
        if (spawnDir == Direction.nY) dir = Vector3.down;
        if (spawnDir == Direction.nZ) dir = Vector3.back;

        //Calculations
        spawnDist = tileLength * iteration;

        GenerateHolder();
        SetupDollyPath();

        for (int i = 0; i < objectsInFront; i++)
        {
            CreatePath();
            if (!reversedTrack)
            {
                spawnDist += tileLength;
            } else
            {
                spawnDist -= tileLength;
            }
        }
    }

    public void CreatePool()
    {
        // Spawn TIles
        for (int i = 0; i < nObjectPooling; i++)
        {
            //GameObject newTile;
            //newTile = Instantiate(tilePrefabs[0], transform.position + (dir * spawnDist), transform.rotation);
            //newTile.transform.SetParent(holder.transform);

            //Create used Holder
            string usedHolderName = "Used";
            if (transform.Find(usedHolderName)) DestroyImmediate(transform.Find(usedHolderName).gameObject);

            usedTileholder = new GameObject(usedHolderName).transform;
            usedTileholder.transform.SetParent(holder.transform);

            //Create unused Holder
            string unusedHolderName = "Unused";
            if (transform.Find(unusedHolderName)) DestroyImmediate(transform.Find(unusedHolderName).gameObject);

            unusedTileholder = new GameObject(unusedHolderName).transform;
            unusedTileholder.transform.SetParent(holder.transform);
        }
            
        
    }
}
