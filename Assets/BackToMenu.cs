using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    
    public void LoadGameScene()
    {
        ChatManager chatManager = FindObjectOfType<ChatManager>();
        if (chatManager != null)
        {
            chatManager.Disconnect();

        }
        else {
            Debug.LogWarning("No se encontro");

        }
        SceneManager.UnloadScene("ChatScene");
        SceneManager.LoadScene("MainMenu");
    }
}
