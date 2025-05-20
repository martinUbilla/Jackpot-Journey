using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;
    PauseManager pauseManager;



    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(panel.activeInHierarchy == false)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
        
    }

    public void CloseMenu()
    {
        pauseManager.UnPauseGame();
        panel.SetActive(false);
    }
    public void OpenMenu()
    {
        pauseManager.PauseGame();
        panel.SetActive(true);
    }
}
