using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class ChangeLevel : MonoBehaviour
{
    public static int sceneNum = 0;
    public Transform player;
    public Rigidbody2D playerRb;
    [NonSerialized] public static Vector3 checkpoint;
    //public UIManager canvasManager;
    //public SoundManagerScript soundManager;

    private Dictionary<string, System.Action<Vector3>> collisionHandlers;

    private void Start()
    {
        // Inicializar el diccionario de manejadores de colisiones
        collisionHandlers = new Dictionary<string, System.Action<Vector3>>
        {
            { "Demo", _ => HandleDemo() },
            { "NextLevel", position => LoadScene(sceneNum + 1, new Vector3(position.x, player.position.y, player.position.z)) },
            { "ReturnLevel", position => LoadScene(sceneNum - 1, new Vector3(-position.x, player.position.y, player.position.z)) },
            { "NextLevelUp", position => LoadScene(sceneNum + 1, new Vector3(player.position.x, position.y, player.position.z)) },
            { "ReturnLevelDown", position => LoadScene(sceneNum - 1, new Vector3(player.position.x, position.y + 7, player.position.z)) },
            { "Final", _ => HandleFinal() }
        };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionHandlers.TryGetValue(collision.gameObject.name, out var handler))
        {
            Vector3 position = GetLoadPJ();
            handler(position);
        }
    }

    private void LoadScene(int newSceneNum, Vector3 newPosition)
    {
        sceneNum = newSceneNum;
        SceneManager.LoadScene(sceneNum);
        player.position = newPosition;
    }
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
        GameObject loadPj = GameObject.Find("LoadPj");
        return loadPj != null ? loadPj.transform.position : checkpoint;
    }

    public static Vector3 GetCheckpoint()
    {
        GameObject checkpointObj = GameObject.Find("Checkpoint");
        return checkpointObj != null ? checkpointObj.transform.position : checkpoint;
    }
}
