using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakControl : MonoBehaviour
{
    [SerializeField]
    Rigidbody sneakHead;
    [SerializeField]
    Rigidbody sneakTail;
    [SerializeField]
    List<Rigidbody> sneakBody;

    //дл€ перемещени€
    [SerializeField]
    float mouseSenseControlMove;
    [SerializeField]
    float headForce;
    [SerializeField]
    float forcePerBody;
    bool isMoving;
    float additionalMovingAngle;
    float _headForceControl;
    float _bodyForceControl;

    //камера
    [SerializeField]
    GameObject rotationSphere;

    //дл€ генерации змейки
    SneakUpgrades upgrades;
    [SerializeField]
    bool isGenerated;
    [SerializeField]
    int sneakSize;
    [SerializeField]
    GameObject bodyPrefab;
    [SerializeField]
    GameObject headPrefab;
    [SerializeField]
    GameObject tailPrefab;
    float bodiesInterval = 0.02f;
    float bodyZscale = 0.05f;
    [SerializeField]
    int bodyCameraCenter;
    float SpawnDistance
    {
        get 
        {
            return (bodiesInterval + bodyZscale - 0.001f);
        }
    }

    //дл€ механики перемещени€ змеи в воздухе
    [SerializeField]
    GameObject headRotationSphere;
    Vector3 targetHeadPosition; //позици€ в пространнстве к которому голова змеи стремитс€
    float supportLength; //изначальна€ длинна поднимаемой части змеи
    bool sneakHeadSpacing; //состо€ние механики
    [SerializeField]
    int bodySupportPoint;
    [SerializeField]
    GameObject bodySupportObject;
    [SerializeField]
    Material supportBodyMaterial;
    [SerializeField]
    Material defaultBodyMaterial;
    [SerializeField]
    Material defaultTailMaterial;
    [SerializeField]
    float headSpacingForce;
    [SerializeField]
    GameObject targetPositionDebug;
    [SerializeField]
    float mouseSenseControlSpacing;

    //дл€ механики взбирани€ на дерево
    bool headFixed;
    FixedJoint headJoint;
    Collider lastHeadCollision;
    
    void Start()
    {
        upgrades = GetComponent<SneakUpgrades>();

        if (isGenerated)
        {
            GenerateSneak();
        }
        else
        {
            List<Rigidbody> list = new List<Rigidbody>();
            Joint joint = sneakTail.GetComponent<Joint>();
            Rigidbody connectedBody = joint.connectedBody;
            do
            {
                list.Add(connectedBody);
                connectedBody = joint.connectedBody;
                joint = connectedBody.GetComponent<Joint>();
            }
            while (joint != null);

            list.Add(sneakTail);
            sneakBody = list;

            rotationSphere.GetComponentInChildren<SneakCamera>().viewObject = sneakBody[bodyCameraCenter].gameObject;

            SetNewSupportBody(bodySupportPoint);
        }
    }

    void Update()
    {
        isMoving = Input.GetButton("Fire1");
        if (isMoving)
        {
            float yMouseMove = Input.GetAxis("Mouse Y");
            if (yMouseMove > 0)
            {
                HeadForceControl = yMouseMove;
                BodyForceControl = 1f;
            }
            else
            if (yMouseMove < 0)
            {
                BodyForceControl = Mathf.Abs(yMouseMove);
                HeadForceControl = 0f;
            }
            else
            if (yMouseMove == 0)
            {
                HeadForceControl = 0f;
                BodyForceControl = 0f;
            }

            additionalMovingAngle = 90 * Input.GetAxis("Horizontal");
        }

        headRotationSphere.transform.position = bodySupportObject.transform.position;
        //sneakHead trigger
        BodySupport bodySupport = bodySupportObject.GetComponent<BodySupport>();
        if (sneakHeadSpacing != Input.GetButton("Fire2"))
        {
            if(sneakHeadSpacing)
            {
                bodySupport.DestroyJoint();
                targetPositionDebug.SetActive(false);
            }
            else
            {
                targetPositionDebug.SetActive(true);
            }
        }
        if (sneakHeadSpacing && !bodySupport.JointIsSet)
            bodySupport.SetJoint();

        sneakHeadSpacing = Input.GetButton("Fire2");

        if(sneakHeadSpacing)
        {
            float yMouseMove = Input.GetAxis("Mouse Y") * mouseSenseControlSpacing;
            float xMouseMove = Input.GetAxis("Mouse X") * mouseSenseControlSpacing;
            Vector3 rotation = headRotationSphere.transform.rotation.eulerAngles + new Vector3(yMouseMove, xMouseMove, 0f);
            //ограничени€ на вращение
            float virtualYmov = headRotationSphere.transform.rotation.eulerAngles.x + yMouseMove;
            if (virtualYmov > 80 && virtualYmov < 180)
            {
                rotation = new Vector3(80, xMouseMove + headRotationSphere.transform.rotation.eulerAngles.y, 0);
            }

            if (virtualYmov < 280 && virtualYmov >= 180)
            {
                rotation = new Vector3(280, xMouseMove + headRotationSphere.transform.rotation.eulerAngles.y, 0);
            }

            rotation.z = 0f;
            headRotationSphere.transform.rotation = Quaternion.Euler(rotation);
        }
        else
        {
            Vector3 rotation = bodySupportObject.transform.rotation.eulerAngles;
            rotation.z = 0f;
            headRotationSphere.transform.rotation = rotationSphere.transform.rotation;
        }

        if(Input.GetButtonDown("FixSneakHead"))
        {
            headFixed = !headFixed;
        }

        if (headFixed)
        {
            if(headJoint == null)
                SetHeadJoint();
        }
        else
        {
            DestroyHeadJoint();
        }

        if (Input.GetButtonDown("NextSupportBoddy"))
        {
            SetNewSupportBody(bodySupportPoint + 1);
        }

        if(Input.GetButtonDown("PreviousSupportBoddy"))
        {
            SetNewSupportBody(bodySupportPoint - 1);
        }
    }

    private void FixedUpdate()
    {
        //реализаци€ перемещени€ змеи по земле
        //направление движени€ соответсвующее направлению камере
        float angle = rotationSphere.transform.rotation.eulerAngles.y + additionalMovingAngle;
        angle = angle % 360;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 moveDirection = rotation * Vector3.forward;

        if(isMoving)
        {
            //движение головы
            sneakHead.AddForce(moveDirection * headForce * HeadForceControl);

            //движение позвонков
            for (int i = 0; i < sneakBody.Count; i++)
            {
                float bodySpring; //процентное сжатие между позвонками
                //определение степени сжати€
                if (i > 0)
                    bodySpring = (Vector3.Distance(sneakBody[i].transform.position, sneakBody[i - 1].transform.position) - bodyZscale) / bodiesInterval;
                else
                {
                    bodySpring = (Vector3.Distance(sneakBody[i].transform.position, sneakHead.transform.position) - bodyZscale) / bodiesInterval;
                }
                bodySpring = 1 - bodySpring;
                
                Joint joint = sneakBody[i].GetComponent<Joint>();
                Vector3 forceDirection = (joint.connectedBody.transform.position - sneakBody[i].position).normalized;

                if ((0.98f > bodySpring && _headForceControl == 0) || _headForceControl > 0) //(если уровень сжати€ меньше 90% и нужно сжатие) или нужно разжатие
                {
                    //то двигаем позвонок
                    sneakBody[i].AddForce(forceDirection * forcePerBody * BodyForceControl);
                }
            }
        }

        //реализаци€ механики перемещени€ головы змеи в пространстве
        //просчЄт точки куда голова змеи должна стремитьс€
        targetHeadPosition = headRotationSphere.transform.TransformPoint(Vector3.forward * supportLength * 1.5f);
        targetPositionDebug.transform.position = targetHeadPosition;

        //выполнение перемещени€ в ту точку куда стремитьс€ голова
        if (sneakHeadSpacing)
        {
            Vector3 relativeForce = targetHeadPosition - sneakHead.transform.position;
            sneakHead.AddForce(relativeForce * headSpacingForce);
        }
    }

    private void SetNewSupportBody(int bodyIndex)
    {
        if (bodyIndex >= 0 && bodyIndex < sneakBody.Count)
        {
            MeshRenderer mesh;
            if (bodySupportObject != null)
            {
                Destroy(bodySupportObject.GetComponent<BodySupport>());
                mesh = bodySupportObject.GetComponent<MeshRenderer>();
                mesh.material = defaultBodyMaterial;
            }
            bodySupportPoint = bodyIndex;
            bodySupportObject = sneakBody[bodySupportPoint].gameObject;
            headRotationSphere.transform.position = bodySupportObject.transform.position;
            supportLength = Vector3.Distance(headRotationSphere.transform.position, sneakHead.transform.position);
            bodySupportObject.AddComponent<BodySupport>();
            mesh = bodySupportObject.GetComponent<MeshRenderer>();
            mesh.material = supportBodyMaterial;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enviroment")
            lastHeadCollision = collision.collider;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Enviroment")
            lastHeadCollision = collision.collider;
    }

    private void OnCollisionExit(Collision collision)
    {
        lastHeadCollision = null;
    }

    public void SetHeadJoint()
    {
        if (lastHeadCollision != null)
        {
            headJoint = gameObject.AddComponent<FixedJoint>();
            headJoint.enableCollision = true;
        }
    }

    public void DestroyHeadJoint()
    {
        if (headJoint != null)
            Destroy(headJoint);
    }

    private float HeadForceControl 
    {
        get => _headForceControl; 
        set 
        {
            if(value > 1f)
                value = 1f;

            if (value < 0f)
                value = 0f;
            _headForceControl = value;
        }
    }

    private float BodyForceControl
    {
        get => _bodyForceControl;
        set
        {
            if (value > 2f)
                value = 2f;

            if (value < 0f)
                value = 0f;
            _bodyForceControl = value;
        }
    }

    public void GenerateSneak()
    {
        List<Rigidbody> list = new List<Rigidbody>();
        //иниициализаци€ параметров змейки
        SneakUpgrades.LengthLevel parameters = upgrades.lengthLevels[upgrades.currentLengthLevel];
        forcePerBody = parameters.forcePerBody;
        headForce = parameters.headForce;
        headSpacingForce = parameters.headSpacingForce;
        sneakSize = parameters.bodiesCount;
        bodyZscale = parameters.bodyZScale;
        bodiesInterval = parameters.bodiesInterval;

        if (parameters.lengthLevel == 2)
        {
            bodyPrefab = upgrades.bodyPrefab2;
            headPrefab = upgrades.headPrefab2;
            tailPrefab = upgrades.tailPrefab2;
        }

        if (parameters.lengthLevel == 3)
        {
            bodyPrefab = upgrades.bodyPrefab3;
            headPrefab = upgrades.headPrefab3;
            tailPrefab = upgrades.tailPrefab3;
        }

        //создание головы
        GameObject head = Instantiate(headPrefab);
        head.transform.position = transform.position;
        head.transform.parent = transform;
        sneakHead = head.GetComponent<Rigidbody>();

        //создание тела
        for (int i = 0; i < sneakSize; i++)
        {
            GameObject newBody = Instantiate(bodyPrefab);
            newBody.transform.parent = transform;
            newBody.name = "Sneak Body " + i;
            Rigidbody newRb = newBody.GetComponent<Rigidbody>();
            Joint joint = newBody.GetComponent<Joint>();
            if (i == 0)
            {
                joint.connectedBody = sneakHead;
                newBody.transform.position = sneakHead.transform.position - new Vector3(0f, 0f, SpawnDistance);
            }
            else
            {
                joint.connectedBody = list[i - 1];
                newBody.transform.position = list[i - 1].transform.position - new Vector3(0f, 0f, SpawnDistance);
            }
            list.Add(newRb);
        }

        //присоединение хвоста
        GameObject tail = Instantiate(tailPrefab);
        tail.transform.parent = transform;
        sneakTail = tail.GetComponent<Rigidbody>();

        sneakTail.GetComponent<Joint>().connectedBody = list[list.Count - 1];
        sneakTail.transform.position = list[list.Count - 1].transform.position - new Vector3(0f, 0f, SpawnDistance);
        list.Add(sneakTail);
        sneakBody = list;

        //камера
        bodyCameraCenter = sneakSize / 2;
        rotationSphere.GetComponentInChildren<SneakCamera>().viewObject = sneakBody[bodyCameraCenter].gameObject;
        //позвонок дл€ опоры
        SetNewSupportBody(sneakSize / 2);
    }

    public void DestroySneak()
    {
        Destroy(sneakHead.gameObject);
        Destroy(sneakTail.gameObject);
        for(int i = 0; i < sneakSize; i++)
        {
            Destroy(sneakBody[i].gameObject);
        }
        sneakBody.Clear();
    }
}
