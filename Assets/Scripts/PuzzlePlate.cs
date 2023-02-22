using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePlate : MonoBehaviour
{
    [SerializeField]
    PlateControl plate;
    [SerializeField]
    PuzzleController puzzle;
    [SerializeField]
    int plateNumb = 0;

    private void Start()
    {
        switch(plateNumb)
        {
            case 0:
                plate.OnPlatePress += Plate_OnPlate1Press;
                break;
            case 1:
                plate.OnPlatePress += Plate_OnPlate2Press;
                break;
            case 2:
                plate.OnPlatePress += Plate_OnPlate3Press;
                break;
            case 3:
                plate.OnPlatePress += Plate_OnPlate4Press;
                break;
            default:
                plate.OnPlatePress += Plate_OnPlate1Press;
                break;
        }
    }

    private void Plate_OnPlate4Press(object sender, System.EventArgs e)
    {
        puzzle.Plate4Press();
    }

    private void Plate_OnPlate3Press(object sender, System.EventArgs e)
    {
        puzzle.Plate3Press();
    }

    private void Plate_OnPlate2Press(object sender, System.EventArgs e)
    {
        puzzle.Plate2Press();
    }

    private void Plate_OnPlate1Press(object sender, System.EventArgs e)
    {
        puzzle.Plate1Press();
    }
}
