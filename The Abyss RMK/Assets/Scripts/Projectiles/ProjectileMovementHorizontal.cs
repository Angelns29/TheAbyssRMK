using System.Collections;
using UnityEngine;

public class ProjectileMovementHorizontal : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2 _velocity = new(18f, 0f); // Movimiento hacia abajo
    public Transform spawn;    // Punto de spawn para proyectiles que van hacia abajo
    public int waitTime;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        //Move();
        StartCoroutine(WaitAndMove());
    }
    private void FixedUpdate()
    {
        RotateRock();
    }
    public void RotateRock()
    {
        transform.Rotate(new Vector3 (0,0,transform.rotation.z+5),Space.World);
    }
    IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(waitTime);
        Move();
    }

    public void Move()
    {
        _rb.linearVelocity = _velocity; // Aplicar la velocidad hacia horizontal
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EndProjectile"))
        {
            transform.position = spawn.position;// Mover al punto de spawn superior
        }
    }
}
