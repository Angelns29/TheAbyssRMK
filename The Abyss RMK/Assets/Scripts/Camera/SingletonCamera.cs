using Unity.Cinemachine;
using UnityEngine;

public class SingletonCamera : MonoBehaviour
{
    public static SingletonCamera instance;
    public CinemachineCamera _camera;
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

}
