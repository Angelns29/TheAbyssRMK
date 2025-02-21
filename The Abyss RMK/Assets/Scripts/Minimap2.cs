using UnityEngine;

public class Minimap2 : MonoBehaviour
{
    public RenderTexture fogTexture; // RenderTexture para la niebla
    public Transform player; // Transform del jugador
    public float revealRadius = 5f; // Radio de revelación
    public Material fogMaterial; // Material para dibujar en la RenderTexture

    private Camera fogCamera; // Cámara para renderizar la niebla
    private Texture2D brushTexture; // Textura del pincel para revelar áreas

    void Start()
    {
        // Configurar la cámara para dibujar sobre la RenderTexture
        fogCamera = new GameObject("FogCamera").AddComponent<Camera>();
        fogCamera.orthographic = true;
        fogCamera.orthographicSize = 50; // Ajusta según el tamaño del mapa
        fogCamera.clearFlags = CameraClearFlags.SolidColor;
        fogCamera.backgroundColor = Color.black; // Fondo negro para la niebla
        fogCamera.cullingMask = 0; // No renderizar ningún objeto
        fogCamera.targetTexture = fogTexture; // Asignar la RenderTexture

        // Posicionar la cámara correctamente
        fogCamera.transform.position = new Vector3(27, 24, -396);
        fogCamera.transform.rotation = Quaternion.Euler(90, 0, 0); // Mirar hacia abajo

        // Crear un pincel simple para "revelar" áreas
        brushTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                float distance = Vector2.Distance(new Vector2(128, 128), new Vector2(x, y)) / 128f;
                float alpha = Mathf.Clamp01(1 - distance);
                brushTexture.SetPixel(x, y, new Color(1, 1, 1, alpha)); // Blanco con transparencia
            }
        }
        brushTexture.Apply();

        fogMaterial.SetTexture("_MainTex", brushTexture);
        // Limpiar la niebla al inicio
        ClearFog();
    }

    void LateUpdate()
    {
        if (fogCamera && fogCamera.targetTexture)
        {
            // Activar el objeto que muestra la RenderTexture
            GameObject mapUI = GameObject.Find("FogTexture"); // Ajusta el nombre según tu escena
            if (mapUI != null)
            {
                mapUI.SetActive(true); // Activar el mapa
            }

            fogCamera.Render(); // Renderizar la cámara
            RevealArea(player.position); // Revelar el área alrededor del jugador
            // Desactivar el objeto si no está visible
            if (mapUI != null && InputManager.instance.MapeInput) // Solo si no se está mostrando el mapa
            {
                mapUI.SetActive(false); // Desactivar el mapa
            }
        }
    }


    void ClearFog()
    {
        // Limpiar la RenderTexture con color negro
        RenderTexture.active = fogTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }

    void RevealArea(Vector3 worldPos)
    {
        // Convertir la posición del mundo a coordenadas de la RenderTexture
        Vector3 screenPos = fogCamera.WorldToViewportPoint(worldPos); // Usar WorldToViewportPoint
        int texX = (int)(screenPos.x * fogTexture.width); // Escalar a las dimensiones de la RenderTexture
        int texY = (int)(screenPos.y * fogTexture.height);

        // Dibujar el pincel en la RenderTexture
        RenderTexture.active = fogTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, fogTexture.width, fogTexture.height, 0);

        // Activar el material y asignar la textura
        fogMaterial.SetTexture("_MainTex", brushTexture);
        fogMaterial.SetPass(0);

        // Dibujar un cuadrado utilizando el pincel
        GL.Begin(GL.QUADS);
        GL.Color(Color.white);
        GL.TexCoord2(0, 0); GL.Vertex3(texX - 32, texY - 32, 0); // Centrado en la posición del jugador
        GL.TexCoord2(1, 0); GL.Vertex3(texX + 32, texY - 32, 0);
        GL.TexCoord2(1, 1); GL.Vertex3(texX + 32, texY + 32, 0);
        GL.TexCoord2(0, 1); GL.Vertex3(texX - 32, texY + 32, 0);
        GL.End();

        GL.PopMatrix();
        RenderTexture.active = null;

        // Guardar la RenderTexture en un archivo para depuración
        SaveRenderTextureToFile(fogTexture, Application.dataPath + "/FogTexture.png");
    }
    void SaveRenderTextureToFile(RenderTexture rt, string filePath)
    {
        // Crear una Texture2D para leer los píxeles de la RenderTexture
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        // Activar la RenderTexture para leer sus píxeles
        RenderTexture.active = rt;

        // Leer los píxeles de la RenderTexture y guardarlos en la Texture2D
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply(); // Aplicar los cambios

        // Convertir la Texture2D a un archivo PNG
        byte[] bytes = tex.EncodeToPNG();

        // Guardar el archivo en la ruta especificada
        System.IO.File.WriteAllBytes(filePath, bytes);

        // Desactivar la RenderTexture
        RenderTexture.active = null;

        // Liberar la memoria de la Texture2D
        Destroy(tex);

        Debug.Log("RenderTexture guardada en: " + filePath);
    }
}