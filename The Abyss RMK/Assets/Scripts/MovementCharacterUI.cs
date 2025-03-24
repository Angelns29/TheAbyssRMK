using UnityEngine;

public class MovementCharacterUI : MonoBehaviour
{
    public float speed; // Velocidad de movimiento
    public float resetY = -350f; // Posición Y inicial cuando reaparece
    public float screenTop = 380f; // Límite superior de la pantalla
    public float minX;
    public float maxX;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update() 
    {
        // Mueve la imagen hacia arriba
        rectTransform.anchoredPosition += new Vector2(0, speed * Time.deltaTime);

        // Si el personaje atraviesa la parte superior, lo reposicionamos abajo con X aleatoria
        if (rectTransform.anchoredPosition.y > screenTop)
        {
            float randomX = Random.Range(minX, maxX);
            rectTransform.anchoredPosition = new Vector2(randomX, resetY);
        }
    }
}
