using UnityEngine;

public class Minimap3 : MonoBehaviour
{
    public Transform player;
    public Texture2D fogTexture;
    public float revealRadius = 5f;
    public Material minimapFogMaterial;
    private Color32[] fogPixels;
    private bool isUpdated = false;

    private int textureWidth;
    private int textureHeight;

    // Límites del mapa (ajusta estos valores según tu mapa real)
    //public Vector2 mapMin = new Vector2(-27f, -7f);
    //public Vector2 mapMax = new Vector2(65f, 55f);

    public Vector2 mapMin = new Vector2(-25f, -25f);
    public Vector2 mapMax = new Vector2(25f, 25f);

    public float previousMapWidth;
    public float previousMapHeight; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (fogTexture == null)
        {
            textureWidth = 640;
            textureHeight = 315;
            fogTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
            fogTexture.wrapMode = TextureWrapMode.Clamp;
            fogTexture.filterMode = FilterMode.Bilinear;
        }
        else
        {
            textureWidth = fogTexture.width;
            textureHeight = fogTexture.height;
        }

        fogPixels = new Color32[textureWidth * textureHeight];

        // Inicializa la textura completamente opaca
        for (int i = 0; i < fogPixels.Length; i++)
        {
            fogPixels[i] = new Color32(0, 0, 0, 255); // Negro opaco
        }
        fogTexture.SetPixels32(fogPixels);

        fogTexture.Apply();
        AssignTextureToMinimapMaterial();
    }
    void AdjustRevealRadius()
    {
        // Obtener el tamaño relativo del nuevo mapa en comparación con el anterior
        float mapScaleFactor = Mathf.Max(MinimapZoom.Instance.mapBounds.width / previousMapWidth, MinimapZoom.Instance.mapBounds.height / previousMapHeight);

        // Ajustar el radio de revelado en función del tamaño del mapa
        revealRadius *= mapScaleFactor;

        // Guardar las nuevas dimensiones para futuros ajustes
        previousMapWidth = MinimapZoom.Instance.mapBounds.width;
        previousMapHeight = MinimapZoom.Instance.mapBounds.height;
    }

    void AssignTextureToMinimapMaterial()
    {
        if (minimapFogMaterial != null)
        {
            minimapFogMaterial.SetTexture("_FogTex", fogTexture);
        }
        else
        {
            Debug.LogError("Material MinimapFog no asignado en el Inspector.");
        }
    }

    void Update()
    {
        Vector2 playerPos = new Vector2(player.position.x, player.position.y);
        RevealFog(playerPos);

        if (isUpdated)
        {
            fogTexture.SetPixels32(fogPixels);
            fogTexture.Apply();
            isUpdated = false;
        }
    }

    //Funciona
    void RevealFog(Vector2 position)
    {
        // Convertir la posición del mundo a coordenadas normalizadas (0 a 1)
        float normalizedX = Mathf.InverseLerp(mapMin.x, mapMax.x, position.x);
        float normalizedY = Mathf.InverseLerp(mapMin.y, mapMax.y, position.y);

        // Convertir a coordenadas de la textura
        int px = Mathf.FloorToInt(normalizedX * textureWidth);
        int py = Mathf.FloorToInt(normalizedY * textureHeight);

        // Definir el tamaño del rectángulo en píxeles
        int rectWidth = Mathf.CeilToInt(revealRadius * textureWidth / (mapMax.x - mapMin.x));
        int rectHeight = Mathf.CeilToInt(revealRadius * textureHeight / (mapMax.y - mapMin.y));

        // Iterar sobre el área rectangular centrada en el jugador
        for (int y = -rectHeight ; y <= rectHeight ; y++)
        {
            for (int x = -rectWidth ; x <= rectWidth ; x++)
            {
                int fx = px + x;
                int fy = py + y;

                // Verificar que está dentro de los límites de la textura
                if (fx >= 0 && fx < textureWidth && fy >= 0 && fy < textureHeight)
                {
                    int index = fy * textureWidth + fx;
                    fogPixels[index].a = 0; // Hacer transparente
                    isUpdated = true;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 bottomLeft = new Vector3(mapMin.x, mapMin.y, 0);
        Vector3 topRight = new Vector3(mapMax.x, mapMax.y, 0);
        Gizmos.DrawLine(bottomLeft, new Vector3(topRight.x, mapMin.y, 0));
        Gizmos.DrawLine(bottomLeft, new Vector3(mapMin.x, topRight.y, 0));
        Gizmos.DrawLine(topRight, new Vector3(mapMin.x, topRight.y, 0));
        Gizmos.DrawLine(topRight, new Vector3(topRight.x, mapMin.y, 0));
    }
}
