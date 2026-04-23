using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectionMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName;
    public void MainMenu()
    {
        Debug.Log("Back to main menu");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void PlayGame()
    {
        Debug.Log("To Track Selection");
    }
}
