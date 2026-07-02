using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectUI : MonoBehaviour
{
    [Header("Level Data Assets — drag from Project window")]
    public LevelData level1Data;
    public LevelData level2Data;
    public LevelData level3Data;

    [Header("Level Buttons — drag each button here")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    [Header("Settings")]
    [SerializeField] float buttonSoundDelay = 0.15f;

    void Start()
    {
        RefreshLevelButtons();
    }

    void RefreshLevelButtons()
    {
        SetButtonState(level1Button, ProgressionManager.Instance.IsLevelUnlocked(0));
        SetButtonState(level2Button, ProgressionManager.Instance.IsLevelUnlocked(1));
        SetButtonState(level3Button, ProgressionManager.Instance.IsLevelUnlocked(2));
    }

    void SetButtonState(Button button, bool unlocked)
    {
        button.interactable = unlocked;

        // Greyed out automatically when interactable = false if your button has
        // a ColorBlock set up — but we also manually tint the alpha for clarity
        Color color = button.image.color;
        color.a = unlocked ? 1f : 0.4f;
        button.image.color = color;
    }

    public void OnLevel1Pressed()
    {
        StartCoroutine(LoadWithDelay(level1Data));
    }

    public void OnLevel2Pressed()
    {
        StartCoroutine(LoadWithDelay(level2Data));
    }

    public void OnLevel3Pressed()
    {
        StartCoroutine(LoadWithDelay(level3Data));
    }

    public void OnBackPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToMainMenu();
    }

    IEnumerator LoadWithDelay(LevelData levelData)
    {
        AudioManager.Instance.PlayLevelSelect();
        yield return new WaitForSecondsRealtime(buttonSoundDelay);
        SceneLoader.Instance.LoadLevel(levelData);
    }
}