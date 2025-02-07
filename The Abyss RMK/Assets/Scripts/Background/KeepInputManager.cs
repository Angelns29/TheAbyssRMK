using UnityEngine;

public class KeepInputManager : MonoBehaviour
{
    public static KeepInputManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else Destroy(gameObject);
    }
}
