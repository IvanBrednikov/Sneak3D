using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBlockControl : MonoBehaviour
{
    [SerializeField]
    int state = 0; //0 - apple, 1 - ?, 2 - sneak, 3 - tree
    [SerializeField]
    float startAngleOffset = 0;
    [SerializeField]
    float rollSpeed = 60f; //degrees per sec
    int maxVariants = 4;
    float rollAngle = 90f;

    bool isRolling;
    float rollTarget;

    [SerializeField]
    AudioSource audioRoll;

    public void SetState(int newState)
    {
        if(state != newState)
        {
            int virtState = newState;
            if (newState < state)
                virtState = maxVariants + newState;
            int diff = virtState - state;

            Debug.Log(name + " diff = " + diff);
            for (int i = 0; i < diff; i++)
                PlusState();
        }
    }

    public void PlusState()
    {
        state++;
        if (state == maxVariants)
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
        if (state == 3)
            rollTarget = rollAngle * 3;

        audioRoll.Play();
    }

    private void Update()
    {
        if (isRolling)
        {
            float rollAngleOnFrame = rollSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.AngleAxis(rollAngleOnFrame, Vector3.forward);
            transform.rotation *= rotation;

            if (transform.rotation.eulerAngles.z >= rollTarget - 1 && transform.rotation.eulerAngles.z <= rollTarget + 1)
            {
                isRolling = false;
                audioRoll.Stop();
            }
        }
    }

    public int State { get { return state; } }
}
