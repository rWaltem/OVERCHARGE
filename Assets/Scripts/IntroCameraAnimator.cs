using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine;

public class IntroCameraManager : MonoBehaviour
{
    [System.Serializable]
    public class IntroCamera
    {
        public GameObject cameraObject;
        public CinemachineSplineDolly dolly;
        public float duration = 5f;
    }

    public List<IntroCamera> cameras = new();
    public EventManager eventManager;

    private int currentCameraIndex = 0;
    private float timer = 0f;

    private InputSystem_Actions playerControls;
    private InputAction skipInput;

    void Awake()
    {
        playerControls = new InputSystem_Actions();
        Debug.Log("New player input system");
    }

    void OnEnable()
    {
        skipInput = playerControls.Player.Skip;
        skipInput.Enable();
        skipInput.performed += OnSkipIntro;
    }

    void OnDisable()
    {
        skipInput.Disable();
    }

    private void OnSkipIntro(InputAction.CallbackContext ctx)
    {
        FinishIntro();
    }

    void Start()
    {
        // Enable only the first camera
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].cameraObject.SetActive(i == 0);
        }
    }

    void FinishIntro()
    {
        foreach (var cam in cameras)
        {
            if (cam.cameraObject != null)
                cam.cameraObject.SetActive(false);
        }

        currentCameraIndex = cameras.Count;
        eventManager.currentGameState = EventManager.GameState.Runtime;
    }

    void Update()
    {
        // skip logic
        bool boost = skipInput.ReadValue<float>() > 0;
        if (boost)
        {
            FinishIntro();
            return;
        }

        if (currentCameraIndex >= cameras.Count)
            return;

        IntroCamera current = cameras[currentCameraIndex];

        timer += Time.deltaTime;

        if (current.dolly != null)
        {
            current.dolly.CameraPosition = timer / current.duration;
        }

        if (timer >= current.duration)
        {
            current.cameraObject.SetActive(false);

            currentCameraIndex++;
            timer = 0f;

            if (currentCameraIndex < cameras.Count)
            {
                cameras[currentCameraIndex].cameraObject.SetActive(true);
            }
            else
            {
                FinishIntro();
            }
        }
    }
}