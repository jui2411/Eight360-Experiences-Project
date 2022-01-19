using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachPointCreator : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        CreateAttach();
    }

    private void CreateAttach()
    {
        if(TryGetComponent(out XRGrabInteractable interactable))
        {
            GameObject attachObject = new GameObject("attach");

            attachObject.transform.SetParent(transform);
            attachObject.transform.localPosition = Vector3.zero;
            attachObject.transform.localRotation = Quaternion.identity;

            interactable.attachTransform = attachObject.transform;
        }
    }
}
