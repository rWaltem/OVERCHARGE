using UnityEngine;
using UnityEngineInternal;

public class RacerSpawner : MonoBehaviour
{
    // A list of positions where ships are spawned at
    public Transform[] gridPos;
    public int playerStartNum = 0;
    public GameObject playerPrefab;
    public GameObject CPURacerPrefab;
    public bool doneSpawning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SpawnRacers()
    {
        for (int i = 0; i < gridPos.Length; i++)
        {
            //Debug.Log(gridPos[i].position);
            
            if (i == playerStartNum)
            {
                Instantiate(playerPrefab, gridPos[i].position, gridPos[i].rotation);
            }
            else
            {
                Instantiate(CPURacerPrefab, gridPos[i].position, gridPos[i].rotation);
            }
        }

        doneSpawning = true;
    }
}
