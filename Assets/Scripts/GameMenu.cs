using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings; 

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    Button resumeButton;
    [SerializeField]
    GameObject options;
    [SerializeField]
    GameObject controls;
    [SerializeField]
    Button controlsClose;
    [SerializeField]
    GameObject quitConfirm;
    [SerializeField]
    Button cancelQuit;
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

    //сохранения
    [SerializeField]
    GoalController goalController;
    public bool isFirstPlay = true;
    public bool loadProgress = true;
    [SerializeField]
    SneakControl sneak;
    public Vector3 playerSpawn;
    [SerializeField]
    GameObject enviromentSounds;
    [SerializeField]
    UpgradesPanelHandler panel;

    private void Start()
    {
        sneakCam = sneakCamera.GetComponent<SneakCamera>();
        SetResolutoinsDropDownItems();
        SetScreenResolution(resolutionDrop.options.Count - 1);
        LoadSettings();
        gameObject.SetActive(false);

        if (PlayerPrefs.HasKey("firstPlay"))
        {
            int firstPlay = PlayerPrefs.GetInt("firstPlay");
            isFirstPlay = firstPlay == 1;
        }
        else
            isFirstPlay = true;

        if(isFirstPlay)
        {
            Open();
            OpenControls();
        }
        else
        {
            if(loadProgress)
            {
                LoadProgress();
            }
            goalController.ShowGoal();
            enviromentSounds.SetActive(true);
        }     
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
        quitConfirm.SetActive(false);
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
        SaveProgress();
        Application.Quit();
    }

    public void QuitWithoutSave()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        DeleteProgress();
        SceneManager.LoadScene("SampleScene");
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
        if(PlayerPrefs.HasKey("fov"))
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
        int curreResItem = -1;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string str = $"{resolutions[i].width}x{resolutions[i].height} {resolutions[i].refreshRate}hz";
            Dropdown.OptionData data = new Dropdown.OptionData(str);
            options.Add(data);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                curreResItem = i;
        }

        resolutionDrop.options = options;
        resolutionDrop.value = curreResItem;
    }

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }

    public void OpenControls()
    {
        controls.SetActive(true);
        controlsClose.Select();
        gameObject.SetActive(false);
    }
    
    public void CloseControls()
    {
        if(isFirstPlay)
        {
            isFirstPlay = false;
            Close();
            goalController.ShowGoal();
            enviromentSounds.SetActive(true);
        }
        else
        {
            Open();
        }
        controls.SetActive(false);
    }

    void SaveProgress()
    {
        Vector3 playerPosition = sneak.HeadPosition();
        int goal = goalController.CurrentGoal;

        PlayerPrefs.SetFloat("sneakX", playerPosition.x);
        PlayerPrefs.SetFloat("sneakY", playerPosition.y);
        PlayerPrefs.SetFloat("sneakZ", playerPosition.z);
        PlayerPrefs.SetInt("goal", goal);
        PlayerPrefs.SetInt("firstPlay", 0);

        panel.SaveProgress();
    }

    void LoadProgress()
    {
        Vector3 playerPosition = new Vector3();

        playerPosition.x = PlayerPrefs.GetFloat("sneakX");
        playerPosition.y = PlayerPrefs.GetFloat("sneakY") + 2f;
        playerPosition.z = PlayerPrefs.GetFloat("sneakZ");
        int goal = PlayerPrefs.GetInt("goal");

        goalController.CurrentGoal = goal;
        playerSpawn = playerPosition;

        panel.LoadProgress();
    }

    void DeleteProgress()
    {
        PlayerPrefs.DeleteKey("sneakX");
        PlayerPrefs.DeleteKey("sneakY");
        PlayerPrefs.DeleteKey("sneakZ");
        PlayerPrefs.DeleteKey("goal");
        PlayerPrefs.SetInt("firstPlay", 1);
        panel.DeleteProgress();
    }

    public void SelectLocale(int value)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value]; //0 - en, 1 -ru
    }
}
