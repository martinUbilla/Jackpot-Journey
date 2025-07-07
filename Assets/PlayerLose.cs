using UnityEngine;

public class PlayerLose : MonoBehaviour
{
    [SerializeField] GameObject loseMessagePanel;

    public void Lose()
    {
        loseMessagePanel.SetActive(true);
    }
}
