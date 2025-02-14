using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

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
    [Header("Pause")]
    [SerializeField] private GameObject mapMenu;
    [SerializeField] private RawImage _mapImage;
    [Header("Settings")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider soundsSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [DoNotSerialize] public int language;
    private GameObject previousMenu;
    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueMenu;
    [Header("Demo")]
    [SerializeField] private GameObject demoMenu;
    [SerializeField] private TMP_Text deathDemoText;
    [SerializeField] private TMP_Text timeDemoText;
    [Header("Controller Input")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsFirst;
    [SerializeField] private GameObject _pauseFirst;
    [SerializeField] private GameObject _dialogueFirst;
    [SerializeField] private GameObject _demoFirst;


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
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.PauseGameInput)
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
        if (InputManager.instance.MapeInput)
        {
            if (!mapMenu.activeInHierarchy)
            {
                OpenMap();
            }
            else
            {
                CloseMap();
            }
        }
    }

    public void DisableStart()
    {
        GameManager.gameManager.StartTimer();
        SoundManagerScript.soundManagerScript.StartMusic();
        startMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

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
        EventSystem.current.SetSelectedGameObject(_settingsFirst);

    }
    public void GoBack()
    {
        settingsMenu.SetActive(false);
        previousMenu.SetActive(true);
        if (previousMenu.name == "PauseMenu") EventSystem.current.SetSelectedGameObject(_pauseFirst);
        else if (previousMenu.name == "StartMenu") EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
        previousMenu = null;

    }
    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    public void SetVolume(float volume)
    {
        SoundManagerScript.soundManagerScript.SetVolumeMusic(volume);
        PlayerPrefs.SetFloat("volume",volume);
    }
    public void SetVolumeSounds(float volume)
    {
        SoundManagerScript.soundManagerScript.SetSoundsMusic(volume);
    }

    public void ChangeLanguage(int indexLanguage)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[indexLanguage];
        language = indexLanguage;
        GameManager.gameManager.SaveLanguage(language);
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
        if (SoundManagerScript.soundManagerScript.GetVolumeMusic() != 0f) SoundManagerScript.soundManagerScript.SetVolumeMusic(PlayerPrefs.GetFloat("volume"));
        GameManager.gameManager.StartTimer();
        EventSystem.current.SetSelectedGameObject(null);

    }
    public void Pause()
    {
        Debug.Log("Pause");
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_pauseFirst);

        Time.timeScale = 0f;
        pausedGame = true;
        if (SoundManagerScript.soundManagerScript.GetVolumeMusic() != 0f) SoundManagerScript.soundManagerScript.SetVolumeMusic(0.05f);
        GameManager.gameManager.StopTime();
    }
    
    #endregion 

    public void StartMenuDialogue()
    {
        Time.timeScale = 0f;
        dialogueMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_dialogueFirst);

    }

    public void HideMenuDialogue()
    {
        Time.timeScale = 1f;
        dialogueMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

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
        EventSystem.current.SetSelectedGameObject(_demoFirst);
        deathDemoText.text = "Deaths: " + GameManager.gameManager.GetDeaths();
        timeDemoText.text = "Total Time: " + GameManager.gameManager.GetTime();
    }

    public void OpenMap()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (activeSceneIndex)
        {
            case 0:
                _mapImage.texture = (Texture) Resources.Load("Map/Level1Map");
                StartCoroutine(ShowMap());
                return;
            case 1:
                _mapImage.texture = (Texture) Resources.Load("Map/Level2Map");
                StartCoroutine(ShowMap());

                return;
            case 2:
                _mapImage.texture = (Texture) Resources.Load("Map/Level3Map");
                StartCoroutine(ShowMap());

                return;
            default:
                _mapImage.texture = (Texture)Resources.Load("Map/Level1Map");
                StartCoroutine(ShowMap());
                return;
        }

        
        
    }
    IEnumerator ShowMap()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("LlegaAqui");
        Time.timeScale = 0;
        mapMenu.SetActive(true);
    }

    public void CloseMap()
    {
        Time.timeScale = 1;
        mapMenu.SetActive(false);
    }
}
