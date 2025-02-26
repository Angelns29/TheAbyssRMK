using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class ChangeLevel : MonoBehaviour
{
    public static ChangeLevel instance;
    [SerializeField] public int sceneNum = 0;
    public Transform player;
    public Rigidbody2D playerRb;
    [NonSerialized] public Vector3 checkpoint;
    //public UIManager canvasManager;
    //public SoundManagerScript soundManager;

    private Dictionary<string, System.Action<Vector3>> collisionHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        // Inicializar el diccionario de manejadores de colisiones
        collisionHandlers = new Dictionary<string, System.Action<Vector3>>
        {
            { "Demo", _ => HandleDemo() },
            { "NextLevel", position => StartCoroutine(LoadScene(sceneNum + 1, position)) },
            { "Final", _ => HandleFinal() }
        };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionHandlers.TryGetValue(collision.gameObject.name, out var handler))
        {
            Vector3 position = GetLoadPJ();
            handler(position);
            sceneNum += 1;
        }

        
    }
    private IEnumerator LoadScene(int sceneIndex, Vector3 position)
    {
        // Cargar la escena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        // Esperar a que la escena termine de cargar
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Esperar un frame extra para asegurarse de que todo está listo
        yield return new WaitForEndOfFrame();

        // Ahora que la escena está cargada, actualizar referencias
        SetCheckpointAndTeleport();
    }

    private void SetCheckpointAndTeleport()
    {
        Transform checkpoint = GameObject.Find("FirstCheckpoint")?.transform;
        if (checkpoint != null)
        {
            PlayerLife.instance.SetCheckpoint(checkpoint);
            if (player != null)
            {
                player.position = checkpoint.position;
            }
        }
        else
        {
            Debug.LogError("No se encontró el FirstCheckpoint en la nueva escena.");
        }

        //ChangeMinimapLevel.instance.SetCurrentLevel(sceneNum);
    }

    /*private void LoadScene(int newSceneNum, Vector3 newPosition)
    {
        sceneNum = newSceneNum;
        SceneManager.LoadScene(sceneNum);
        player.position = newPosition;
    }*/
    private void HandleDemo()
    {
        SoundManagerScript.soundManagerScript.PlayFinalSong();
        UICanvas.instance.ShowDemoEnd();
    }
    private void HandleFinal()
    {
        //soundManager.PlayFinalSong();
        //canvasManager.EndGame();
    }

    /*public void PlayAgain()
    {
        if (UIManager.uiManager.resetGame)
        {
            sceneNum = 0;
            SceneManager.LoadScene(sceneNum);
            StartCoroutine(ResetGame());
        }
    }*/

    private IEnumerator ResetGame()
    {
        yield return null;
        player.position = GetCheckpoint();
    }

    private Vector3 GetLoadPJ()
    {
        GameObject loadPj = GameObject.Find("FirstCheckpoint");
        return loadPj != null ? loadPj.transform.position : checkpoint;
    }

    public Vector3 GetCheckpoint()
    {
        GameObject checkpointObj = GameObject.Find("Checkpoint");
        return checkpointObj != null ? checkpointObj.transform.position : checkpoint;
    }
}
