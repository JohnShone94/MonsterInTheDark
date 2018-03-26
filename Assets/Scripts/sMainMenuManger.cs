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
    public Dropdown resDropdown;
    public Dropdown textureDropdown;
    public Dropdown aaDropdown;
    public Dropdown vSyncDropdown;
    public Slider volumeSlider;
    public Button btnApplySettings;

    public GameObject pauseMenu;
    public GameObject optionsMenu;

    public AudioSource musicSource;

    public Resolution[] resolutions;
    public sSettingsClass settings;
    private void Start()
    {
        btnStart.onClick.AddListener(BtnStart);
        btnQuit.onClick.AddListener(BtnQuit);

        settings = new sSettingsClass();

        fullScreenToggle.onValueChanged.AddListener(delegate { OnToggleFullscreen(); });
        resDropdown.onValueChanged.AddListener(delegate { OnChangeResolution(); });
        textureDropdown.onValueChanged.AddListener(delegate { OnChangeTexture(); });
        aaDropdown.onValueChanged.AddListener(delegate { OnChangeAntialiasing(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { OnChangeVSync(); });
        volumeSlider.onValueChanged.AddListener(delegate { OnChangeVolume(); });

        btnApplySettings.onClick.AddListener(delegate { SaveUserSettings(); });
        btnOptions.onClick.AddListener(delegate { Options(); });

        resolutions = Screen.resolutions;

        foreach (Resolution resolution in resolutions)
        {
            resDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
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

    public void SaveUserSettings()
    {
        string jsonData = JsonUtility.ToJson(settings, true);
        File.WriteAllText(Application.persistentDataPath + "/userSettings.json", jsonData);

        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void OnToggleFullscreen()
    {
        settings.fullscreen = Screen.fullScreen = fullScreenToggle.isOn;
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

    public void OnChangeVolume()
    {
        musicSource.volume = settings.musicVolume = volumeSlider.value;
    }

    public void LoadUserSettings()
    {
        settings = JsonUtility.FromJson<sSettingsClass>(File.ReadAllText(Application.persistentDataPath + "/userSettings.json"));
        volumeSlider.value = settings.musicVolume;
        aaDropdown.value = settings.antialiasing;
        vSyncDropdown.value = settings.vSync;
        textureDropdown.value = settings.textureQual;
        resDropdown.value = settings.resolutionInd;
        Screen.fullScreen = fullScreenToggle.isOn = settings.fullscreen;
        resDropdown.RefreshShownValue();
    }
}
