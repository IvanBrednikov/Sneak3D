using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalController : MonoBehaviour
{
    [SerializeField]
    Text goalText;

    int currentGoal = 0;
    string[] goals =
{
"Goal: Collect 20 food",
"Goal: Make upgrade \"Climbing\"",
"Goal: Get up on the Great Tree",
"Goal: Find enter to the temple",
"Goal: Activate 3 pressure plates",
"Goal: Get into the temple",
"Goal: Complete puzzle",
"Game end!"
};

    //для отслеживания целей
    [SerializeField]
    UpgradesPanelHandler panel;
    [SerializeField]
    UpgradeButton climbingButton;
    [SerializeField]
    TempleDoorPlate[] plates;
    [SerializeField]
    PuzzleController puzzle;

    //анимация и звук нового задания
    [SerializeField]
    AudioSource questCompleteAudio;
    [SerializeField]
    RectTransform newGoalPanel;
    [SerializeField]
    Text newGoalText;
    [SerializeField]
    float animationSpeed;
    [SerializeField]
    float hidePos;
    [SerializeField]
    float showPos;
    bool hiding;
    bool showing;
    bool standing;
    [SerializeField]
    float standingTime = 5f;
    float standingTimer;

    private void Start()
    {
        goalText.text = goals[currentGoal];
    }

    private void Update()
    {
        if (panel.totalPoints >= 20)
        {
            SetNewGoal(1);
        }

        if(climbingButton.UpgradeApplied)
        {
            SetNewGoal(2);
        }

        if(currentGoal == 4)
        {
            int activatedPlates = 0;
            for (int i = 0; i < plates.Length; i++)
                if (plates[i].isActive)
                    activatedPlates++;
            goalText.text = goals[4] + $" ({activatedPlates}/3)";
            
            if(activatedPlates == plates.Length)
            {
                SetNewGoal(5);
            }
        }

        if(currentGoal == 0)
        {
            goalText.text = goals[0] + $" ({panel.totalPoints}/20)";
        }

        if(puzzle.CheckCondition())
        {
            SetNewGoal(7);
        }

        if(showing)
        {
            Vector3 position = newGoalPanel.localPosition;
            position.y -= animationSpeed * Time.deltaTime;
            newGoalPanel.localPosition = position;

            if(position.y <= showPos)
            {
                showing = false;
                standing = true;
                standingTimer = standingTime;
            }
        }

        if(standing)
        {
            standingTimer -= Time.deltaTime;
            if(standingTimer <= 0)
            {
                standing = false;
                hiding = true;
            }
        }

        if (hiding)
        {
            Vector3 position = newGoalPanel.localPosition;
            position.y += animationSpeed * Time.deltaTime;
            newGoalPanel.localPosition = position;

            if (position.y >= hidePos)
            {
                hiding = false;
                newGoalPanel.gameObject.SetActive(false);
            }
        }
    }

    public void SetNewGoal(int goalNumb)
    {
        if(currentGoal == goalNumb - 1)
        {
            currentGoal = goalNumb;
            goalText.text = goals[goalNumb];
            questCompleteAudio.Play();
            NewQuestAnimation();
        }
    }

    public void TreeTriggerReact()
    {
        SetNewGoal(3);
    }

    public void TempleTriggerReact()
    {
        SetNewGoal(4);
    }

    public void SecondTempleTriggerReact()
    {
        SetNewGoal(6);
    }

    public void NewQuestAnimation()
    {
        newGoalText.text = "New " + goals[currentGoal];
        newGoalPanel.gameObject.SetActive(true);
        showing = true;
        hiding = false;
        standing = false;
    }

    public void ShowGoal()
    {
        questCompleteAudio.Play();
        NewQuestAnimation();
        newGoalPanel.gameObject.SetActive(true);
    }

    public int CurrentGoal 
    { 
        get { return currentGoal; } 
        set 
        { 
            currentGoal = value;
            goalText.text = goals[value];
        } 
    }
}
