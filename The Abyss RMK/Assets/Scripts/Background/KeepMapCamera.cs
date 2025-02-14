using UnityEngine;

public class KeepMapCamera : MonoBehaviour
{
    public static KeepMapCamera instance;
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
