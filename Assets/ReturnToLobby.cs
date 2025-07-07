using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLobby : MonoBehaviour
{
    public void VolverAlLobby()
    {
        // Solo cambiamos de escena, sin enviar nada al servidor
        SceneManager.LoadScene("ChatScene");
    }
}

