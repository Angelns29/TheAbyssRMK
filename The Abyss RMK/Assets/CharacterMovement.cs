using System;
using System.Collections;
using Unity.VisualScripting;
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
    private int _gravity = 4;
    private bool gravityChanged = false;
    public GameObject GroundCheck;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb.gravityScale = _gravity;
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
        if (_jumpInput) StartCoroutine(CheckGravity());

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

    IEnumerator CheckGravity()
    {
        if (IsGrounded() && gravityChanged == false)
        {
            _spriteRenderer.flipY = true;
            _groundCheck.localPosition = new Vector3(0.32f, 0.85f, 0);
            _gravity *= -1;
            _rb.gravityScale = _gravity;
            gravityChanged = true;
        }
        else if (IsGrounded() && gravityChanged == true)
        {
            _spriteRenderer.flipY = false;
            _groundCheck.localPosition = new Vector3(0.32f, -0.95f, 0);
            _gravity *= -1;
            _rb.gravityScale = _gravity;

        }
        yield return new WaitForSeconds(1);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
        
    }
}
