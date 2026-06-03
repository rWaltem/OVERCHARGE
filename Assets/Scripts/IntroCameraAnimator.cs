using Unity.Cinemachine;
using UnityEngine;

public class IntroCameraAnimator : MonoBehaviour
{
    public CinemachineSplineDolly dolly;
    public GameObject nextCamera;
    public EventManager eventManager;
    public float seconds = 5f;
    private float secondsPast;

    void Update()
    {
        if (secondsPast >= seconds)
        {
            if (nextCamera != null)
            {
                nextCamera.SetActive(true);
            } else
            {
                eventManager.currentGameState = EventManager.GameState.Runtime;
            }
            
            gameObject.SetActive(false);
        }

        dolly.CameraPosition = secondsPast / seconds;

        secondsPast += Time.deltaTime;
    }
}
