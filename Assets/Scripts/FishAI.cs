using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour
{
    [SerializeField]
    float forceValue = 20f;
    [SerializeField]
    float angle = 45f;
    Rigidbody rb;
    public Transform fishSpawner;
    [SerializeField]
    GameObject mesh;

    [SerializeField]
    AudioSource fishIntoWaterSound;
    [SerializeField]
    AudioSource fishOutOfWaterSound;
    bool fishIsInWater = true;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 direction = fishSpawner.position - transform.position;
        direction.y = 0f;

        //поворот наверх
        Quaternion rotation = Quaternion.AngleAxis(90, Vector3.up);
        Vector3 perp = rotation * direction;
        rotation = Quaternion.AngleAxis(-angle, perp);
        direction = rotation * direction;

        Vector3 force = direction.normalized * forceValue;
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    private void Update()
    {
        Vector3 viewPoint = transform.position + rb.velocity;
        transform.LookAt(viewPoint, Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WaterSoundLevel")
        {
            if (fishIsInWater)
            {
                fishIsInWater = false;
                fishOutOfWaterSound.Play();
            }
            else
            {
                fishIntoWaterSound.Play();
            }
        }
    }
}
