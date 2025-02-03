using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2 _velocity = new(0f, -12f); // Movimiento hacia abajo
    public Transform spawnRight;    // Punto de spawn para proyectiles que van hacia abajo
    public Transform spawnLeft;  // Punto de spawn para proyectiles que van hacia arriba
    //private SoundManagerScript _soundManager;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Obtener el SoundManager de manera m�s eficiente
        //_soundManager = SoundManagerScript.soundManagerScript;

        /*if (_soundManager == null)
        {
            Debug.LogError("SoundManager no encontrado.");
        }*/
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
            // Reposicionar el proyectil seg�n su nombre
            if (gameObject.name == "ProjectileRight")
            {
                transform.position = spawnRight.position;// Mover al punto de spawn superior
                _rb.linearVelocity = new Vector2 (0,0);
            }
            else if (gameObject.name == "ProjectileLeft")
            {
                transform.position = spawnLeft.position; // Mover al punto de spawn inferior
                _rb.linearVelocity = new Vector2(0, 0);

            }
        }
    }
}
