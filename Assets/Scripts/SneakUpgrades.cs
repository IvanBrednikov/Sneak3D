using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakUpgrades : MonoBehaviour
{
    public int currentLengthLevel = 0;
    public string skin;

    SneakControl sneak;

    public GameObject headPrefab1;
    public GameObject headPrefab2;
    public GameObject headPrefab3;
    public GameObject bodyPrefab1;
    public GameObject bodyPrefab2;
    public GameObject bodyPrefab3;
    public GameObject tailPrefab1;
    public GameObject tailPrefab2;
    public GameObject tailPrefab3;
    public GameObject sneakSkin0;
    public GameObject sneakSkin1;
    public GameObject sneakSkin2;
    public GameObject sneakSkin3;

    [SerializeField]
    GameObject swimmingObstacle;

    [SerializeField]
    Material defaultSkin;
    [SerializeField]
    Material[] length0Skins;
    [SerializeField]
    Material[] length1Skins;
    [SerializeField]
    Material[] length2Skins;
    [SerializeField]
    Material[] length3Skins;

    //savedGame
    bool canClimbing;
    bool canStanding;
    public Material savedSneakSkin;

    private void Start()
    {
        sneak = GetComponent<SneakControl>();
    }

    public LengthLevel[] lengthLevels = 
    { 
        new LengthLevel(0, 2f, 25, 15, 10, 0.05f, 0.02f),
        new LengthLevel(1, 3, 20, 15, 20, 0.05f, 0.02f),
        new LengthLevel(2, 4, 25, 15, 30, 0.10f, 0.04f),
        new LengthLevel(3, 4, 30, 10, 40, 0.15f, 0.06f)
    };

    public struct LengthLevel
    {
        public int lengthLevel;
        public float forcePerBody;
        public float headForce;
        public float headSpacingForce;
        public int bodiesCount;
        public float bodyZScale;
        public float bodiesInterval;

        public LengthLevel(int level, float forcePerBody, float headForce, 
            float headSpacingForce, int bodiesCount, float bodyZScale,
            float bodiesInterval)
        {
            this.lengthLevel = level;
            this.forcePerBody = forcePerBody;
            this.headForce = headForce;
            this.headSpacingForce = headSpacingForce;
            this.bodiesCount = bodiesCount;
            this.bodyZScale = bodyZScale;
            this.bodiesInterval = bodiesInterval;
        }
    }

    //�������� ����������� ������
    public void ReGenerateSneak()
    {
        sneak.DestroySneak();
        sneak.GenerateSneak();
    }

    public void UpgradeSneak(int lengthLevel, bool swimming, bool standing, bool climbing, string skin)
    {
        bool regenerateNeed = lengthLevel != currentLengthLevel;
        currentLengthLevel = lengthLevel;
        sneak.canClimbing = climbing;
        sneak.canStanding = standing;
        this.skin = skin;

        if (swimmingObstacle != null)
            swimmingObstacle.SetActive(!swimming);

        if (regenerateNeed)
            ReGenerateSneak();

        Material mat = GetMaterialSkin(skin);
        sneak.SetMaterial(mat);
    }

    public Material GetMaterialSkin(string skin)
    {
        Material result = defaultSkin;

        if(skin != "defaultSkin" && skin != "skin1")
        {
            int skinNumber = -1;

            switch (skin)
            {
                case "skin2":
                    skinNumber = 0;
                    break;
                case "skin3":
                    skinNumber = 1;
                    break;
                case "skin4":
                    skinNumber = 2;
                    break;
                case "skin5":
                    skinNumber = 3;
                    break;
            }


            if (skinNumber >= 0 && skinNumber < 4)
            {
                switch (currentLengthLevel)
                {
                    case 0:
                        result = length0Skins[skinNumber];
                        break;
                    case 1:
                        result = length1Skins[skinNumber];
                        break;
                    case 2:
                        result = length2Skins[skinNumber];
                        break;
                    case 3:
                        result = length3Skins[skinNumber];
                        break;
                }
            }
        }

        return result;
    }

    public void UpgradeSneakBeforeStart(int lengthLevel, bool swimming, bool standing, bool climbing, string skin)
    {
        currentLengthLevel = lengthLevel;
        this.canClimbing = climbing;
        this.canStanding = standing;
        this.skin = skin;

        if (swimmingObstacle != null)
            swimmingObstacle.SetActive(!swimming);

        savedSneakSkin = GetMaterialSkin(skin);
    }

    public void SavedUpgradeApplyOnStart()
    {
        sneak.canClimbing = canClimbing;
        sneak.canStanding = canStanding;
    }
}
