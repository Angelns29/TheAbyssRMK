using System.Collections;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{

    public float secondsWait; // Tiempo de espera antes de activar la bomba
    public ProjectileMovement bomb; // Referencia al script de movimiento de la bomba

    private void Start()
    {
        // Validar que la bomba esté asignada
        if (bomb == null)
        {
            Debug.LogError("La bomba no está asignada en el Inspector.");
            return;
        }

        // Iniciar la corrutina para activar la bomba
        StartCoroutine(ActivateBomb());
    }

    private IEnumerator ActivateBomb()
    {
        // Esperar el tiempo especificado
        yield return new WaitForSeconds(secondsWait);

        // Activar el movimiento de la bomba
        bomb.Move();
    }

}
