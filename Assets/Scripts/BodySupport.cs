using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySupport : MonoBehaviour
{
    Collider lastCollision;

    public void SetJoint()
    {
        if (lastCollision != null)
            gameObject.AddComponent<FixedJoint>();
    }

    public void DestroyJoint()
    {
        FixedJoint joint = GetComponent<FixedJoint>();
        if (joint != null)
            Destroy(joint);
    }

    public bool IsCollising {get {return lastCollision != null;} }

    public bool JointIsSet { get { return GetComponent<FixedJoint>() != null; } }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enviroment")
            lastCollision = collision.collider;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Enviroment")
            lastCollision = collision.collider;
    }

    private void OnCollisionExit(Collision collision)
    {
        lastCollision = null;
    }
}
