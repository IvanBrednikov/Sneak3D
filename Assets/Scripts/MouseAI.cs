using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseAI : MonoBehaviour
{
    Vector3 destination;
    NavMeshAgent agent;
    bool isInDanger;
    public float distancePlayerAvoid = 8f;
    public float distanceIdleMove = 0.2f;

    public float maxTimeBetweenIndleMove = 2f;
    public float minTimeBetweenIndleMove = 0.3f;
    float timer;

    private void Start()
    {
        destination = transform.position;
        timer = maxTimeBetweenIndleMove;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.destination = destination;
        if (isInDanger && agent.velocity == Vector3.zero)
        {
            isInDanger = false;
        }
        
        

        //спокойное перемещение мыши
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = Random.Range(0.3f, 2f);
            if(!isInDanger)
                destination = FindIdleDestination();
        }
    }

    Vector3 FindIdleDestination()
    {
        Vector3 result;

        Vector3 direction = Vector3.forward;
        float angle = Random.Range(-90f, 90f);
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        direction = rotation * direction;
        Vector3 relativePosition = direction.normalized * distanceIdleMove;
        
        result = transform.TransformPoint(relativePosition);

        return result;
    }

    Vector3 FindNonDangerDestination(Vector3 dangerPosition)
    {
        Vector3 result;
        int directonRandomizer = Random.Range(0, 3);
        Vector3 direction = transform.position - dangerPosition;
        float angle = 0f;

        switch(directonRandomizer)
        {
            //вправо со стороны игрока
            case 0:
                angle = 45;
                break;
            //влево со стороны игрока
            case 1:
                angle = -45;
                break;
        }

        if(angle != 0)
        {
            //поворот вектора direction
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            direction = rotation * direction;
        }

        Vector3 relativePosition = direction.normalized * distancePlayerAvoid;

        result = transform.TransformPoint(relativePosition);

        return result;
    }

    public void DangerReact(Vector3 dangerPosition)
    {
        if(!isInDanger)
        {
            isInDanger = true;
            destination = FindNonDangerDestination(dangerPosition);
        }
    }
}
