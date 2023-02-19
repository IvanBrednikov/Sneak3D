using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBlockControl : MonoBehaviour
{
    [SerializeField]
    int state = 0; //0 - apple, 1 - tree, 2 - sneak
    float rollAngle = 120f;
    [SerializeField]
    float startAngleOffset = 0;
    [SerializeField]
    float rollSpeed = 60f; //degrees per sec

    bool isRolling;
    float rollTarget;

    [SerializeField]
    AudioSource audioRoll;

    public void SetState(int newState)
    {
        if (state == 0)
        {
            if (newState == 1)
                PlusState();
            else
            if (newState == 2)
            {
                PlusState();
                PlusState();
            }
        }
        else
        if (state == 1)
        {
            if (newState == 0)
            {
                PlusState();
                PlusState();
            }
            else
            if (newState == 2)
            {
                PlusState();
            }
        }
        else
        if (state == 2)
        {
            if (newState == 0)
            {
                PlusState();
            }
            else
            if (newState == 1)
            {
                PlusState();
                PlusState();
            }
        }
    }

    public void PlusState()
    {
        state++;
        if (state == 3)
            state = 0;
        RollBlock();
    }

    void RollBlock()
    {
        isRolling = true;
        if (state == 0)
            rollTarget = 359;
        if (state == 1)
            rollTarget = rollAngle;
        if (state == 2)
            rollTarget = rollAngle * 2;

        audioRoll.Play();
    }

    private void Update()
    {
        if (isRolling)
        {
            float rollAngleOnFrame = rollSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.AngleAxis(rollAngleOnFrame, Vector3.forward);
            transform.rotation *= rotation;

            if (transform.rotation.eulerAngles.z >= rollTarget - 3 && transform.rotation.eulerAngles.z <= rollTarget + 3)
            {
                isRolling = false;
                audioRoll.Stop();
            }
        }
    }

    public int State { get { return state; } }
}
