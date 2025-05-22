using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    PauseManager pauseManager;
    [SerializeField] List<UpgradeButton> upgradesButton;
    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }
    public void OpenPanel(List<UpgradeData> upgradeDatas)
    {
        Clean();
        pauseManager.PauseGame();
        panel.SetActive(true);

        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            upgradesButton[i].gameObject.SetActive(true);               
            upgradesButton[i].Set(upgradeDatas[i]);
        }
    }
    private void Start()
    {
        for (int i = 0; i < upgradesButton.Count; i++)
        {
            upgradesButton[i].gameObject.SetActive(false);
        }

    }
    public void Upgrade(int pressedButtonID)
    {
        GameManager.instance.playerTransform.GetComponent<Level>().Upgrade(pressedButtonID);
        ClosePanel();
    }
    
    public void Clean()
    {
        for (int i = 0;i < upgradesButton.Count; i++)
        {
            upgradesButton[i].Clean();
        }
    }

    public void ClosePanel()
    {
        for (int i = 0;  i< upgradesButton.Count; i++)
        {
            upgradesButton[i].gameObject.SetActive(false);
        }

        pauseManager.UnPauseGame();
        panel.SetActive(false);
    }
}
