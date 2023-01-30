using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    UpgradesPanelHandler upgradesPanel;
    private void Start()
    {
        upgradesPanel = FindObjectOfType<UpgradesPanelHandler>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        FoodProperty food = other.GetComponent<FoodProperty>();
        if(food != null)
        {
            food.FoodConsume(upgradesPanel);
        }
    }
}
