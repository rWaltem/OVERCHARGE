using Unity.Cinemachine;
//using Unity.Cinemachine.Editor;
using UnityEngine;

public class cameraSpeedEffect : MonoBehaviour
{
    public Rigidbody player;
    public CinemachineCamera cameraManager;

    [Header("")]
    public float defaultFOV = 60f;
    public float speedDivider = 85;
    public float cameraShakeMuliplier;

    void Start()
    {
        cameraManager.Lens.FieldOfView = defaultFOV;
    }

    // Update is called once per frame
    void Update()
    {
        cameraManager.Lens.FieldOfView = defaultFOV + (player.linearVelocity.magnitude / speedDivider);
    }
}
