using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UpgradeButton : Button
{
    bool upgradeSelected;
    bool upgradeApplied;
    [SerializeField]
    int upgradeCost;
    [SerializeField]
    bool skinButton;

    public GameObject foodCostImage;
    public Text foodCostText;

    public Sprite unSelectedSprite;
    public Sprite selectedSprite;

    public event System.EventHandler OnAnyClick;

    protected override void Start()
    {
        OnUpgradeDeselect += (obj, e) => { };
        OnUpgradeSelect += (obj, e) => { };
        OnAnyClick += (obj, e) => { };
        foodCostText.text = upgradeCost.ToString();
    }

    public int UpgradeCost 
    { 
        get => upgradeCost;
        set 
        {
            upgradeCost = value;
            foodCostText.text = value.ToString();
        }
    }

    public bool UpgradeApplied { get => upgradeApplied; set => upgradeApplied = value; }
    public bool UpgradeSelected
    { 
        get => upgradeSelected;
        set 
        {
            upgradeSelected = value;
            if(value)
                image.sprite = selectedSprite;
            else
                image.sprite = unSelectedSprite;
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(!upgradeApplied || skinButton)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!upgradeSelected && IsInteractable())
                {
                    upgradeSelected = true;
                    image.sprite = selectedSprite;
                    OnUpgradeSelect(this, null);
                }
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (upgradeSelected)
                {
                    upgradeSelected = false;                    
                    image.sprite = unSelectedSprite;
                    OnUpgradeDeselect(this, null);
                }
            }
            OnAnyClick(this, null);
        }
    }

    public event System.EventHandler OnUpgradeSelect;

    public event System.EventHandler OnUpgradeDeselect;

    public void Deselect()
    {
        upgradeSelected = false;
        OnUpgradeDeselect(this, null);
        image.sprite = unSelectedSprite;
    }

    public void UpgradeApply()
    {
        upgradeApplied = true;
        foodCostImage.gameObject.SetActive(false);
        foodCostText.gameObject.SetActive(false);
    }

}
