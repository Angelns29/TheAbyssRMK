using UnityEngine;
using UnityEngine.UI;

public class TextFollowObject : MonoBehaviour
{
    public Transform targetObject; // Objeto al que seguirá el texto
    public Vector3 offset; // Ajuste de posición del texto respecto al objeto

    private RectTransform rectTransform;
    private Camera mainCamera;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (targetObject != null && mainCamera != null)
        {
            // Convierte la posición del objeto en el mundo a una posición en la pantalla
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetObject.position + offset);

            // Actualiza la posición del texto
            rectTransform.position = screenPosition;
        }
    }
}