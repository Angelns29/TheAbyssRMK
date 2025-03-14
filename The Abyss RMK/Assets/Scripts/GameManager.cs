using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    private static int _deaths;
    private float timer;
    public static string timerText;
    public static bool timerActive = false;

    //Collectables 
    private Dictionary<string, bool> collectables = new Dictionary<string, bool>(); //Dicionario donde se almacenarán todos los booleanos de los diferentes collectables
    public int recollectedCollectables = 0;
    public int totalCollectables;
    public bool allCollectablesRecollected = false;
    public TextMeshProUGUI collectablesText;
    public GameObject canvasCollectable;

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

        //Añadimos al diccionario los collecionables que tenemos en el nivel
        collectables.Add("collectableLevel0", false);
        collectables.Add("collectableLevel1", false);
        collectables.Add("collectableLevel21", false);
        collectables.Add("collectableLevel22", false);
        collectables.Add("collectableLevel31", false);
        collectables.Add("collectableLevel32", false);
        collectables.Add("collectableLevel41", false);
        collectables.Add("collectableLevel42", false);
        collectables.Add("collectableLevel43", false);


        totalCollectables = collectables.Count;
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
        //Debug.Log("Bool time: "+timerActive);
        //Debug.Log(timerText);
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

    #region collectables
    

    public int GetTotalCollectables()
    {
        return totalCollectables;
    }
    public int GetRecollectedCollectables()
    {
        return recollectedCollectables;
    }
    public void SetCollectableTrue(string name)
    {
        if (collectables.ContainsKey(name) && !collectables[name])
        {
            collectables[name] = true; // Marcar el coleccionable como recolectado
            recollectedCollectables++; // Incrementar el contador de coleccionables recolectados
            Debug.Log("Recolectado");
            Debug.Log(recollectedCollectables.ToString() + "/" + totalCollectables.ToString());

            // Verificar si todos los coleccionables han sido recolectados
            if (recollectedCollectables >= totalCollectables)
            {
                allCollectablesRecollected = true;
            }
        }
    }
    public void ShowCollectablesCount()
    {
        collectablesText.text = recollectedCollectables + "/" + totalCollectables;
        canvasCollectable.SetActive(true);

        StartCoroutine(DisableCollectablesCount());
    }

    IEnumerator DisableCollectablesCount()
    {
        yield return new WaitForSeconds(2);
        canvasCollectable.SetActive(false);
    }
    #endregion
}
