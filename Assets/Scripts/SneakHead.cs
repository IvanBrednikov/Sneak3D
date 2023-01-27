using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakHead : MonoBehaviour
{
    public Collider lastHeadCollision;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enviroment")
            lastHeadCollision = collision.collider;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Enviroment")
            lastHeadCollision = collision.collider;
    }

    private void OnCollisionExit(Collision collision)
    {
        lastHeadCollision = null;
    }
}
