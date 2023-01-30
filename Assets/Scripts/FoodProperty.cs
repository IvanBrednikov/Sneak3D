using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProperty : MonoBehaviour
{
    public int foodValue;

    public void FoodConsume(UpgradesPanelHandler panel)
    {
        panel.FoodAdd(foodValue);
        Destroy(gameObject);
    }
}
