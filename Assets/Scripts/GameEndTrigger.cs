using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject gameEndText;
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "FoodConsumeTrigger")
            gameEndText.SetActive(true);
    }
}
