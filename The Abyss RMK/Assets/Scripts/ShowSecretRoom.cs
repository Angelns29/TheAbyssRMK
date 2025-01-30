using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShowSecretRoom : MonoBehaviour
{
    private Tilemap _tiles;
    public float fadeDuration = 2f;


    void Start()
    {
        _tiles = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            StartCoroutine(DisappearWall());
        }
    }
    IEnumerator DisappearWall()
    {
        float elapsedTime = 0f;
        Color startColor = _tiles.color; // Color inicial (completamente opaco)
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); //Color Final TRansparente

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); // Interpola el valor alfa
            _tiles.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        _tiles.color = endColor;

        gameObject.SetActive(false); //Desactivamos para que no vuelva a hacer la animación
    }
}
