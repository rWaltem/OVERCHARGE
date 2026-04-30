using UnityEngine;

public class RacerSpawner : MonoBehaviour
{
    // A list of positions where ships are spawned at
    public Transform[] startingPositions;
    public GameObject playerPrefab;
    public GameObject CPURacerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform gridPos in startingPositions)
        {
            Debug.Log(gridPos.position);
            Instantiate(CPURacerPrefab, gridPos.position, gridPos.rotation);
        }
    }
}
