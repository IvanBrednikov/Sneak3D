using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    [SerializeField]
    RollingBlockControl[] rollBlocks;
    int condition = 2;
    [SerializeField]
    PuzzleDoor door;

    private void Start()
    {
        GeneratePuzzle();
    }

    private void Update()
    {
        if(CheckCondition() && !door.isOpened)
        {
            door.Open();
        }
    }

    public bool CheckCondition()
    {
        bool result = true;

        for (int i = 0; i < rollBlocks.Length; i++)
            if (rollBlocks[i].State != condition)
                result = false;

        return result;
    }

    void GeneratePuzzle()
    {
        int[] comb1 = { 1, 0, 2, 1 };
        int[] comb2 = { 1, 0, 1, 0 };
        int[] comb3 = { 0, 2, 1, 1 };

        int[] combination = comb1;

        switch (Random.Range(0, 3))
        {
            case 0:
                combination = comb1;
                break;
            case 1:
                combination = comb2;
                break;
            case 2:
                combination = comb3;
                break;
        }

        for (int i = 0; i < rollBlocks.Length; i++)
            rollBlocks[i].SetState(combination[i]);
    }

    public void Plate1Press()
    {
        rollBlocks[0].PlusState();
        rollBlocks[1].PlusState();
        rollBlocks[2].PlusState();
    }

    public void Plate2Press()
    {
        rollBlocks[1].PlusState();
        rollBlocks[2].PlusState();
    }

    public void Plate3Press()
    {
        rollBlocks[2].PlusState();
        rollBlocks[3].PlusState();
    }
}
