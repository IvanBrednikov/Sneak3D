using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public GameMenu gameMenu;
    [SerializeField]
    UpgradesPanelHandler upgradesPanel;
    [SerializeField]
    GameObject options;
    [SerializeField]
    GameObject quitConfirm;
    [SerializeField]
    GameObject controls;

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !gameMenu.gameObject.activeSelf && !options.activeSelf)
        {
            if (upgradesPanel.gameObject.activeSelf)
                upgradesPanel.ClosePanel();
            gameMenu.Open();
        }

        if (Input.GetButtonDown("Cancel") && options.activeSelf)
        {
            gameMenu.CloseOptions();
        }

        if (Input.GetButtonDown("Cancel") && controls.activeSelf)
        {
            gameMenu.CloseControls();
        }

        if (Input.GetButtonDown("UpgradesPanelOpen") && !upgradesPanel.gameObject.activeSelf && !gameMenu.gameObject.activeSelf && !options.activeSelf)
        {
            upgradesPanel.Open();
        }
    }

    public bool UiActive
    {
        get
        {
            return 
                gameMenu.gameObject.activeSelf || 
                upgradesPanel.gameObject.activeSelf || 
                options.activeSelf || 
                quitConfirm.activeSelf ||
                controls.activeSelf;
        }
    }
}
