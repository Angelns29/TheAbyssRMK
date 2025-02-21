using UnityEngine;

public class MinimapFogOfWar : MonoBehaviour
{
    public RenderTexture fogTexture; // RenderTexture para la niebla
    public Transform player; // Referencia al jugador
    public float revealRadius = 5f; // Radio de revelación

    private Texture2D fogTexture2D; // Textura 2D para manipular la niebla
    private Color32[] fogPixels; // Array de píxeles de la textura
    private int textureWidth;
    private int textureHeight;

    void Start()
    {
        // Inicializar la textura completamente negra
        textureWidth = fogTexture.width;
        textureHeight = fogTexture.height;

        // Crear una textura 2D del mismo tamaño que la RenderTexture
        fogTexture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        // Inicializar el array de píxeles
        fogPixels = new Color32[textureWidth * textureHeight];

        // Limpiar la niebla al inicio
        ClearFog();
    }

    void Update()
    {
        // Revelar la zona alrededor del jugador
        Vector2 playerPos = WorldToMinimapCoords(player.position);
        RevealArea(playerPos, revealRadius);
    }

    void ClearFog()
    {
        // Rellenar la textura con color negro
        for (int i = 0; i < fogPixels.Length; i++)
            fogPixels[i] = Color.black;

        // Aplicar los cambios a la textura 2D
        fogTexture2D.SetPixels32(fogPixels);
        fogTexture2D.Apply();

        // Copiar la textura 2D a la RenderTexture
        Graphics.Blit(fogTexture2D, fogTexture);
    }

    void RevealArea(Vector2 pos, float radius)
    {
        // Convertir las coordenadas normalizadas a píxeles
        int x = (int)(pos.x * textureWidth);
        int y = (int)(pos.y * textureHeight);
        int r = (int)(radius * textureWidth);

        // Calcular los límites del área a revelar
        int startX = Mathf.Clamp(x - r, 0, textureWidth - 1);
        int endX = Mathf.Clamp(x + r, 0, textureWidth - 1);
        int startY = Mathf.Clamp(y - r, 0, textureHeight - 1);
        int endY = Mathf.Clamp(y + r, 0, textureHeight - 1);

        // Revelar el área circular
        for (int px = startX; px <= endX; px++)
        {
            for (int py = startY; py <= endY; py++)
            {
                // Verificar si el píxel está dentro del círculo
                if ((px - x) * (px - x) + (py - y) * (py - y) <= r * r)
                {
                    fogPixels[py * textureWidth + px] = Color.clear;
                }
            }
        }

        // Aplicar los cambios a la textura 2D
        fogTexture2D.SetPixels32(fogPixels);
        fogTexture2D.Apply();

        // Copiar la textura 2D a la RenderTexture
        Graphics.Blit(fogTexture2D, fogTexture);
    }

    Vector2 WorldToMinimapCoords(Vector3 worldPos)
    {
        // Convertir coordenadas del mundo a coordenadas de la minimapa
        // Ajusta estos valores según el tamaño de tu mundo y minimapa
        float x = (worldPos.x + 50) / 100f; // Ajusta según el tamaño del mapa
        float y = (worldPos.z + 50) / 100f; // Usar Z en lugar de Y para mapas 2D
        return new Vector2(x, y);
    }
}