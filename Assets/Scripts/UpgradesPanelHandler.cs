using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradesPanelHandler : MonoBehaviour
{
    public int currentPoints = 20;
    int pointsSpent = 0;
    [SerializeField]
    SneakUpgrades sneakUpgrades;
    [SerializeField]
    Text pointsLabel;

    [SerializeField]
    UpgradeButton length1Button;
    [SerializeField]
    UpgradeButton length2Button;
    [SerializeField]
    UpgradeButton length3Button;
    [SerializeField]
    UpgradeButton swimmingButton;
    [SerializeField]
    UpgradeButton standingButton;
    [SerializeField]
    UpgradeButton climbingButton;
    [SerializeField]
    UpgradeButton skin1Button;
    [SerializeField]
    UpgradeButton skin2Button;
    [SerializeField]
    UpgradeButton skin3Button;
    [SerializeField]
    UpgradeButton skin4Button;
    [SerializeField]
    UpgradeButton skin5Button;

    string lastSkinSelected = "default";

    UpgradeButton[] GetUpgradeButtons 
    {
        get
        {
            UpgradeButton[] upgrades = 
            {
                length1Button,
                length2Button,
                length3Button,
                swimmingButton,
                standingButton,
                climbingButton,
                skin1Button,
                skin2Button,
                skin3Button,
                skin4Button,
                skin5Button
            };
            return upgrades;
        } 
    }

    void Start()
    {
        length1Button.OnUpgradeDeselect += Length1Button_OnUpgradeDeselect;
        length2Button.OnUpgradeDeselect += Length2Button_OnUpgradeDeselect;
        standingButton.OnUpgradeDeselect += StandingButton_OnUpgradeDeselect;

        //скины
        skin1Button.OnUpgradeSelect += (obj, e) => { lastSkinSelected = "skin1"; DeselectAppliedSkins(); };
        skin2Button.OnUpgradeSelect += (obj, e) => { lastSkinSelected = "skin2"; DeselectAppliedSkins(); };
        skin3Button.OnUpgradeSelect += (obj, e) => { lastSkinSelected = "skin3"; DeselectAppliedSkins(); };
        skin4Button.OnUpgradeSelect += (obj, e) => { lastSkinSelected = "skin4"; DeselectAppliedSkins(); };
        skin5Button.OnUpgradeSelect += (obj, e) => { lastSkinSelected = "skin5"; DeselectAppliedSkins(); };

        skin1Button.OnUpgradeDeselect += Skin1Button_OnUpgradeDeselect;
        skin2Button.OnUpgradeDeselect += Skin1Button_OnUpgradeDeselect;
        skin3Button.OnUpgradeDeselect += Skin1Button_OnUpgradeDeselect;
        skin4Button.OnUpgradeDeselect += Skin1Button_OnUpgradeDeselect;
        skin5Button.OnUpgradeDeselect += Skin1Button_OnUpgradeDeselect;

        UpgradeButton[] buttons = GetUpgradeButtons;

        for(int i = 0; i < buttons.Length; i++)
            buttons[i].OnAnyClick += (obj, e) => { ButtonsActivating(); };

        ButtonsActivating();
    }

    private void Skin1Button_OnUpgradeDeselect(object sender, System.EventArgs e)
    {
        if (SkinsButtonAreNotSelected)
            lastSkinSelected = "default";
        else
            SetSelectedSkin();
    }

    private void StandingButton_OnUpgradeDeselect(object sender, System.EventArgs e)
    {
        climbingButton.Deselect();
    }

    private void Length2Button_OnUpgradeDeselect(object sender, System.EventArgs e)
    {
        standingButton.Deselect();
        climbingButton.Deselect();
        length3Button.Deselect();
    }

    private void Length1Button_OnUpgradeDeselect(object sender, System.EventArgs e)
    {
        length2Button.Deselect();
        swimmingButton.Deselect();
    }

    void Update()
    {
        pointsLabel.text = (currentPoints - pointsSpent).ToString();

        if(Input.GetButtonDown("UpgradesPanelOpen"))
        {
            ClosePanel();
        }
    }

    public void ConfirmChanges()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpgradesApply();
        ApplyToSneak();
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ButtonsActivating();
    }

    public void ClosePanel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpgradesDisagree();
        gameObject.SetActive(false);
    }

    void ApplyToSneak()
    {
        int lengthLevel = 0;
        if (length1Button.UpgradeApplied)
            lengthLevel = 1;
        if (length2Button.UpgradeApplied)
            lengthLevel = 2;
        if (length3Button.UpgradeApplied)
            lengthLevel = 3;

        sneakUpgrades.UpgradeSneak(lengthLevel, swimmingButton.UpgradeApplied, standingButton.UpgradeApplied, climbingButton.UpgradeApplied, lastSkinSelected);
    }

    //определяет какие кнопки будут включены
    private void ButtonsActivating()
    {
        UpgradeButton[] buttons = GetUpgradeButtons;

        pointsSpent = 0;
        //посчёт потраченых очков
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].UpgradeSelected && !buttons[i].UpgradeApplied)
                pointsSpent += buttons[i].UpgradeCost;
        }
        int freePoints = currentPoints - pointsSpent;

        //определение активности кнопок
        for (int i = 0; i < buttons.Length; i++)
        {
            if (freePoints < buttons[i].UpgradeCost && !buttons[i].UpgradeApplied && !buttons[i].UpgradeSelected)
                buttons[i].interactable = false;
            else
                buttons[i].interactable = true;
        }

        if(!length1Button.UpgradeApplied && !length1Button.UpgradeSelected)
        {
            swimmingButton.interactable = false;
            length2Button.interactable = false;
            standingButton.interactable = false;
        }

        if(!length2Button.UpgradeApplied && !length2Button.UpgradeSelected)
        {
            length3Button.interactable = false;
            climbingButton.interactable = false;
        }

        if(!standingButton.UpgradeApplied && !standingButton.UpgradeSelected)
        {
            climbingButton.interactable = false;
        }
    }

    void UpgradesApply()
    {
        UpgradeButton[] buttons = GetUpgradeButtons;

        for(int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].UpgradeSelected)
                buttons[i].UpgradeApply();
        }

        currentPoints -= pointsSpent;
        pointsSpent = 0;

        skin1Button.UpgradeSelected = false;
        skin2Button.UpgradeSelected = false;
        skin3Button.UpgradeSelected = false;
        skin4Button.UpgradeSelected = false;
        skin5Button.UpgradeSelected = false;

        switch (lastSkinSelected)
        {
            case "skin1":
                skin1Button.UpgradeSelected = true;
                break;
            case "skin2":
                skin2Button.UpgradeSelected = true;
                break;
            case "skin3":
                skin3Button.UpgradeSelected = true;
                break;
            case "skin4":
                skin4Button.UpgradeSelected = true;
                break;
            case "skin5":
                skin5Button.UpgradeSelected = true;
                break;
            default:
                break;
        }
    }

    void UpgradesDisagree()
    {
        UpgradeButton[] buttons = GetUpgradeButtons;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].UpgradeSelected && !buttons[i].UpgradeApplied)
                buttons[i].Deselect();
        }
        pointsSpent = 0;
    }

    bool SkinsButtonAreNotSelected
    {
        get
        {
            bool result = 
                !(skin1Button.UpgradeSelected || 
                skin2Button.UpgradeSelected || 
                skin3Button.UpgradeSelected || 
                skin4Button.UpgradeSelected || 
                skin5Button.UpgradeSelected);
            return result;
        }
    }

    void SetSelectedSkin()
    {
        if (skin1Button.UpgradeSelected)
            lastSkinSelected = "skin1";
        if (skin2Button.UpgradeSelected)
            lastSkinSelected = "skin2";
        if (skin3Button.UpgradeSelected)
            lastSkinSelected = "skin3";
        if (skin4Button.UpgradeSelected)
            lastSkinSelected = "skin4";
        if (skin5Button.UpgradeSelected)
            lastSkinSelected = "skin5";
    }

    void DeselectAppliedSkins()
    {
        if(skin1Button.UpgradeApplied && lastSkinSelected != "skin1")
            skin1Button.UpgradeSelected = false;
        if (skin2Button.UpgradeApplied && lastSkinSelected != "skin2")
            skin2Button.UpgradeSelected = false;
        if (skin3Button.UpgradeApplied && lastSkinSelected != "skin3")
            skin3Button.UpgradeSelected = false;
        if (skin4Button.UpgradeApplied && lastSkinSelected != "skin4")
            skin4Button.UpgradeSelected = false;
        if (skin5Button.UpgradeApplied && lastSkinSelected != "skin5")
            skin5Button.UpgradeSelected = false;
    }

    public void FoodAdd(int value)
    {
        currentPoints += value;
    }
}
