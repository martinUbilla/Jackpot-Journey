using UnityEngine;

public class PlayerWin : MonoBehaviour
{
    [SerializeField] GameObject winMessagePanel;

    public void Win()
    {
        winMessagePanel.SetActive(true);
    }
}
