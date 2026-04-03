using UnityEngine;
using UnityEngine.XR;

public class FOVController : MonoBehaviour
{
    [Header("Camera Reference")]
    public Camera playerCamera;

    [Header("FOV Settings")]
    public float normalFOV = 60f;
    public float movingFOV = 45f; // Changed from 50f to 45f for more noticeable effect
    public float fovSmoothSpeed = 5f;

    [Header("Movement Detection")]
    public float movementThreshold = 0.1f;
    private float currentInputMagnitude = 0f;
    private Vector2 moveInput = Vector2.zero;

    private float targetFOV;
    private bool isMoving = false;

    void Start()
    {
        // If no camera assigned, try to find it
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                // Try to find camera by tag
                GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
                if (cameraObj != null)
                {
                    playerCamera = cameraObj.GetComponent<Camera>();
                }
            }
        }

        if (playerCamera != null)
        {
            targetFOV = normalFOV;
            playerCamera.fieldOfView = normalFOV;
            Debug.Log("FOVController: Camera found and initialized with FOV: " + normalFOV);
        }
        else
        {
            Debug.LogError("FOVController: No camera found! Make sure you have a camera in the scene.");
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        // Detect movement input from XR controllers
        DetectMovement();

        // Smoothly transition FOV
        float newFOV = isMoving ? movingFOV : normalFOV;
        float currentFOV = playerCamera.fieldOfView;

        if (Mathf.Abs(currentFOV - newFOV) > 0.1f)
        {
            playerCamera.fieldOfView = Mathf.Lerp(currentFOV, newFOV, fovSmoothSpeed * Time.deltaTime);
        }
        else if (currentFOV != newFOV)
        {
            playerCamera.fieldOfView = newFOV;
        }

        // Debug logging (remove after testing)
        if (isMoving && currentFOV != movingFOV)
        {
            Debug.Log("FOVController: Moving detected, changing FOV to: " + movingFOV + " (current: " + currentFOV + ")");
        }
        else if (!isMoving && currentFOV != normalFOV)
        {
            Debug.Log("FOVController: Not moving, changing FOV to: " + normalFOV + " (current: " + currentFOV + ")");
        }
    }

    void DetectMovement()
    {
        // Get movement from XR input
        currentInputMagnitude = 0f;

        // Check left hand thumbstick
        var leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out moveInput))
        {
            currentInputMagnitude = Mathf.Max(currentInputMagnitude, moveInput.magnitude);
        }

        // Check right hand thumbstick
        var rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out moveInput))
        {
            currentInputMagnitude = Mathf.Max(currentInputMagnitude, moveInput.magnitude);
        }

        // Update movement state
        bool wasMoving = isMoving;
        isMoving = currentInputMagnitude > movementThreshold;

        // Debug logging for movement detection
        if (isMoving != wasMoving)
        {
            Debug.Log("FOVController: Movement state changed to: " + isMoving + " (input magnitude: " + currentInputMagnitude + ")");
        }
    }
}
