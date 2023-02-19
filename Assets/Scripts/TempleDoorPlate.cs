using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleDoorPlate : MonoBehaviour
{
    [SerializeField]
    PlateControl plate;
    public bool isActive;
    [SerializeField]
    MeshRenderer lamp;
    [SerializeField]
    Material lampActive;

    private void Start()
    {
        plate.OnPlatePress += Plate_OnPlatePress;
    }

    private void Plate_OnPlatePress(object sender, System.EventArgs e)
    {
        isActive = true;
        lamp.material = lampActive;
    }
}
