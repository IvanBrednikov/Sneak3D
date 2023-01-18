using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringControl : MonoBehaviour
{
    [SerializeField]
    bool doForce;
    [SerializeField]
    float force;
    SpringJoint spring;
    void Start()
    {
        spring = GetComponent<SpringJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
