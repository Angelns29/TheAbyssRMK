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
        if (_currentSceneIndex >= 1 && _currentSceneIndex != 3)
        {
            _camera.Lens.OrthographicSize = 10;
            _positionComposer.Composition.ScreenPosition = new Vector2(-0.8f,-0.5f);
            /*_positionComposer.Composition.ScreenPosition.y = -0.5f;
            _positionComposer.Composition.ScreenPosition.x = -0.8f;*/
        }
        else if (_currentSceneIndex == 3)
        {
            _positionComposer.Composition.ScreenPosition = new Vector2(0, -0.5f);
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
