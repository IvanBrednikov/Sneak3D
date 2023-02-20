using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakControl : MonoBehaviour
{
    [SerializeField]
    UserInterface sneakInterface;
    [SerializeField]
    Rigidbody sneakHead;
    [SerializeField]
    Rigidbody sneakTail;
    [SerializeField]
    List<Rigidbody> sneakBody;

    //для перемещения
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

    //для генерации змейки
    [SerializeField]
    Transform firstSpawnPoint;
    [SerializeField]
    Transform secondSpawnPoint;
    [SerializeField]
    Transform respawnLevel;
    SneakUpgrades upgrades;
    [SerializeField]
    GameMenu gameMenu;
    [SerializeField]
    UpgradesPanelHandler panel;
    public Vector3 spawnPoint;
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

    //для механики перемещения змеи в воздухе
    public bool canStanding;
    [SerializeField]
    GameObject headRotationSphere;
    Vector3 targetHeadPosition; //позиция в пространнстве к которому голова змеи стремится
    float supportLength; //изначальная длинна поднимаемой части змеи
    bool sneakHeadSpacing; //состояние механики
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

    //для механики взбирания на дерево
    public bool canClimbing;
    SneakHead headCollisions;
    bool headFixed;
    FixedJoint headJoint;
    Collider lastHeadCollision;

    //для скина змейки
    [SerializeField]
    GameObject skinPrefab;
    GameObject skinObject;
    SkinnedMeshRenderer skinMesh;
    [SerializeField]
    bool useSkin = true;
    
    void Start()
    {
        spawnPoint = transform.position;
        upgrades = GetComponent<SneakUpgrades>();
        skinMesh = GetComponentInChildren<SkinnedMeshRenderer>();

        if (!gameMenu.isFirstPlay)
        {
            spawnPoint = gameMenu.playerSpawn;
            upgrades.SavedUpgradeApplyOnStart();
        }

        if (isGenerated)
        {
            GenerateSneak();
            if(!gameMenu.isFirstPlay)
            {
                SetMaterial(upgrades.savedSneakSkin);
            }
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
        if(!sneakInterface.UiActive)
        {
            isMoving = Input.GetButton("Fire1");
            if (isMoving)
            {
                float yMouseMove = Input.GetAxis("Mouse Y joyInverted"); //для геймпада и мыши
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
                if (sneakHeadSpacing)
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

            if (canStanding)
                sneakHeadSpacing = Input.GetButton("Fire2");

            if (sneakHeadSpacing)
            {
                float yMouseMove = Input.GetAxis("Mouse Y") * mouseSenseControlSpacing;
                float xMouseMove = Input.GetAxis("Mouse X joyInverted") * mouseSenseControlSpacing;
                Vector3 rotation = headRotationSphere.transform.rotation.eulerAngles + new Vector3(yMouseMove, xMouseMove, 0f);
                //ограничения на вращение
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

            lastHeadCollision = headCollisions.lastHeadCollision;
            if (Input.GetButtonDown("FixSneakHead") && canClimbing)
            {
                headFixed = !headFixed;
            }

            if (headFixed)
            {
                if (headJoint == null)
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

            if (Input.GetButtonDown("PreviousSupportBoddy"))
            {
                SetNewSupportBody(bodySupportPoint - 1);
            }
        }
    }

    private void FixedUpdate()
    {
        //реализация перемещения змеи по земле
        //направление движения соответсвующее направлению камере
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
                //определение степени сжатия
                if (i > 0)
                    bodySpring = (Vector3.Distance(sneakBody[i].transform.position, sneakBody[i - 1].transform.position) - bodyZscale) / bodiesInterval;
                else
                {
                    bodySpring = (Vector3.Distance(sneakBody[i].transform.position, sneakHead.transform.position) - bodyZscale) / bodiesInterval;
                }
                bodySpring = 1 - bodySpring;
                
                Joint joint = sneakBody[i].GetComponent<Joint>();
                Vector3 forceDirection = (joint.connectedBody.transform.position - sneakBody[i].position).normalized;

                if ((0.98f > bodySpring && _headForceControl == 0) || _headForceControl > 0) //(если уровень сжатия меньше 90% и нужно сжатие) или нужно разжатие
                {
                    //то двигаем позвонок
                    sneakBody[i].AddForce(forceDirection * forcePerBody * BodyForceControl);
                }
            }
        }

        //реализация механики перемещения головы змеи в пространстве
        //просчёт точки куда голова змеи должна стремиться
        targetHeadPosition = headRotationSphere.transform.TransformPoint(Vector3.forward * supportLength * 1.5f);
        targetPositionDebug.transform.position = targetHeadPosition;

        //выполнение перемещения в ту точку куда стремиться голова
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

    public void SetHeadJoint()
    {
        if (lastHeadCollision != null)
        {
            headJoint = sneakHead.gameObject.AddComponent<FixedJoint>();
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
        transform.position = spawnPoint;
        List<Rigidbody> list = new List<Rigidbody>();

        //иниициализация параметров змейки
        SneakUpgrades.LengthLevel parameters = upgrades.lengthLevels[upgrades.currentLengthLevel];
        forcePerBody = parameters.forcePerBody;
        headForce = parameters.headForce;
        headSpacingForce = parameters.headSpacingForce;
        sneakSize = parameters.bodiesCount;
        bodyZscale = parameters.bodyZScale;
        bodiesInterval = parameters.bodiesInterval;
        skinPrefab = upgrades.sneakSkin0;

        if(parameters.lengthLevel == 1)
        {
            skinPrefab = upgrades.sneakSkin1;
        }

        if (parameters.lengthLevel == 2)
        {
            bodyPrefab = upgrades.bodyPrefab2;
            headPrefab = upgrades.headPrefab2;
            tailPrefab = upgrades.tailPrefab2;
            skinPrefab = upgrades.sneakSkin2;
        }

        if (parameters.lengthLevel == 3)
        {
            bodyPrefab = upgrades.bodyPrefab3;
            headPrefab = upgrades.headPrefab3;
            tailPrefab = upgrades.tailPrefab3;
            skinPrefab = upgrades.sneakSkin3;
        }
        //конец инициализации

        //создание головы
        GameObject head = Instantiate(headPrefab);
        head.transform.position = transform.position;
        head.transform.parent = transform;
        sneakHead = head.GetComponent<Rigidbody>();
        headCollisions = head.GetComponent<SneakHead>();

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
        //позвонок для опоры
        SetNewSupportBody(sneakSize / 2);

        //настройка модельки змеи
        if(useSkin)
        {
            //создание модельки змеи
            skinObject = Instantiate(skinPrefab);
            skinMesh = skinObject.GetComponent<SkinnedMeshRenderer>();
            skinObject.transform.rotation = Quaternion.Euler(-90f, -90f, 0f);
            skinObject.transform.position = transform.position;
            skinObject.transform.parent = transform;

            //добавление костей для кожи змеи
            Transform[] bones = skinMesh.bones;
            //кость головы
            bones[0].transform.parent = sneakHead.transform;
            bones[0].transform.localPosition = Vector3.zero;
            sneakHead.GetComponent<MeshRenderer>().enabled = false;
            int listIndex = 0;
            //кости тела
            for (int boneIndex = 1; boneIndex < bones.Length - 1; boneIndex++)
            {
                bones[boneIndex].parent = list[listIndex].transform;
                bones[boneIndex].transform.localPosition = Vector3.zero;
                list[listIndex].GetComponent<MeshRenderer>().enabled = false;
                listIndex++;
            }
            //кость хвоста
            bones[bones.Length - 1].transform.parent = sneakTail.transform;
            bones[bones.Length - 1].transform.localPosition = Vector3.zero;
            sneakTail.GetComponent<MeshRenderer>().enabled = false;
            skinMesh.bones = bones;
        }
    }

    public void DestroySneak()
    {
        spawnPoint = sneakHead.transform.position + Vector3.up * 2f;
        Destroy(sneakHead.gameObject);
        Destroy(sneakTail.gameObject);
        for(int i = 0; i < sneakSize; i++)
        {
            Destroy(sneakBody[i].gameObject);
        }
        sneakBody.Clear();
        Destroy(skinMesh.gameObject);
    }

    public void SetMaterial(Material material)
    {
        if(material != null)
        {
            skinMesh.material = material;
        }
    }

    public void Respawn()
    {
        DestroySneak();
        if (spawnPoint.y > respawnLevel.position.y)
            spawnPoint = secondSpawnPoint.position;
        else
            spawnPoint = firstSpawnPoint.position;
        GenerateSneak();
    }

    public Vector3 HeadPosition()
    {
        return sneakHead.transform.position;
    }
}
