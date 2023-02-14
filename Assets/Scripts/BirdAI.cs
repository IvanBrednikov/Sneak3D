using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BirdAI : MonoBehaviour
{
    Vector3 destination;
    NavMeshAgent agent;
    [SerializeField]
    Animator animator;
    [SerializeField]
    AudioSource audio;

    [SerializeField]
    float flyTime = 10f;
    [SerializeField]
    float walkTime = 10f;
    [SerializeField]
    float transitionTime = 2f;
    [SerializeField]
    float maxTimeBetweenWalk = 2f;
    [SerializeField]
    float minTimeBetweenWalk = 0.2f;
    [SerializeField]
    float walkDistance = 0.2f;
    [SerializeField]
    float flightDistance = 2f;
    [SerializeField]
    float skyGlobalHeight = 5f;
    [SerializeField]
    GameObject[] landingZones;
    [SerializeField]
    float transitionAngle = 45f;
    [SerializeField]
    float newDestinationFlightDistance = 1f;
    [SerializeField]
    int maxCallsOnPointCheck = 10;

    enum BirdMovingState { IsWalking = 0, IsTakingOff = 1, IsFlying = 2, IsLanding = 3 }
    BirdMovingState birdMovingState;
    Vector3 transitionStartPosition;
    Vector3 movingLastPosition;

    float movingStateTimer;
    float walkDelayTimer;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        birdMovingState = BirdMovingState.IsWalking;
        movingStateTimer = walkTime;
        walkDelayTimer = Random.Range(minTimeBetweenWalk, maxTimeBetweenWalk);
    }

    void Update()
    {
        movingStateTimer -= Time.deltaTime;
        walkDelayTimer -= Time.deltaTime;

        //переключение состояния птицы между наземным, полётом, взлётом, приземлением
        if (movingStateTimer <= 0)
        {
            switch (birdMovingState)
            {
                case BirdMovingState.IsWalking:
                    try
                    {
                        destination = FindTakeOffPosition();
                        birdMovingState = BirdMovingState.IsTakingOff;
                        movingStateTimer = transitionTime;
                        agent.enabled = false;
                        transitionStartPosition = transform.position;
                        audio.Play();

                    }
                    catch (NavMeshPointNotFound exception)
                    {
                        destination = FindWalkingPosition();
                        movingStateTimer = walkTime;
                    }
                    break;
                case BirdMovingState.IsTakingOff:
                    birdMovingState = BirdMovingState.IsFlying;
                    movingStateTimer = flyTime;
                    agent.enabled = true;
                    destination = FindFlightPosition();
                    break;
                case BirdMovingState.IsFlying:
                    try
                    {
                        destination = FindLandPosition();
                        birdMovingState = BirdMovingState.IsLanding;
                        movingStateTimer = transitionTime;
                        agent.enabled = false;
                        transitionStartPosition = transform.position;
                    }
                    catch (NavMeshPointNotFound exception)
                    {
                        destination = FindFlightPosition();
                        movingStateTimer = flyTime;
                    }
                    break;
                case BirdMovingState.IsLanding:
                    birdMovingState = BirdMovingState.IsWalking;
                    movingStateTimer = walkTime;
                    agent.enabled = true;
                    destination = FindWalkingPosition();
                    audio.Stop();
                    break;
            }
        }

        //если таймер задержки перемещения истёк то задать новую точку
        if (walkDelayTimer <= 0 && 
            birdMovingState == BirdMovingState.IsWalking)
        {
            destination = FindWalkingPosition();
            walkDelayTimer = Random.Range(minTimeBetweenWalk, maxTimeBetweenWalk);
        }

        //если сейчас полёт и растояние до точки назначения уже близко то изменить точку назначения
        if (birdMovingState == BirdMovingState.IsFlying &&
           Vector3.Distance(transform.position, destination)
           <= newDestinationFlightDistance)
        {
            movingLastPosition = transform.position;
            destination = FindFlightPosition();
        }

        //обработка полёта и ходьбы
        if (birdMovingState == BirdMovingState.IsWalking || birdMovingState == BirdMovingState.IsFlying)
            agent.destination = destination;


        //обработка взлёта и приземления
        if (birdMovingState == BirdMovingState.IsTakingOff ||
            birdMovingState == BirdMovingState.IsLanding)
        {
            Vector3 speedVector = (destination - transitionStartPosition) / transitionTime;
            Vector3 translation = speedVector * Time.deltaTime;
            transform.position += translation;
        }

        bool isFlying = 
            birdMovingState == BirdMovingState.IsFlying ||
            birdMovingState == BirdMovingState.IsTakingOff ||
            birdMovingState == BirdMovingState.IsLanding;
        bool isWalking =
            birdMovingState == BirdMovingState.IsWalking &&
            agent.velocity.magnitude > 0f;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isFlying", isFlying);
    }

    Vector3 FindWalkingPosition()
    {
        Vector3 result;

        Vector3 direction = Vector3.forward;
        float angle = Random.Range(-90f, 90f);
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        direction = rotation * direction;
        Vector3 relativePosition = direction.normalized * walkDistance;


        result = transform.TransformPoint(relativePosition);

        return result;
    }

    Vector3 FindTakeOffPosition()
    {
        Vector3 result;
        bool positionFound = false;

        //выбор направления
        Quaternion rotation = Quaternion.AngleAxis(transitionAngle, Vector3.right);
        Vector3 direction = rotation * Vector3.up;

        //расчёт длины пути
        float relativeHeight = skyGlobalHeight - transform.position.y;
        float pathLength = relativeHeight / Mathf.Cos(transitionAngle * Mathf.Deg2Rad);

        result = transform.TransformPoint(direction.normalized * pathLength);

        //проверка на попадание точки на navMesh
        positionFound = CheckNavMeshPoint(result);

        if (!positionFound)
        {
            NavMeshPointNotFound exc = new NavMeshPointNotFound("TakeOff position not found");
            throw exc;
        }
        return result;
    }

    Vector3 FindTakeOffInDangerPosition()
    {
        Vector3 result = Vector3.zero;
        bool positionFound = false;
        int calls = 0;

        while (!positionFound)
        {
            //выбор направления
            Quaternion rotation = Quaternion.AngleAxis(transitionAngle, Vector3.right);
            float angle = Random.Range(-180, 181);
            Quaternion rotationY = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 direction = rotationY * rotation * Vector3.up;

            //расчёт длины путя
            float relativeHeight = skyGlobalHeight - transform.position.y;
            float pathLength = relativeHeight / Mathf.Cos(transitionAngle * Mathf.Deg2Rad);

            result = transform.TransformPoint(direction.normalized * pathLength);

            //проверка на попадание точки на navMesh
            positionFound = CheckNavMeshPoint(result);

            calls++;
            if (calls > maxCallsOnPointCheck)
            {
                NavMeshPointNotFound exc = new NavMeshPointNotFound("InDangerTakeOff position not found");
                throw exc;
            }
        }
        
        return result;
    }

    Vector3 FindLandPosition()
    {
        int calls = 0;

        Vector3 result = Vector3.zero;
        bool positionFound = false;
        bool positionFindInProcess = true;

        while(positionFindInProcess)
        {
            //выбор направления
            Quaternion rotation = Quaternion.AngleAxis(-transitionAngle, Vector3.right);

            //если первый вариант пути не получился
            if (calls > 0)
            {
                float randomAngle = Random.Range(-180, 181);
                Quaternion rotation2 = Quaternion.AngleAxis(randomAngle, Vector3.up);
                rotation *= rotation2;
            }
            Vector3 direction = rotation * Vector3.down;

            //проверка направления на попадание по земле
            float rayLength = 100f;
            bool landZoneHitted = false;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.TransformDirection(direction), rayLength);
            for(int i = 0; i < hits.Length; i++)
                for(int j = 0; j < landingZones.Length; j++)
                    if (hits[i].collider.name == landingZones[j].name)
                    {
                        landZoneHitted = true;
                        break;
                    }
            
            for(int i = 0; i < hits.Length; i++)
            {
                if (CheckNavMeshPoint(hits[i].point) && hits[i].collider.name == "Terrain" && landZoneHitted)
                {
                    positionFound = true;
                    positionFindInProcess = false;
                    result = hits[i].point;
                    break;
                }
            }

            calls++;
            if(calls > maxCallsOnPointCheck)
            {
                positionFindInProcess = false;
            }
        }
        
        //проверка на нахождение точки
        if (!positionFound)
        {
            NavMeshPointNotFound exc = new NavMeshPointNotFound("Land position not found");
            throw exc;
        }
        return result;
    }

    Vector3 FindFlightPosition()
    {
        Vector3 result = movingLastPosition;
        int calls = 0;
        bool posFindning = true;

        while(posFindning)
        {
            Vector3 direction = Vector3.forward;
            float angle = Random.Range(-120f, 120f);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            direction = rotation * direction;
            Vector3 relativePosition = direction.normalized * flightDistance;

            result = transform.TransformPoint(relativePosition);

            calls++;
            if(CheckNavMeshPoint(result))
            {
                posFindning = false;
            }

            if(calls > maxCallsOnPointCheck)
            {
                posFindning = false;
            }
        }
        
        return result;
    }

    public void DangerReact(Vector3 dangerPosition)
    {
        try
        {
            destination = FindTakeOffPosition();
            birdMovingState = BirdMovingState.IsTakingOff;
            movingStateTimer = transitionTime;
            agent.enabled = false;
            transitionStartPosition = transform.position;
            audio.Play();
        }
        catch (NavMeshPointNotFound exception)
        {
            movingStateTimer = walkTime;
        }
    }

    //проверяет доступность точки на navMesh
    bool CheckNavMeshPoint(Vector3 point)
    {
        bool result;
        NavMeshHit hit = new NavMeshHit();
        result = NavMesh.SamplePosition(point, out hit, 1f, NavMesh.AllAreas);

        return result;
    }

}

public class NavMeshPointNotFound : System.Exception
{
    public NavMeshPointNotFound(string message) : base(message)
    {
        Debug.LogException(this);
    }
}

