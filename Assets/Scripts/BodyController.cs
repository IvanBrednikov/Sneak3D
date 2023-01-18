using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    Rigidbody rb;
    ConfigurableJoint joint;
    [SerializeField]
    bool move;
    [SerializeField]
    float force;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (move)
        {
            Vector3 forceDirection = (joint.connectedBody.position - transform.position).normalized;
            rb.AddForce(forceDirection * force);
        }
    }
}
