using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    public static PlayerLife instance;
    private Animator _animator;
    private Rigidbody2D _rb;
    private Transform _player;
    private SpriteRenderer _sr;
    private BoxCollider2D _collider;
    private Transform checkpoint;
    private SoundManagerScript _soundManager;
    private ChangeLevel _changeLevel;
    private RigidbodyConstraints2D _originalConstraints;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _player = transform;
        _sr = GetComponent<SpriteRenderer>();
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManagerScript>();
        _changeLevel = GetComponent<ChangeLevel>();
        _originalConstraints = _rb.constraints;
        checkpoint = GameObject.Find("FirstCheckpoint").transform;
        //_player.position = GetCheckpoint();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.tag){
            case "Obstacle":
                StartCoroutine(WaitAndHandleDeath(0));
                return;
            case "Enemy":
                StartCoroutine(WaitAndHandleDeath(1));
                return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            StartCoroutine(WaitAndHandleDeath(0));
        }
        if (collision.gameObject.name == "ResetGravity")
        {
            if (!CharacterMovement.instance.gravityChanged)
            {
                CharacterMovement.instance.SetGravity(0); //Ponemos la gravedad a 0 para que no atraviese el mapa
            }
            else
            {
                CharacterMovement.instance.SetGravity(-2); // Lo ponemos como estaba
            }
        }
        if (collision.gameObject.name == "ResumeGravity")
        {
            if (!CharacterMovement.instance.gravityChanged)
            {
                CharacterMovement.instance.SetGravity(2); //Lo ponemos como estaba
            }
            else
            {
                CharacterMovement.instance.SetGravity(0); //Ponemos la gravedad a 0 para que no atraviese el mapa
            }
        }
    }

    private IEnumerator WaitAndHandleDeath(float seconds)
    {
        SingletonCamera.instance.DisablePlayer();
        _collider.enabled = false; //Desactivamos el collider para que no haya multiples muertes
        
        //_rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        if (seconds>0) yield return new WaitForSeconds(seconds);
        if (CharacterMovement.instance.gravityChanged)
        {
            CharacterMovement.instance.ChangeGravity();
        }

        _animator.SetTrigger("isDeath");
        _soundManager.PlaySFX(_soundManager.death);
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

        GameManager.gameManager.AddDeath();
        _rb.constraints = _originalConstraints;
        _player.position = GetCheckpoint();
        SingletonCamera.instance.ResetPlayer();
        _rb.gravityScale = CharacterMovement.instance.gravity;
        _collider.enabled=true;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;


    }
    public void SetCheckpoint(Transform checkpointPosition)
    {
        checkpoint = checkpointPosition;
    }
    public Vector3 GetCheckpoint()
    {
        if (checkpoint != null)
        {
            return checkpoint.transform.position;
        }
        else
        {
            SceneManager.LoadScene(ChangeLevel.instance.sceneNum);
            checkpoint = GameObject.Find("FirstCheckpoint").transform;
            return checkpoint.position;
            //ChangeLevel.instance.GetCheckpoint();
            //return ChangeLevel.instance.checkpoint;
        }

    }
}
