using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene à charger quand on démarre le jeu")]
    public string sceneToLoad;

    [Header("Fenêtres du menu")]
    public GameObject settingsWindow;
    public GameObject creditsWindow;

    [Header("Options Crédits")]
    [Tooltip("True = charger une scène au lieu d'ouvrir la fenêtre")]
    public bool loadCreditScene = false;
    [Tooltip("Nom de la scène de crédits si loadCreditScene = true")]
    public string creditSceneName;

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void SettingOpen() => settingsWindow.SetActive(true);
    public void SettingClose() => settingsWindow.SetActive(false);

    public void CreditsOpen()
    {
        if (loadCreditScene && !string.IsNullOrEmpty(creditSceneName))
        {
            SceneManager.LoadScene(creditSceneName);
        }
        else if (creditsWindow != null)
        {
            creditsWindow.SetActive(true);
        }
    }

    public void CreditsClose()
    {
        if (creditsWindow != null)
            creditsWindow.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
