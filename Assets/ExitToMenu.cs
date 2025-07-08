using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour
{
    public void BackToMenu()
    {
        // Enviar evento de "finish-game" (equivale a quit-match)
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.FinalizarPartida();
        }

        // Luego volver al men� principal
        SceneManager.LoadScene("MainMenu");
    }
}
