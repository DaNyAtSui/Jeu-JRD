using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger2D : MonoBehaviour
{
    [Header("Nom de la scène à charger")]
    public string sceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("Aucun nom de scène défini dans l'inspecteur.");
            }
        }
    }
}
