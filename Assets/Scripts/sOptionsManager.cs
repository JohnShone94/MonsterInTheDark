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
    public AudioSource otherSource;
    public AudioClip lullaby;
    public Resolution[] resolutions;
    public sSettingsClass settings;

    private void OnEnable()
    {
        //Createing a reference to the settings class.
        settings = new sSettingsClass();
        //Setting up the onValueChanged events.
        fullScreenToggle.onValueChanged.AddListener(delegate { OnToggleFullscreen(); });
        advancedToggle.onValueChanged.AddListener(delegate { OnAdvancedToggle(); });
        resDropdown.onValueChanged.AddListener(delegate { OnChangeResolution(); });
        gameQuality.onValueChanged.AddListener(delegate { OnChangeGameQuality(); });
        textureDropdown.onValueChanged.AddListener(delegate { OnChangeTexture(); });
        aaDropdown.onValueChanged.AddListener(delegate { OnChangeAntialiasing(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { OnChangeVSync(); });
        volumeSlider.onValueChanged.AddListener(delegate { OnChangeVolume(); });
        fovSlider.onValueChanged.AddListener(delegate { OnChangeFOV(); });
        //Setting up the onClick events.
        btnApplySettings.onClick.AddListener(delegate { SaveUserSettings(); });
        btnResume.onClick.AddListener(delegate { ResumeGame(); });
        btnRestart.onClick.AddListener(delegate { RestartGame(); });
        btnOptions.onClick.AddListener(delegate { Options(); });
        btnEgOptions.onClick.AddListener(delegate { EgOptions(); });
        btnReturnToMM.onClick.AddListener(delegate { ReturnToMM(); });
        btnEgReturnToMM.onClick.AddListener(delegate { EgReturnToMM(); });
        //Make sure that the advanced options are not on.
        advancedToggle.isOn = false;
        //create a reference to the resolutions array.
        resolutions = Screen.resolutions;

        foreach(Resolution resolution in resolutions)
        {
            //add each resolution to the dropdown box.
            resDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        for(int i = 0; i < QualitySettings.names.Length; i++)
        {
            //add each quality level to the game quality dropdown box.
            gameQuality.options.Add(new Dropdown.OptionData(QualitySettings.names[i].ToString()));
        }
        //finally we need to load the users settings.
        LoadUserSettings();
    }

    public void LoadEndGame()
    {
        //First we activate the endgamemenu.
        endGameMenu.SetActive(true);
        //next we unlock the cursor.
        Cursor.lockState = CursorLockMode.None;
        //next we set the cursor to visible.
        Cursor.visible = true;
        //then we pause the game.
        Time.timeScale = 0.0f;
        //next we tell the player that it cant take input
        cc.bCanTakeInput = false;
        //finally we set the music clip to the lullaby song and tell it to play.
        musicSource.clip = lullaby;
        musicSource.Play();
    }

    public void OnToggleFullscreen()
    {
        //We set the players fullscreen to equal the toggle option, and then the settings to equal the players fullscreen.
        settings.fullscreen = Screen.fullScreen = fullScreenToggle.isOn;
    }
    public void OnAdvancedToggle()
    {
        //We set the advanced options to active and deactive depending on the value of the advanced toggle option.
        advancedMenu.SetActive(advancedToggle.isOn);
    }

    public void OnChangeResolution()
    {
        //First we set the settings class resoultion index to equal that of the dropdownbox value.
        settings.resolutionInd = resDropdown.value;
        //then we set the screens resolutions to equal that of the dropdownbox value.
        Screen.SetResolution(resolutions[resDropdown.value].width, resolutions[resDropdown.value].height, Screen.fullScreen);
    }

    public void OnChangeAntialiasing()
    {
        //first we set the settings class antialiasing value to be that of the dropdownbox value to the power of 2.
        //then we set the QualitySettings antialiasing to equal that of the settings class.
        QualitySettings.antiAliasing = settings.antialiasing = (int)Mathf.Pow(2, aaDropdown.value);
    }

    public void OnChangeTexture()
    {
        //First we set the settings class texture quality to equal the value of the texture dropdown box.
        //and then we set the QualitySettings texture quality to equal the value of the settings class texture quality.
        QualitySettings.masterTextureLimit = settings.textureQual = textureDropdown.value;
    }

    public void OnChangeVSync()
    {
        //first we set the settings class vsync to equal that of the vsync dropdown value.
        //then we set the QualitySettings vsync value to equal that of the settings class vsync
        QualitySettings.vSyncCount = settings.vSync = vSyncDropdown.value;
    }

    public void OnChangeVolume()
    {
        //first we set the settings class musicvolume to equal the volumeslider value.
        //then we set the musicSources volume to equal that of the settings class.
        musicSource.volume = settings.musicVolume = volumeSlider.value;
        otherSource.volume = settings.musicVolume = volumeSlider.value;
    }

    public void OnChangeFOV()
    {
        //first we set the settings class FOV to equal that of the sliders value.
        //then we set the Camera FOV to equal that of the settings class.
        Camera.main.fieldOfView = settings.FOV = fovSlider.value;
        //finally we update the text box to equal the value of the fovslider.
        fovPercent.text = fovSlider.value.ToString();
    }

    public void OnChangeGameQuality()
    {
        //first we set the settings class quality level to equal the value of the gameQuality dropdownbox
        settings.qualityLevel = gameQuality.value;
        //then we set the QualitySettings quality level to equal the settings class quality level.
        QualitySettings.SetQualityLevel(settings.qualityLevel);
        //Next we update the vsync, texture quality and antialiasing dropdown boxes.
        vSyncDropdown.value = QualitySettings.vSyncCount;
        textureDropdown.value = QualitySettings.masterTextureLimit;
        aaDropdown.value = QualitySettings.antiAliasing;
    }

    public void ResumeGame()
    {
        //we deactivate the pausemenu.
        pauseMenu.SetActive(false);
        //then we relock the cursor.
        Cursor.lockState = CursorLockMode.Locked;
        //then we turn off the visability of the cursor.
        Cursor.visible = false;
        //next we unpause the game.
        Time.timeScale = 1.0f;
        //finally we tell the play it can take input again.
        cc.bCanTakeInput = true;
    }
    public void RestartGame()
    {
        //We load the player scene.
        SceneManager.LoadScene(1);
    }

    public void ReturnToMM()
    {
        //We load the main menu scene.
        SceneManager.LoadScene(0);
    }
    public void EgReturnToMM()
    {
        //We load the main menu scene.
        SceneManager.LoadScene(0);
    }

    public void Options()
    {
        //Activate the options menu and deactivate the pause menu.
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void EgOptions()
    {
        //first we set the Endgame boolean to true.
        endGame = true;
        //next we deactivate the end game menu and activate the options menu.
        endGameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void SaveUserSettings()
    {
        //next we save the user settings by creating a json file.
        string jsonData = JsonUtility.ToJson(settings, true);
        File.WriteAllText(Application.persistentDataPath + "/userSettings.json", jsonData);

        //We have to check if the player is looking at the endgame or not.
        if (endGame)
        {
            //if so then we activate the endgame menu and deactivate the options menu.
            //finally setting endgame to equal false.
            endGameMenu.SetActive(true);
            optionsMenu.SetActive(false);
            endGame = false;
        }
        else
        {
            //if not then we activate the pause menu and deactivate the options menu.
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
    }

    public void LoadUserSettings()
    {
        //first we check to see if a settings file exists and if not then we manually,
        //set all the settings class values and run the save settings function to create a file.
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
        //once that is done we load the values from the json file and update all the options values.
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