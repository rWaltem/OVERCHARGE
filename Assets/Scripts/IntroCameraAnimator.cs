using System.Collections.Generic;
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

    void Start()
    {
        // Enable only the first camera
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].cameraObject.SetActive(i == 0);
        }
    }

    void Update()
    {
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
                eventManager.currentGameState = EventManager.GameState.Runtime;
            }
        }
    }
}