using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject gameEndText;
    [SerializeField]
    Vector3 sneakSpawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "FoodConsumeTrigger")
        { 
            gameEndText.SetActive(true);
            SneakControl sneak = FindObjectOfType<SneakControl>();
            sneak.transform.position = sneakSpawnPoint;
        }
    }
}
