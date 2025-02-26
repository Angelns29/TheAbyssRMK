using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MinimapZoom : MonoBehaviour
{
    public static MinimapZoom Instance;
    public Camera minimapCamera;                 // Cámara del minimapa
    public float zoomSpeed = 5f;                 // Velocidad del zoom
    public float minZoom;                   // Zoom máximo (cercano)
    public float maxZoom;                  // Zoom mínimo (alejado)
    public float defaultZoom;
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
        /*minimapCamera = GameObject.FindWithTag("CameraMap").GetComponent<Camera>();
        defaultPosition = minimapCamera.transform.position;
        defaultZoom = minimapCamera.orthographicSize;
        minZoom = defaultZoom - 20;
        maxZoom = defaultZoom + 20;
        CalculateMapBounds();*/
        UpdateReferences();

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
        minimapCamera = GameObject.FindWithTag("CameraMap")?.GetComponent<Camera>();

        if (minimapCamera != null)
        {
            defaultPosition = minimapCamera.transform.position;
            defaultZoom = minimapCamera.orthographicSize;
            minZoom = defaultZoom - 20;
            maxZoom = defaultZoom + 20;
        }
        CalculateMapBounds();
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
        //HandlePan();
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



    /*
    void HandleZoom()
    {
        float scrollValue = inputActions.MinimapControls.Zoom.ReadValue<float>();
        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            minimapCamera.orthographicSize -= scrollValue * zoomSpeed;
            minimapCamera.orthographicSize = Mathf.Clamp(minimapCamera.orthographicSize, minZoom, maxZoom);
        }
    }*/

    // Controla el Paneo NO FUNCIONA
    void HandlePan()
    {
        Vector2 panInput = inputActions.MinimapControls.Pan.ReadValue<Vector2>();

        // Detecta si el input viene del Mouse o Gamepad
        bool isMousePanning = Mouse.current != null && Mouse.current.rightButton.isPressed;
        bool isGamepadPanning = Gamepad.current != null && panInput.magnitude > 0.1f || panInput.magnitude < -0.1f;

        if (isMousePanning || isGamepadPanning)
        {
            Vector3 panMovement;

            if (isMousePanning)
            {
                // Movimiento con Mouse Delta (escala para controlar sensibilidad)
                float mouseSensitivity = 0.5f;
                panMovement = new Vector3(-panInput.x, -panInput.y, 0) * mouseSensitivity * minimapCamera.orthographicSize;
            }
            else
            {
                // Movimiento con Gamepad Stick (usa panSpeed y DeltaTime)
                panMovement = new Vector3(-panInput.x, -panInput.y, 0) * panSpeed * minimapCamera.orthographicSize * Time.deltaTime;
            }

            // Calcula nueva posición
            Vector3 targetPosition = minimapCamera.transform.position + panMovement;

            // Limita el movimiento dentro de los bounds
            float camHalfHeight = minimapCamera.orthographicSize;
            float camHalfWidth = camHalfHeight * minimapCamera.aspect;

            targetPosition.x = Mathf.Clamp(targetPosition.x, mapBounds.xMin + camHalfWidth, mapBounds.xMax - camHalfWidth);
            targetPosition.y = Mathf.Clamp(targetPosition.y, mapBounds.yMin + camHalfHeight, mapBounds.yMax - camHalfHeight);

            // Suavizado del movimiento
            minimapCamera.transform.position = Vector3.Lerp(minimapCamera.transform.position, targetPosition, 10f * Time.deltaTime);
        }
    }



    /*void HandlePan()
    {
        Vector2 panInput = inputActions.MinimapControls.Pan.ReadValue<Vector2>();
        if (isPanning )//|| panInput.magnitude > 0.1f)
        {
            Vector3 panMovement = new Vector3(-panInput.x, -panInput.y, 0) * panSpeed * minimapCamera.orthographicSize *Time.deltaTime;
            Vector3 newPosition = minimapCamera.transform.position + panMovement;

            // Aplicar límites al paneo
            newPosition.x = Mathf.Clamp(newPosition.x, mapBounds.xMin, mapBounds.xMax);
            newPosition.y = Mathf.Clamp(newPosition.y, mapBounds.yMin, mapBounds.yMax);

            minimapCamera.transform.position = newPosition;
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

    // Resetea la cámara al punto inicial
    void ResetCamera()
    {
        minimapCamera.transform.position = defaultPosition;
        minimapCamera.orthographicSize = defaultZoom;
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
            zoomSlider.value = Mathf.InverseLerp(maxZoom, minZoom, minimapCamera.orthographicSize);
        }
    }
}

