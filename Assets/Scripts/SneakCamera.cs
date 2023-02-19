using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakCamera : MonoBehaviour
{
    [SerializeField]
    public GameObject viewObject;
    [SerializeField]
    CameraRotationSphere rotationSphere;
    [SerializeField]
    public float distance;
    [SerializeField]
    float scrollSpeed;
    [SerializeField]
    float maxDistance;
    [SerializeField]
    float minDistance;
    [SerializeField]
    float mouseSenseControlCamera;
    [SerializeField]
    float depthOnStart;

    Collider otherCollider; //коллайдер объекта с которым пересекается триггер камеры
    [SerializeField]
    UserInterface ui;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.depth = depthOnStart;
        rotationSphere.transform.parent = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        //движение камеры за целью
        if (viewObject != null)
            rotationSphere.transform.position =
                viewObject.transform.position;

        //поворот камеры по движению мыши   
        float xMouseMove = Input.GetAxis("Mouse X") * mouseSenseControlCamera;
        float yMouseMove = Input.GetAxis("Mouse Y") * mouseSenseControlCamera;

        if (Input.GetButton("Fire2"))
        {
            yMouseMove = 0;
            xMouseMove = Input.GetAxis("Mouse X joyInverted") * mouseSenseControlCamera; //для геймпада
        }

        if (Input.GetButton("Fire1") || ui.UiActive)
        {
            xMouseMove = 0;
            yMouseMove = 0;
        }

        Vector3 rotation = rotationSphere.transform.rotation.eulerAngles + new Vector3(-yMouseMove, xMouseMove, 0);

        float virtualYmov = rotationSphere.transform.rotation.eulerAngles.x - yMouseMove;
        if (virtualYmov > 80 && virtualYmov < 180)
        {
            rotation = new Vector3(80, xMouseMove + rotationSphere.transform.rotation.eulerAngles.y, 0);
        }

        if (virtualYmov < 280 && virtualYmov >= 180)
        {
            rotation = new Vector3(280, xMouseMove + rotationSphere.transform.rotation.eulerAngles.y, 0);
        }

        rotationSphere.transform.rotation = Quaternion.Euler(rotation);

        otherCollider = rotationSphere.otherCollider;
        //исключение выхода камера через terrain и с другими объектами
        float cuttedDistance = distance;
        
        RaycastHit[] hits = Physics.RaycastAll(rotationSphere.transform.position, rotationSphere.transform.TransformDirection(new Vector3(0f, 0f, -1)), distance);

        for (int i = 0; i < hits.Length; i++)
            if (otherCollider != null && hits[i].collider.name == otherCollider.name)
            {
                cuttedDistance = hits[i].distance;
                break;
            }

        hits = Physics.RaycastAll(rotationSphere.transform.position, rotationSphere.transform.TransformDirection(new Vector3(0f, 0f, -1)), distance + 1f);
        float highestLevel = 0;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.name == "Terrain" || 
                hits[i].transform.name == "WaterLevel" ||
                hits[i].transform.tag == "TempleGeometry")
            {
                if(hits[i].point.y > highestLevel)
                {
                    cuttedDistance = hits[i].distance - 1f;
                    highestLevel = hits[i].point.y;
                }
            }
        }
        
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -cuttedDistance);

        float mouseWheel = Input.GetAxis("MouseWheel");
        float newDistance = distance - (mouseWheel * scrollSpeed);

        if ((newDistance < maxDistance && newDistance > minDistance) && !ui.UiActive)
            distance = newDistance;
    }

    public void SetMouseSensevity(float value)
    {
        mouseSenseControlCamera = value;
    }
}
