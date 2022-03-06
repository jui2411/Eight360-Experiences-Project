using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParent : MonoBehaviour
{

    private Transform originalParent = null;
    private Vector3 originalPos;
    private Vector3 originalRot;

    private void Awake()
    {
        originalParent = transform.parent;
        originalPos = transform.localPosition;
        originalRot = transform.localEulerAngles;
    }


    public void SetNewParent(Transform attachPoint) 
    {
        transform.parent = attachPoint;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = originalRot;
    }

    public void SetOriginalParent()
    {
        transform.parent = originalParent;
        transform.localPosition = originalPos;
        transform.localEulerAngles = originalRot;
    }
}
