using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class sOptionsManager : MonoBehaviour
{


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
    public Button btnResume;
    public Button btnRestart;
    public Button btnOptions;
    public Button btnEgOptions;
    public Button btnReturnToMM;
    public Button btnEgReturnToMM;
    public sCharacterController cc;

    public Text fovPercent;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject endGameMenu;
    public GameObject advancedMenu;

    public bool endGame;

    public AudioSource musicSource;
    public AudioClip lullaby;
    public Resolution[] resolutions;
    public sSettingsClass settings;

    private void OnEnable()
    {
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
        btnResume.onClick.AddListener(delegate { ResumeGame(); });
        btnRestart.onClick.AddListener(delegate { RestartGame(); });
        btnOptions.onClick.AddListener(delegate { Options(); });
        btnEgOptions.onClick.AddListener(delegate { EgOptions(); });
        btnReturnToMM.onClick.AddListener(delegate { ReturnToMM(); });
        btnEgReturnToMM.onClick.AddListener(delegate { EgReturnToMM(); });

        advancedToggle.isOn = false;

        resolutions = Screen.resolutions;

        foreach(Resolution resolution in resolutions)
        {
            resDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        for(int i = 0; i < QualitySettings.names.Length; i++)
        {
            gameQuality.options.Add(new Dropdown.OptionData(QualitySettings.names[i].ToString()));
        }

        LoadUserSettings();
    }

    public void LoadEndGame()
    {
        endGameMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0.0f;
        cc.bCanTakeInput = false;

        musicSource.clip = lullaby;
        musicSource.Play();
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

    public void OnChangeVolume()
    {
        musicSource.volume = settings.musicVolume = volumeSlider.value;
    }

    public void OnChangeFOV()
    {
        Camera.main.fieldOfView = settings.FOV = fovSlider.value;
        fovPercent.text = fovSlider.value.ToString();
    }

    public void OnChangeGameQuality()
    {
        settings.qualityLevel = gameQuality.value;
        QualitySettings.SetQualityLevel(settings.qualityLevel);
        vSyncDropdown.value = QualitySettings.vSyncCount;
        textureDropdown.value = QualitySettings.masterTextureLimit;
        aaDropdown.value = QualitySettings.antiAliasing;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        cc.bCanTakeInput = true;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ReturnToMM()
    {
        SceneManager.LoadScene(0);
    }
    public void EgReturnToMM()
    {
        SceneManager.LoadScene(0);
    }

    public void Options()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void EgOptions()
    {
        endGame = true;
        endGameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void SaveUserSettings()
    {
        string jsonData = JsonUtility.ToJson(settings, true);
        File.WriteAllText(Application.persistentDataPath + "/userSettings.json", jsonData);

        if (endGame)
        {
            endGameMenu.SetActive(true);
            optionsMenu.SetActive(false);
            endGame = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
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