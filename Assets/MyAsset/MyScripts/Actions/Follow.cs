using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Transform target;
    public Vector2 planeSize;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }

    [ContextMenu("Apply Changes")]
    void ChangeSize()
    {
        transform.localScale = new Vector3(planeSize.x, 0, planeSize.y);
    }
}
