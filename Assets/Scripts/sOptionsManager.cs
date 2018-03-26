using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class sOptionsManager : MonoBehaviour
{


    public Toggle fullScreenToggle;
    public Dropdown resDropdown;
    public Dropdown textureDropdown;
    public Dropdown aaDropdown;
    public Dropdown vSyncDropdown;
    public Slider volumeSlider;
    public Button btnApplySettings;
    public Button btnResume;
    public Button btnRestart;
    public Button btnOptions;
    public Button btnEgOptions;
    public Button btnReturnToMM;
    public Button btnEgReturnToMM;
    public sCharacterController cc;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject endGameMenu;

    public bool endGame;

    public AudioSource musicSource;
    public AudioClip lullaby;

    public Resolution[] resolutions;
    public sSettingsClass settings;

    private void OnEnable()
    {
        settings = new sSettingsClass();

        fullScreenToggle.onValueChanged.AddListener(delegate { OnToggleFullscreen(); });
        resDropdown.onValueChanged.AddListener(delegate { OnChangeResolution(); });
        textureDropdown.onValueChanged.AddListener(delegate { OnChangeTexture(); });
        aaDropdown.onValueChanged.AddListener(delegate { OnChangeAntialiasing(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { OnChangeVSync(); });
        volumeSlider.onValueChanged.AddListener(delegate { OnChangeVolume(); });

        btnApplySettings.onClick.AddListener(delegate { SaveUserSettings(); });
        btnResume.onClick.AddListener(delegate { ResumeGame(); });
        btnRestart.onClick.AddListener(delegate { RestartGame(); });
        btnOptions.onClick.AddListener(delegate { Options(); });
        btnEgOptions.onClick.AddListener(delegate { EgOptions(); });
        btnReturnToMM.onClick.AddListener(delegate { ReturnToMM(); });
        btnEgReturnToMM.onClick.AddListener(delegate { EgReturnToMM(); });

        resolutions = Screen.resolutions;

        foreach(Resolution resolution in resolutions)
        {
            resDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
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