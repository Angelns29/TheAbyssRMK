using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MinimapZoom : MonoBehaviour
{
    public static MinimapZoom Instance;
    public Camera minimapCamera;                 // Cámara del minimapa
    public float zoomSpeed = 0.5f;                 // Velocidad del zoom
    public float minZoom = 1;                   // Zoom máximo (cercano)
    public float maxZoom = 5;                  // Zoom mínimo (alejado)
    public float defaultZoom;
    public float zoomLevel;
    public Material minimapFogMaterial;
    private Vector2 minimapOffset = Vector2.zero; // Offset inicial en el minimapa
    private float mapWidth;
    private float mapHeight;

    public float panSpeed = 200f;                  // Velocidad al mover la cámara
    public Rect mapBounds;                       // Límites del mapa para el paneo
    public Slider zoomSlider;                    // Slider UI para mostrar el nivel de zoom
    public Transform player;

    private InputSystem_Actions inputActions;    // Referencia al Input System
    private bool isPanning = false;              // Estado para saber si se está paneando
    private Vector3 defaultPosition;             // Posición inicial para reset

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }     
        inputActions = new InputSystem_Actions();    
        player = GameObject.FindWithTag("Player").transform;
        UpdateReferences();
        ResetCamera();
        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reasignar referencias en cada nueva escena
        UpdateReferences();
    }

    private void UpdateReferences()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        //StartCoroutine(GetMaterial());
        minimapFogMaterial = GameObject.Find("MinimapImage").GetComponent<RawImage>().material;

        // Ajustar tamaño de minimapa
        mapWidth = minimapFogMaterial.mainTexture.width;
        mapHeight = minimapFogMaterial.mainTexture.height;


        minimapCamera = GameObject.FindWithTag("CameraMap")?.GetComponent<Camera>();

        if (minimapCamera != null)
        {
            defaultPosition = minimapCamera.transform.position;
            //defaultZoom = minimapCamera.orthographicSize;
            //minZoom = defaultZoom - 20;
            //maxZoom = defaultZoom + 20;
            
        }
        CalculateMapBounds();
    }
    public void SetMaterial(Material material)
    {
        minimapFogMaterial = material;
    }
    private void OnEnable()
    {
        inputActions.MinimapControls.Enable();

        // Suscribirse a eventos
        inputActions.MinimapControls.PanClick.performed += ctx => StartPanning();
        inputActions.MinimapControls.PanClick.canceled += ctx => StopPanning();
        inputActions.MinimapControls.ResetView.performed += ctx => ResetCamera();
        inputActions.MinimapControls.CenterOnPlayer.performed += ctx => CenterOnPlayer();
    }

    private void OnDisable()
    {
        inputActions.MinimapControls.Disable();

        // Desuscribirse de eventos
        inputActions.MinimapControls.PanClick.performed -= ctx => StartPanning();
        inputActions.MinimapControls.PanClick.canceled -= ctx => StopPanning();
        inputActions.MinimapControls.ResetView.performed -= ctx => ResetCamera();
        inputActions.MinimapControls.CenterOnPlayer.performed -= ctx => CenterOnPlayer();

    }
    
    void Update()
    {
        HandleZoom();
        HandlePan();
        UpdateZoomSlider();
    }
    private void CalculateMapBounds()
    {
        // Busca un collider que defina el nivel  
        TilemapCollider2D mapCollider = GameObject.FindWithTag("Map").GetComponent<TilemapCollider2D>();
        if (mapCollider != null)
        {
            
            Bounds bounds = mapCollider.composite.bounds;
            mapBounds = new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
        }
        else
        {
            Debug.LogWarning("No se encontró un collider con el tag 'Map'.");
        }
    }

    void HandleZoom()
    {
        float scrollValue = inputActions.MinimapControls.Zoom.ReadValue<float>();

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            zoomLevel += scrollValue * zoomSpeed;
            zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);

            UpdateMinimapZoomAndOffset(zoomLevel);  // Ajustar Zoom y Offset
        }
    }


    Vector2 GetPlayerUV()
    {
        float u = Mathf.InverseLerp(mapBounds.xMin, mapBounds.xMax, player.position.x);
        float v = Mathf.InverseLerp(mapBounds.yMin, mapBounds.yMax, player.position.y);
        return new Vector2(u, v);
    }
    void UpdateMinimapZoomAndOffset(float zoomAmount)
    {
        // Actualizar el Zoom
        minimapFogMaterial.SetFloat("_Zoom", zoomAmount);

        // Obtener la posición normalizada del personaje en el minimapa
        Vector2 playerMinimapPos = GetPlayerUV();

        // Convertir la posición normalizada a un offset en UV (-0.5 a 0.5)
        Vector2 offset = new Vector2(playerMinimapPos.x - 0.5f, playerMinimapPos.y - 0.5f);

        // Aplicar el offset al material
        minimapFogMaterial.SetVector("_Offset", offset / zoomAmount);  //Offset depende del zoom
    }


    /*
    // Controla el Zoom
    void HandleZoom()
    {
        float scrollValue = inputActions.MinimapControls.Zoom.ReadValue<float>();

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            // Guarda el tamaño ortográfico anterior
            float previousSize = minimapCamera.orthographicSize;

            // Ajusta el tamaño ortográfico según el zoom
            minimapCamera.orthographicSize -= scrollValue * zoomSpeed;
            minimapCamera.orthographicSize = Mathf.Clamp(minimapCamera.orthographicSize, minZoom, maxZoom);

            // Calcula la diferencia de zoom
            float zoomFactor = minimapCamera.orthographicSize / previousSize;

            // Calcula el desplazamiento para mantener el jugador centrado
            Vector3 cameraToPlayer = player.position - minimapCamera.transform.position;
            minimapCamera.transform.position += cameraToPlayer * (1 - zoomFactor);

            // Mantiene la cámara dentro de los límites del mapa
            float camHalfHeight = minimapCamera.orthographicSize;
            float camHalfWidth = camHalfHeight * minimapCamera.aspect;

            Vector3 clampedPosition = minimapCamera.transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, mapBounds.xMin + camHalfWidth, mapBounds.xMax - camHalfWidth);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, mapBounds.yMin + camHalfHeight, mapBounds.yMax - camHalfHeight);

            minimapCamera.transform.position = clampedPosition;
        }
    }
    */

    // Controla el Paneo NO FUNCIONA GAMEPAD

    void HandlePan()
    {
        Vector2 panInput = inputActions.MinimapControls.Pan.ReadValue<Vector2>();

        if (isPanning)
        {
            float mouseSensitivity = 0.005f;
            Vector2 panMovement = new Vector2(-panInput.x, -panInput.y) * mouseSensitivity * zoomLevel;

            // **Calcula los límites del paneo usando el tamaño de la textura del minimapa**
            float maxOffsetX = (mapWidth - 1f) / (2f * zoomLevel);
            float maxOffsetY = (mapHeight - 1f) / (2f * zoomLevel);

            // **Aplica el offset y lo limita**
            minimapOffset += panMovement;
            minimapOffset.x = Mathf.Clamp(minimapOffset.x, -maxOffsetX, maxOffsetX);
            minimapOffset.y = Mathf.Clamp(minimapOffset.y, -maxOffsetY, maxOffsetY);

            // **Envía el offset corregido al Shader**
            minimapFogMaterial.SetVector("_Offset", minimapOffset);
        }
    }




    /*void HandlePan()
    {
        Vector2 panInput = inputActions.MinimapControls.Pan.ReadValue<Vector2>();


        if (isPanning)
        {
            Vector2 panMovement;
            float mouseSensitivity = 0.005f;  // Ajusta según necesidad
            panMovement = new Vector2(-panInput.x, -panInput.y) * mouseSensitivity * zoomLevel;


            // Actualizamos el Offset en el shader
            minimapOffset += panMovement;
            minimapFogMaterial.SetVector("_Offset", minimapOffset / zoomLevel); //Offset relativo al zoom
        }
    }*/


    // Inicia el Paneo
    void StartPanning()
    {
        isPanning = true;
    }

    // Detiene el Paneo
    void StopPanning()
    {
        isPanning = false;
    }

    // Detecta si el input viene del Mouse o Gamepad
    //bool isMousePanning = Mouse.current != null && Mouse.current.rightButton.isPressed;
    //bool isGamepadPanning = Gamepad.current != null && Mathf.Abs(panInput.magnitude) > 0.1f;
    /* if (isMousePanning)
 {
     // Movimiento con Mouse Delta (ajustamos la sensibilidad)
     float mouseSensitivity = 0.005f;  // Ajusta según necesidad
     panMovement = new Vector2(-panInput.x, -panInput.y) * mouseSensitivity * zoomLevel;
 }
 else if (isGamepadPanning)
 {
     // Movimiento con Gamepad Stick (usa panSpeed y DeltaTime)
     float gamepadSensitivity = 0.005f;
     panMovement = new Vector2(-panInput.x, -panInput.y) * gamepadSensitivity * zoomLevel;
 }
 else
 {
     panMovement = new Vector2(0,0);
 }*/

    // Resetea la cámara al punto inicial
    void ResetCamera()
    {
        /*minimapCamera.transform.position = defaultPosition;
        minimapCamera.orthographicSize = defaultZoom;*/
        minimapFogMaterial.SetFloat("_Zoom", 1);
        minimapFogMaterial.SetVector("_Offset", new Vector2(0,0));
        UpdateZoomSlider();
    }

    // Centra la cámara en el jugador
    void CenterOnPlayer()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
            minimapCamera.transform.position = new Vector3(playerPos.x, playerPos.y, minimapCamera.transform.position.z);
        }
    }

    // Actualiza el slider de zoom
    void UpdateZoomSlider()
    {
        if (zoomSlider != null)
        {
            //zoomSlider.value = Mathf.InverseLerp(maxZoom, minZoom, minimapCamera.orthographicSize);
            zoomSlider.value = Mathf.InverseLerp(minZoom, maxZoom, minimapFogMaterial.GetFloat("_Zoom"));
        }
    }
}

