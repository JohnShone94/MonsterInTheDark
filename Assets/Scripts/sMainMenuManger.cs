using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sMainMenuManger : MonoBehaviour
{
    public Button btnStart;
    public Button btnQuit;
    public Button btnOptions;

    public Toggle fullScreenToggle;
    public Toggle advancedToggle;
    public Dropdown resDropdown;
    public Dropdown textureDropdown;
    public Dropdown aaDropdown;
    public Dropdown vSyncDropdown;
    public Dropdown gameQuality;
    public Slider volumeSlider;
    public Slider fovSlider;
    public Button btnApplySettings;

    public Text fovPercent;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject advancedMenu;

    public AudioSource musicSource;

    public Resolution[] resolutions;
    public sSettingsClass settings;
    private void Start()
    {
        btnStart.onClick.AddListener(BtnStart);
        btnQuit.onClick.AddListener(BtnQuit);

        settings = new sSettingsClass();

        fullScreenToggle.onValueChanged.AddListener(delegate { OnToggleFullscreen(); });
        advancedToggle.onValueChanged.AddListener(delegate { OnAdvancedToggle(); });
        resDropdown.onValueChanged.AddListener(delegate { OnChangeResolution(); });
        gameQuality.onValueChanged.AddListener(delegate { OnChangeGameQuality(); });
        textureDropdown.onValueChanged.AddListener(delegate { OnChangeTexture(); });
        aaDropdown.onValueChanged.AddListener(delegate { OnChangeAntialiasing(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { OnChangeVSync(); });
        volumeSlider.onValueChanged.AddListener(delegate { OnChangeVolume(); });
        fovSlider.onValueChanged.AddListener(delegate { OnChangeFOV(); });

        btnApplySettings.onClick.AddListener(delegate { SaveUserSettings(); });
        btnOptions.onClick.AddListener(delegate { Options(); });

        advancedToggle.isOn = false;

        resolutions = Screen.resolutions;

        foreach (Resolution resolution in resolutions)
        {
            resDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            gameQuality.options.Add(new Dropdown.OptionData(QualitySettings.names[i].ToString()));
        }

        LoadUserSettings();
    }

    private void BtnStart()
    {
        SceneManager.LoadScene(1);
    }
    private void BtnQuit()
    {
        Application.Quit();
    }

    public void Options()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OnToggleFullscreen()
    {
        settings.fullscreen = Screen.fullScreen = fullScreenToggle.isOn;
    }
    public void OnAdvancedToggle()
    {
        advancedMenu.SetActive(advancedToggle.isOn);
    }


    public void OnChangeResolution()
    {
        settings.resolutionInd = resDropdown.value;
        Screen.SetResolution(resolutions[resDropdown.value].width, resolutions[resDropdown.value].height, Screen.fullScreen);
    }

    public void OnChangeAntialiasing()
    {
        QualitySettings.antiAliasing = settings.antialiasing = (int)Mathf.Pow(2, aaDropdown.value);
    }

    public void OnChangeTexture()
    {
        QualitySettings.masterTextureLimit = settings.textureQual = textureDropdown.value;

    }

    public void OnChangeVSync()
    {
        QualitySettings.vSyncCount = settings.vSync = vSyncDropdown.value;
    }

    public void OnChangeFOV()
    {
        Camera.main.fieldOfView = settings.FOV = fovSlider.value;
        fovPercent.text = fovSlider.value.ToString();
    }

    public void OnChangeVolume()
    {
        musicSource.volume = settings.musicVolume = volumeSlider.value;
    }

    public void OnChangeGameQuality()
    {
        settings.qualityLevel = gameQuality.value;
        QualitySettings.SetQualityLevel(settings.qualityLevel);
        vSyncDropdown.value = QualitySettings.vSyncCount;
        textureDropdown.value = QualitySettings.masterTextureLimit;
        aaDropdown.value = QualitySettings.antiAliasing;
    }

    public void SaveUserSettings()
    {
        string jsonData = JsonUtility.ToJson(settings, true);
        File.WriteAllText(Application.persistentDataPath + "/userSettings.json", jsonData);

        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }


    public void LoadUserSettings()
    {
        if (!File.Exists(Application.persistentDataPath + "/userSettings.json"))
        {
            settings.qualityLevel = 3;
            settings.fullscreen = true;
            settings.textureQual = QualitySettings.masterTextureLimit;
            settings.musicVolume = 0.5f;
            settings.antialiasing = QualitySettings.antiAliasing;
            settings.vSync = QualitySettings.vSyncCount;
            settings.resolutionInd = resolutions.Length;
            settings.FOV = 45;
            SaveUserSettings();
        }

        settings = JsonUtility.FromJson<sSettingsClass>(File.ReadAllText(Application.persistentDataPath + "/userSettings.json"));
        volumeSlider.value = settings.musicVolume;
        aaDropdown.value = settings.antialiasing;
        vSyncDropdown.value = settings.vSync;
        textureDropdown.value = settings.textureQual;
        resDropdown.value = settings.resolutionInd;
        fovSlider.value = settings.FOV;
        fovPercent.text = fovSlider.value.ToString();
        gameQuality.value = settings.qualityLevel;
        Screen.fullScreen = fullScreenToggle.isOn = settings.fullscreen;
        QualitySettings.SetQualityLevel(settings.qualityLevel);
        resDropdown.RefreshShownValue();
        gameQuality.RefreshShownValue();
    }
}
