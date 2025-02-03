using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    public static UICanvas instance;
    public bool pausedGame = false;
    public ChangeLevel changeLevel;

    [DoNotSerialize] private GameObject _player;
    [DoNotSerialize] private GameObject _background;

    [Header("Start")]
    [SerializeField] private GameObject startMenu;
    [Header("Pause")]
    [SerializeField] private GameObject pauseMenu;
    [Header("Settings")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider soundsSlider;
    [SerializeField] private Toggle fullscreenToggle;
    private GameObject previousMenu;
    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueMenu;
    [Header("Demo")]
    [SerializeField] private GameObject demoMenu;
    [SerializeField] private TMP_Text deathDemoText;
    [SerializeField] private TMP_Text timeDemoText;
    [Header("Final")]
    [SerializeField] private GameObject finalMenu;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private TMP_Text timeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        _player = GameObject.Find("Character");
        _background = GameObject.Find("Background");


        volumeSlider.value = SoundManagerScript.soundManagerScript.GetVolumeMusic();
        soundsSlider.value = SoundManagerScript.soundManagerScript.GetSoundMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            if (pauseMenu.activeInHierarchy)
            {
                PauseGame(true);
                //Resume();
            }
            else
            {
                PauseGame(false);
                Pause();
            }
        }
    }

    public void DisableStart()
    {
        GameManager.gameManager.StartTimer();
        SoundManagerScript.soundManagerScript.StartMusic();
        startMenu.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif

    }
    #region settings
    public void OnSettingsButtonClicked()
    {
        OpenSettings(startMenu);
    }
    public void OnSettingsButtonClickedFromPause()
    {
        OpenSettings(pauseMenu);
    }
    public void OpenSettings(GameObject menuToClose)
    {
        previousMenu = menuToClose;
        if (menuToClose != null)
        {
            menuToClose.SetActive(false); // Cierra el menú actual
        }
        settingsMenu.SetActive(true);     // Abre el menú de configuración
    }
    public void GoBack()
    {
        settingsMenu.SetActive(false);
        previousMenu.SetActive(true);
        previousMenu = null;
    }
    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    public void SetVolume(float volume)
    {
        SoundManagerScript.soundManagerScript.SetVolumeMusic(volume);
    }
    public void SetVolumeSounds(float volume)
    {
        SoundManagerScript.soundManagerScript.SetSoundsMusic(volume);
    }
    #endregion

    #region Pause
    private void PauseGame(bool status)
    {
        pauseMenu.SetActive(status);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        pausedGame = false;
        SoundManagerScript.soundManagerScript.SetVolumeMusic(0.4f);
        GameManager.gameManager.StartTimer();
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        pausedGame = true;
        SoundManagerScript.soundManagerScript.SetVolumeMusic(0.05f);
        GameManager.gameManager.StopTime();
    }
    
    #endregion 

    public void StartMenuDialogue()
    {
        Time.timeScale = 0f;
        dialogueMenu.SetActive(true);
    }

    public void HideMenuDialogue()
    {
        Time.timeScale = 1f;
        dialogueMenu.SetActive(false);
    }

    public void ShowDemoEnd()
    {
        Time.timeScale = 0f;
        GameManager.gameManager.StopTime();
        StartCoroutine(ShowData());
    }
    IEnumerator ShowData()
    {
        yield return new WaitForEndOfFrame();
        demoMenu.SetActive(true);
        deathDemoText.text = "Deaths: " + GameManager.gameManager.GetDeaths();
        timeDemoText.text = "Total Time: " + GameManager.gameManager.GetTime();
    }
}
