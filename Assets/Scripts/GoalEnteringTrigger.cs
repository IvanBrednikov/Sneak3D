using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalEnteringTrigger : MonoBehaviour
{
    [SerializeField]
    GoalController goalController;

    private void Start()
    {
        goalController = FindObjectOfType<GoalController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "TreeTrigger")
        {
            goalController.TreeTriggerReact();
            goalController.treeTriggerActivated = true;
        }

        if(other.name == "TempleTrigger")
        {
            goalController.TempleTriggerReact();
        }

        if(other.name == "SecondTempleTrigger")
        {
            goalController.SecondTempleTriggerReact();
        }
    }
}
