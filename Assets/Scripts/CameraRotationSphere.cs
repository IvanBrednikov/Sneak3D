using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationSphere : MonoBehaviour
{
    BoxCollider boxCollider;
    SneakCamera sneakCamera;

    public Collider otherCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        sneakCamera = GetComponentInChildren<SneakCamera>();
    }

    private void Update()
    {
        boxCollider.center = new Vector3(0, 0, -sneakCamera.distance);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name != "Terrain")
            otherCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        otherCollider = null;
    }
}
