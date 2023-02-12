using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    GameObject options;
    [SerializeField]
    Camera sneakCamera;
    SneakCamera sneakCam;
    [SerializeField]
    InputField mouseSenseField;
    [SerializeField]
    AudioMixer audioMixer;

    private void Start()
    {
        sneakCam = sneakCamera.GetComponent<SneakCamera>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Close();
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Close()
    {
        CloseOptions();
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }

    public void Resume()
    {
        Close();
    }

    public void Options()
    {
        options.SetActive(true);
        gameObject.SetActive(false);
    }

    public void CloseOptions()
    {
        SetMouseSensevity(mouseSenseField.text);
        options.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    //Options funtcions
    public void SetFieldOfView(float value)
    {
        sneakCamera.fieldOfView = value;
    }

    public void SetMouseSensevity(string value)
    {
        float sense;
        value = value.Replace('.', ',');
        if(float.TryParse(value, out sense))
        {
            sneakCam.SetMouseSensevity(sense);
        }
        else
        {
            mouseSenseField.text = "";
        }
    }

    public void SetGraphicQuality(int value)
    {
        Debug.Log(QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(value, true);
        Debug.Log(QualitySettings.GetQualityLevel());
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value);
    }

    public void SetEnviromentVolume(float value)
    {
        audioMixer.SetFloat("EnviromentVolume", value);
    }

    public void SetEffectsVolume(float value)
    {
        audioMixer.SetFloat("EffectsVolume", value);
    }
}
