using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;   // the panel with Play, Rules, Quit
    public GameObject rulesPanel;  // the existing rules panel

    void Start()
    {
        mainPanel.SetActive(true);
        rulesPanel.SetActive(false);
    }

    // Hook to Play button OnClick
    public void OnPlayPressed()
    {
        SceneLoader.Instance.GoToLevelSelect();
    }

    // Hook to Rules button OnClick
    public void OnRulesPressed()
    {
        mainPanel.SetActive(false);
        rulesPanel.SetActive(true);
    }

    // Hook to Go Back button on rules panel OnClick
    public void OnRulesBackPressed()
    {
        rulesPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // Hook to Quit button OnClick
    public void OnQuitPressed()
    {
        SceneLoader.Instance.QuitGame();
    }
}