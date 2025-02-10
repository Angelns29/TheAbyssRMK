using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public static CharacterMovement instance;
    public float speed;
    private InputSystem_Actions _inputActions;
    private Rigidbody2D _rb;
    private Animator _animator;
    public SpriteRenderer spriteRenderer;
    private Vector2 _moveInput;

    private bool _jumpInput;
    public bool interactInput;
    [DoNotSerialize] public int gravity = 4;
    [DoNotSerialize]public bool gravityChanged = false;
    public GameObject GroundCheck;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        _inputActions = new InputSystem_Actions();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _rb.gravityScale = gravity;

        
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    void FixedUpdate()
    {
        // Running Input
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        CheckRunning(_moveInput);

        // Change Gravity
        _inputActions.Player.Jump.performed += i => _jumpInput = true;
        _inputActions.Player.Jump.canceled += i => _jumpInput = false;

        if (_jumpInput && IsGrounded())
        {
            ChangeGravity();
            _jumpInput = false; // Reset jump input to avoid multiple gravity changes
        }

        //Dialogues
        _inputActions.Player.Interact.performed += i => interactInput = true;
        _inputActions.Player.Interact.canceled += i => interactInput = false;

        
    }

    public bool CheckDialogue()
    {
        return interactInput;
    }
    private void CheckRunning(Vector2 moveInput)
    {
        if (_moveInput.x != 0)
        {
            _animator.SetBool("isRunning", true);
            spriteRenderer.flipX = _moveInput.x < 0;
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }
        _moveInput.y = 0f;
        _rb.linearVelocity = new Vector2(_moveInput.x * speed, _rb.linearVelocity.y);
    }

    public void ChangeGravity()
    {
        gravityChanged = !gravityChanged;
        spriteRenderer.flipY = gravityChanged;
        _groundCheck.localPosition = new Vector3(0.32f, gravityChanged ? 0.85f : -0.95f, 0);
        gravity *= -1;
        _rb.gravityScale = gravity;
    }
    public void SetGravity(int gravity)
    {
        _rb.gravityScale = gravity;
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }

}
