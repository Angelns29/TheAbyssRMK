using System;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    private InputSystem_Actions _inputActions;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _moveInput;

    private bool _jumpInput;
    private bool gravityChanged = false;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        _inputActions.Player.Enable();        
    }
    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Running Input
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        CheckRunning(_moveInput);
        //Change Gravity
        _inputActions.Player.Jump.performed += i => _jumpInput = true;
        _inputActions.Player.Jump.canceled += i => _jumpInput = false;
        CheckGravity(_jumpInput);

    }



    private void CheckRunning(Vector2 moveInput)
    {
        if (_moveInput.x != 0)
        {
            _animator.SetBool("isRunning", true);
            if (_moveInput.x < 0) _spriteRenderer.flipX = true;
            else _spriteRenderer.flipX = false;
        }
        else _animator.SetBool("isRunning", false);
        _moveInput.y = 0f;
        _rb.linearVelocity = _moveInput * speed;
    }

    private void CheckGravity(bool jumpInput)
    {
        Debug.Log(IsGrounded());
        if (jumpInput && IsGrounded() && gravityChanged==false)
        {
            _groundCheck.position.Set(0.33f,0.81f, 0);
            //_groundCheck.SetPositionAndRotation(new Vector3(0.33f,0.81f,0),Quaternion.identity);
            _rb.gravityScale = -4f;
            _spriteRenderer.flipY = true;
            gravityChanged = true;
        }
        else if (jumpInput && IsGrounded() && gravityChanged == true) 
        {
            _rb.gravityScale = 4f;
            _spriteRenderer.flipY = false;
            //_groundCheck.SetPositionAndRotation(new Vector3(0.33f, -0.94f, 0), Quaternion.identity);
            _groundCheck.position.Set(0.33f, -0.94f, 0);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
        
    }
}
