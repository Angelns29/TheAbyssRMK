using System.Collections;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2 _velocity = new(0f, -12f); // Movimiento hacia abajo
    public Transform spawn;    // Punto de spawn para proyectiles que van hacia abajo
    public int waitTime;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        StartCoroutine(WaitAndMove());
    }
    IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(waitTime);
        Move();
    }
    public void Move()
    {
        _rb.linearVelocity = _velocity; // Aplicar la velocidad hacia abajo
        //_soundManager?.PlaySFX(_soundManager.bombSound); // Reproducir sonido de la bomba
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EndProjectile"))
        {
            transform.position = spawn.position;// Mover al punto de spawn superior
            _rb.linearVelocity = new Vector2(0, 0);
            
        }
    }
}
