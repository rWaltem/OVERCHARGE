using Unity.Cinemachine;
using UnityEngine;

public class IntroCameraAnimator : MonoBehaviour
{
    public CinemachineSplineDolly dolly;
    public float seconds = 5f;
    private float secondsPast;

    void Update()
    {
        if (secondsPast >= seconds) gameObject.SetActive(false);

        dolly.CameraPosition = secondsPast / seconds;

        secondsPast += Time.deltaTime;
    }
}
