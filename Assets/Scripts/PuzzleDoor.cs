using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    bool isOpening;
    public bool isOpened;
    [SerializeField]
    float targetLevel;
    [SerializeField]
    float speed = 2f;

    [SerializeField]
    AudioSource stoneAudio;

    void Update()
    {
        if(isOpening)
        {
            Vector3 position = transform.position;
            position.y -= speed * Time.deltaTime;
            transform.position = position;

            if(transform.position.y <= targetLevel)
            {
                isOpened = true;
                isOpening = false;
                stoneAudio.Stop();
            }
        }
        
    }

    public void Open()
    {
        if(!isOpened)
        {
            isOpening = true;
            stoneAudio.Play();
        }
    }
}
