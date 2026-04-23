using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string singleplayerSceneName;
    [SerializeField] private string multiplayerSceneName;


    public void Singleplayer()
    {
        Debug.Log("Singleplayer");
        SceneManager.LoadScene(singleplayerSceneName);
    }

    public void Multiplayer()
    {
        Debug.Log("Multiplayer");
        SceneManager.LoadScene(multiplayerSceneName);
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
