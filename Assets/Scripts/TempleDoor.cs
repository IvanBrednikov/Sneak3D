using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleDoor : MonoBehaviour
{
    [SerializeField]
    PuzzleDoor door;
    [SerializeField]
    TempleDoorPlate plate1;
    [SerializeField]
    TempleDoorPlate plate2;
    [SerializeField]
    TempleDoorPlate plate3;

    void Update()
    {
        if(plate1.isActive && plate2.isActive && plate3.isActive && !door.isOpened)
        {
            door.Open();
        }
    }
}
