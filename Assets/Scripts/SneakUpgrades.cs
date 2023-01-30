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

    private void Start()
    {
        sneak = GetComponent<SneakControl>();
    }

    public LengthLevel[] lengthLevels = 
    { 
        new LengthLevel(0, 1.5f, 25, 20, 10, 0.05f, 0.02f),
        new LengthLevel(1, 2, 20, 20, 20, 0.05f, 0.02f),
        new LengthLevel(2, 2, 20, 20, 30, 0.10f, 0.04f),
        new LengthLevel(3, 4, 30, 30, 40, 0.15f, 0.06f)
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

    //вызывает регенерацию змейки
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

        if (regenerateNeed)
            ReGenerateSneak();
    }
}
