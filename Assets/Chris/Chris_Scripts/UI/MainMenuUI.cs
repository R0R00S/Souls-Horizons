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
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToLevelSelect();
    }

    // Hook to Rules button OnClick
    public void OnRulesPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        mainPanel.SetActive(false);
        rulesPanel.SetActive(true);
    }

    // Hook to Go Back button on rules panel OnClick
    public void OnRulesBackPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        rulesPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // Hook to Quit button OnClick
    public void OnQuitPressed()
    {
        AudioManager.Instance.PlayButtonClick();

#if UNITY_WEBGL
        SceneLoader.Instance.GoToMainMenu(); // can't quit a browser tab
#else
        SceneLoader.Instance.QuitGame();
#endif
    }
}