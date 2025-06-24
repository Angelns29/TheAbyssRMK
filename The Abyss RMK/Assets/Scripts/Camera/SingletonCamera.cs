using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonCamera : MonoBehaviour
{
    public static SingletonCamera instance;
    public CinemachineCamera _camera;
    public CinemachinePositionComposer _positionComposer;
    private int _currentSceneIndex = 0; // Almacena el índice de la escena actual

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else Destroy(gameObject);
        _camera = GetComponent<CinemachineCamera>();
        _positionComposer = GetComponent<CinemachinePositionComposer>();
    }
    private void Update()
    {
        // Verificar si la escena ha cambiado
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (activeSceneIndex != _currentSceneIndex)
        {
            _currentSceneIndex = activeSceneIndex;
            ConfigureCamera(); // Reconfigurar la cámara si la escena ha cambiado
        }

    }
    public void ConfigureCamera()
    {
        Debug.Log(_currentSceneIndex);
        if (_currentSceneIndex >= 1 && _currentSceneIndex != 2)
        {
            _camera.Lens.OrthographicSize = 9;
            _positionComposer.Composition.ScreenPosition = new Vector2(-0.20f, 0);
        }
        else if (_currentSceneIndex == 2)
        {
            _positionComposer.Composition.ScreenPosition = new Vector2(0.1f, 0);
        }
    }
    public void DisablePlayer()
    {
        _camera.Follow = null;
    }
    public void ResetPlayer()
    {
        _camera.Follow = GameObject.Find("AbyssWalker").transform;
    }
}
