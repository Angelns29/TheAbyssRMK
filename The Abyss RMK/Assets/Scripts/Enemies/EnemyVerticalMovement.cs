using System.Collections;
using UnityEngine;

public class EnemyVerticalMovement : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform upEdge;
    [SerializeField] private Transform downEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    private SpriteRenderer _sprite;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingUp = true;
    private bool attaking;
    private Animator _animator;
    //public SoundManagerScript soundManager;

    void Awake()
    {
        //soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManagerScript>();
        initScale = enemy.localScale;
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    void FixedUpdate()
    {
        if (!attaking)
        {
            Patrol();
        }

    }
    private void Patrol()
    {
        if (movingUp)
        {
            if (enemy.position.y >= downEdge.position.y) MoveInDirection(-1);
            else
            {
                //soundManager.PlaySFX(soundManager.enemy);
                ChangeDirection();
            }
        }
        else
        {
            if (enemy.position.y <= upEdge.position.y) MoveInDirection(1);
            else
            {
                //soundManager.PlaySFX(soundManager.enemy);
                ChangeDirection();
            }
        }
    }
    private void ChangeDirection()
    {
        movingUp = !movingUp;
        if (movingUp)
        {
            _sprite.flipX = true;
            _sprite.flipY = true;
        }
        else
        {
            _sprite.flipX = false;
            _sprite.flipY = false;
        }
    }
    private void MoveInDirection(int _direction)
    {
        enemy.localScale = new Vector3(initScale.x, Mathf.Abs(initScale.y) * _direction, initScale.z);
        enemy.position = new Vector3(enemy.position.x, enemy.position.y + Time.deltaTime * _direction * speed, enemy.position.z);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        // Activa el estado de ataque
        attaking = true;
        _animator.SetTrigger("isAttacking");

        // Espera el tiempo de ataque
        yield return new WaitForSeconds(1);

        // Desactiva el estado de ataque
        attaking = false;
    }
}
