using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    UpgradesPanelHandler upgradesPanel;
    AudioSource eatSound;
    [SerializeField]
    AudioSource idleSound;

    float delayTimer;

    private void Start()
    {
        upgradesPanel = FindObjectOfType<UpgradesPanelHandler>(true);
        eatSound = GetComponent<AudioSource>();
        delayTimer = Random.Range(15, 30);
    }

    private void Update()
    {
        delayTimer -= Time.deltaTime;
        if(delayTimer <= 0)
        {
            delayTimer = Random.Range(15, 30);
            idleSound.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FoodProperty food = other.GetComponent<FoodProperty>();
        if(food != null)
        {
            food.FoodConsume(upgradesPanel);
            eatSound.Play();
        }
    }
}
