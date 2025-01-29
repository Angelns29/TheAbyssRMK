using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    private Transform _player;
    private SpriteRenderer _sr;
    //private SoundManagerScript _soundManager;
    //private ChangeLevel _changeLevel;
    private RigidbodyConstraints2D _originalConstraints;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _player = transform;
        _sr = GetComponent<SpriteRenderer>();
        //_soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManagerScript>();
        //_changeLevel = GetComponent<ChangeLevel>();
        _originalConstraints = _rb.constraints;

        //_player.position = GetCheckpoint();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy"))
        {
            HandleDeath();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (collision.gameObject.CompareTag("Boom"))
        {
            HandleDeath();
        }
        else */
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            //Checkpoint checkpoint = collision.gameObject.GetComponent<Checkpoint>();
            //if (checkpoint != null)
            //{
            //    checkpoint.Activate();
            //}
        }
    }

    private void HandleDeath()
    {
        if (CharacterMovement.instance.gravityChanged)
        {
            CharacterMovement.instance.ChangeGravity();
        }

        _animator.SetTrigger("isDeath");
        //_soundManager.PlaySFX(_soundManager.death);
        _rb.constraints = RigidbodyConstraints2D.FreezePositionX;

        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1.5f);

        // Restablecer la rotación del jugador si es necesario
        if (_player.rotation.eulerAngles.z != 0)
        {
            _player.rotation = Quaternion.Euler(0, 0, 0);
        }

        //GameManager.AddDeath();
        _rb.constraints = _originalConstraints;
        _player.position = GetCheckpoint();
        _rb.gravityScale = 4;
    }

    public Vector3 GetCheckpoint()
    {
        GameObject checkpoint = GameObject.Find("Checkpoint");
        if (checkpoint != null)
        {
            return checkpoint.transform.position;
        }
        else
        {
            SceneManager.LoadScene(--ChangeLevel.sceneNum);
            return ChangeLevel.checkpoint;
        }

    }
}
