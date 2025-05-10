using UnityEngine;

public class CharacterGameOver : MonoBehaviour
{
    public GameObject gameOverPanel;
    [SerializeField] GameObject weaponParent;
  public void GameOver()
    {
        Debug.Log("game over");
        GetComponent<PlayerMove>().enabled = false;
        gameOverPanel.SetActive(true);
        weaponParent.SetActive(false);
    }
}
