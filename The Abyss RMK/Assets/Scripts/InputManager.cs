using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public bool PauseGameInput { get; private set; }

    private InputAction _pauseGameAction;
    private PlayerInput _playerInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _playerInput = GetComponent<PlayerInput>();
        _pauseGameAction = _playerInput.actions["PauseGame"];
    }

    // Update is called once per frame
    private void Update()
    {
        PauseGameInput = _pauseGameAction.WasPressedThisFrame();
    }
}
