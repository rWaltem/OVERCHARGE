using Unity.VisualScripting;
using UnityEngine;

public class FailOnEnter : MonoBehaviour
{
    public EventManager eventManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            eventManager.currentGameState = EventManager.GameState.Failed;
        }
    }
}