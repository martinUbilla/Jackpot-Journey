using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour
{
   public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
