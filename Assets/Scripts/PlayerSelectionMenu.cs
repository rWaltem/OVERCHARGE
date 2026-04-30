using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum GameMode
{
    Singleplayer,
    SplitScreen,
    Multiplayer,
    TimeTrials
}

public class PlayerSelectionMenu : MonoBehaviour
{
    public GameMode selectedMode;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string devSceneName;


    public void MainMenu()
    {
        Debug.Log("Back to main menu");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void PlayGame()
    {
        Debug.Log("Selected Mode: " + selectedMode);

        // You can branch logic based on mode
        switch (selectedMode)
        {
            case GameMode.Singleplayer:
                Debug.Log("Starting Singleplayer");
                break;
            
            case GameMode.SplitScreen:
                Debug.Log("Starting Splitscreen");
                break;

            case GameMode.Multiplayer:
                Debug.Log("Starting Multiplayer");
                break;

            case GameMode.TimeTrials:
                Debug.Log("Starting Time Trials");
                break;
        }

        SceneManager.LoadScene(devSceneName);
    }
}