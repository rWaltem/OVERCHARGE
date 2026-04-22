using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string singleplayerSceneName;
    [SerializeField] private string multiplayerSceneName;


    public void Singleplayer()
    {
        SceneManager.LoadScene(singleplayerSceneName);
    }

    public void Multiplayer()
    {
        SceneManager.LoadScene(multiplayerSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
