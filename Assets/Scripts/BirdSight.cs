using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSight : MonoBehaviour
{
    [SerializeField]
    BirdAI bird;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            bird.DangerReact(other.transform.position);
        }
    }
}
