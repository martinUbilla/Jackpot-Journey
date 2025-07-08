using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartGameplay()
    {
        SceneManager.LoadScene("GameplayScene");
        Time.timeScale = 1;

    }
}
