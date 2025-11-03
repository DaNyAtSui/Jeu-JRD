using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject settingsWindow;
    public GameObject creditsWindow;

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void SettingOpen()
    {
        settingsWindow.SetActive(true);
    }

    public void SettingClose()
    {
        settingsWindow.SetActive(false);
    }

    public void CreditsOpen()
    {
        creditsWindow.SetActive(true);
    }

    public void CreditsClose()
    {
        creditsWindow.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
