using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSightTrigger : MonoBehaviour
{
    [SerializeField]
    MouseAI mouse;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            mouse.DangerReact(other.transform.position);
        }
    }
}
