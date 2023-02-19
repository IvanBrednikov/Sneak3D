using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateControl : MonoBehaviour
{
    [SerializeField]
    Collider pressTrigger;
    [SerializeField]
    Collider unpressedTrigger;
    [SerializeField]
    bool isPressed;

    public event System.EventHandler OnPlatePress;

    private void OnTriggerEnter(Collider other)
    {
        if(other == pressTrigger && !isPressed)
        {
            isPressed = true;
            OnPlatePress(this, null);
        }
        else
        if (other == unpressedTrigger && isPressed)
        {
            isPressed = false;
        }
    }
}
