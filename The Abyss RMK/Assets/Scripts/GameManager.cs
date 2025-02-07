using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    private static int _deaths;
    private float timer;
    public static string timerText;
    public static bool timerActive = false;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        _deaths = 0;

        Invoke(nameof(LoadLocale), 0.1f);
        SoundManagerScript.soundManagerScript.SetVolumeMusic(PlayerPrefs.GetFloat("volume"));
    }

    private void LoadLocale()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[GetLanguage()];
    }

    public void StartTimer()
    {
        timerActive = true;
    }
    public void StopTime()
    {
        timerActive = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (timerActive == true)
        {
            timer += Time.deltaTime;
        }
        else
        {
            TimeSpan time = TimeSpan.FromSeconds(timer);
            timerText = time.Minutes.ToString() + ":" + time.Seconds.ToString();
        }
        Debug.Log("Bool time: "+timerActive);
        Debug.Log(timerText);
    }
    public void SaveLanguage(int language)
    {
        PlayerPrefs.SetInt("language",language);
    }
    public int GetLanguage()
    {
        return PlayerPrefs.GetInt("language");
    }
    public void AddDeath()
    {
        _deaths++;
    }
    public int GetDeaths()
    {
        return _deaths;
    }
    public string GetTime()
    {
        return timerText;
    }
}
