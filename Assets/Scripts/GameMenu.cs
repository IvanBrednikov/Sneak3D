using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    Button resumeButton;
    [SerializeField]
    GameObject options;
    [SerializeField]
    Camera sneakCamera;
    SneakCamera sneakCam;
    [SerializeField]
    InputField mouseSenseField;
    [SerializeField]
    AudioMixer audioMixer;

    //для сохранения настроек
    [SerializeField]
    Slider masterVolumeSlider;
    [SerializeField]
    Slider envVolume;
    [SerializeField]
    Slider effectsVolume;
    [SerializeField]
    Slider fieldOfView;
    [SerializeField]
    Dropdown graphic;
    [SerializeField]
    Toggle fullScreenToggle;
    [SerializeField]
    Dropdown resolutionDrop;

    private void Start()
    {
        sneakCam = sneakCamera.GetComponent<SneakCamera>();
        SetResolutoinsDropDownItems();
        LoadSettings();
        gameObject.SetActive(false);
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
        resumeButton.Select();
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
        masterVolumeSlider.Select();
        gameObject.SetActive(false);
    }

    public void CloseOptions()
    {
        SetMouseSensevity(mouseSenseField.text);
        SaveSettings();
        options.SetActive(false);
        resumeButton.Select();
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
        QualitySettings.SetQualityLevel(value, true);
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

    void SaveSettings()
    {
        float mouseSense = 1f;
        float.TryParse(mouseSenseField.text, out mouseSense);
        int masterVolume = (int)masterVolumeSlider.value;
        int enviromentVolume = (int)envVolume.value;
        int effectVolume = (int)effectsVolume.value;
        int fov = (int)fieldOfView.value;
        int graph = graphic.value;
        int fullRes = (fullScreenToggle.isOn) ? 1 : 0;
        int resItem = resolutionDrop.value;

        PlayerPrefs.SetFloat("sens", mouseSense);
        PlayerPrefs.SetInt("masterV", masterVolume);
        PlayerPrefs.SetInt("envVol", enviromentVolume);
        PlayerPrefs.SetInt("effVol", effectVolume);
        PlayerPrefs.SetInt("fov", fov);
        PlayerPrefs.SetInt("graphic", graph);
        PlayerPrefs.SetInt("fullRes", fullRes);
        PlayerPrefs.SetInt("resItem", resItem);
    }

    public void LoadSettings()
    {
        if(PlayerPrefs.HasKey("sens"))
        {
            float mouseSense = PlayerPrefs.GetFloat("sens");
            int masterVolume = PlayerPrefs.GetInt("masterV");
            int enviromentVolume = PlayerPrefs.GetInt("envVol");
            int effectVolume = PlayerPrefs.GetInt("effVol");
            int fov = PlayerPrefs.GetInt("fov");
            int graph = PlayerPrefs.GetInt("graphic");
            bool fullRes = PlayerPrefs.GetInt("fullRes") == 1;
            int resItem = PlayerPrefs.GetInt("resItem");

            if (mouseSense == 0)
                mouseSense = 1f;
            if (resItem > resolutionDrop.options.Count - 1)
                resItem = resolutionDrop.options.Count - 1;

            sneakCam.SetMouseSensevity(mouseSense);
            mouseSenseField.text = mouseSense.ToString();
            SetMasterVolume(masterVolume);
            masterVolumeSlider.value = masterVolume;
            SetEnviromentVolume(enviromentVolume);
            envVolume.value = enviromentVolume;
            SetEffectsVolume(effectVolume);
            effectsVolume.value = effectVolume;
            SetFieldOfView(fov);
            fieldOfView.value = fov;
            graphic.value = graph;
            fullScreenToggle.isOn = fullRes;
            SetScreenResolution(resItem);
            resolutionDrop.value = resItem;
        }
    }

    public void SetScreenResolution(int item)
    {
        Resolution[] resolutions = Screen.resolutions;
        Screen.SetResolution(resolutions[item].width, resolutions[item].height, fullScreenToggle.isOn, resolutions[item].refreshRate);
    }

    void SetResolutoinsDropDownItems()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string str = $"{resolutions[i].width}x{resolutions[i].height} {resolutions[i].refreshRate}hz";
            Dropdown.OptionData data = new Dropdown.OptionData(str);
            options.Add(data);
        }

        resolutionDrop.options = options;
    }

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }
}
