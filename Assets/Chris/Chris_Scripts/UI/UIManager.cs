using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TextMeshProUGUI timerText;
    public Image[] lifeIcons;
    public GameObject hudCanvas;

    [Header("Screens")]
    public GameObject gameOverScreen;
    public GameObject winScreen;

    [Header("Win Screen")]
    public TextMeshProUGUI winScoreText;

    private Camera cam;

    [Header("Pause")]
    public GameObject pausePanel;
    public GameObject pauseButton;

    private bool isPaused = false;

    [Header("Time Modifier Notification")]
    public TextMeshProUGUI timeModifierText;
    public float timeModifierDisplayDuration = 1.2f;

    private Coroutine timeModifierCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (winScreen != null) winScreen.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Disable pause button until opening dialogue finishes
        if (pauseButton != null) pauseButton.SetActive(false);
    }

    // Called by GameManager.OnOpeningDialogueComplete()
    public void EnablePauseButton()
    {
        if (pauseButton != null) pauseButton.SetActive(true);
    }

    public void UpdateTimer(string time)
    {
        if (timerText == null) return;
        timerText.text = time;
        timerText.color = int.Parse(time) <= 10 ? Color.red : Color.white;
    }

    public void UpdateLives(int currentLives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].enabled = (i < currentLives);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }

    public void ShowWinScreen(int score)
    {
        if (winScreen != null) winScreen.SetActive(true);

        if (winScoreText != null)
            winScoreText.text = ": " + score;
    }

    public void OnPausePressed()
    {
        AudioManager.Instance.PlayButtonClick();

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            if (hudCanvas != null) hudCanvas.SetActive(false);
            GameManager.Instance.isGameActive = false;
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            if (hudCanvas != null) hudCanvas.SetActive(true);
            GameManager.Instance.isGameActive = true;
        }
    }

    public void OnResumePressed()
    {
        OnPausePressed();
    }

    public void OnPauseLevelSelectPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToLevelSelect();
    }

    public void OnPauseMainMenuPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToMainMenu();
    }

    public void OnRetryPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.ReloadCurrentLevel();
    }

    public void OnNextLevelPressed()
    {
        AudioManager.Instance.PlayButtonClick();

        LevelData next = GetNextLevel();
        if (next != null)
            SceneLoader.Instance.LoadLevel(next);
        else
            SceneLoader.Instance.GoToLevelSelect();
    }

    public void OnLevelSelectPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToLevelSelect();
    }

    public void OnMainMenuPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.GoToMainMenu();
    }

    LevelData GetNextLevel()
    {
        LevelSelectUI levelSelect = FindObjectOfType<LevelSelectUI>();
        return SceneLoader.Instance.GetNextLevel();
    }

    public void ShowTimeModifierNotification(string text)
    {
        if (timeModifierText == null) return;

        if (timeModifierCoroutine != null)
            StopCoroutine(timeModifierCoroutine);

        timeModifierCoroutine = StartCoroutine(ShowTimeModifier(text));
    }

    IEnumerator ShowTimeModifier(string text)
    {
        timeModifierText.text = text;
        timeModifierText.gameObject.SetActive(true);
        timeModifierText.color = text.StartsWith("+") ? Color.red : Color.green;

        yield return new WaitForSecondsRealtime(timeModifierDisplayDuration);

        timeModifierText.gameObject.SetActive(false);
        timeModifierCoroutine = null;
    }
}