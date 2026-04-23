using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectionMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string devSceneName;

    public void MainMenu()
    {
        Debug.Log("Back to main menu");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void PlayGame()
    {
        Debug.Log("To Track Selection");
        Debug.Log("For now jump to dev map");
        SceneManager.LoadScene(devSceneName);
    }
}
