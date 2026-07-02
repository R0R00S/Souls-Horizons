using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelData currentLevel;

    public int lives = 3;
    public float timeRemaining;
    public float timeElapsed = 0f;

    public bool isGameActive = true;

    private int lastDisplayedSecond = -1;

    private float totalPenaltySeconds = 0f;
    private int livesLost = 0;

    void Awake()
    {
        Instance = this;

        if (SceneLoader.Instance != null && SceneLoader.Instance.selectedLevel != null)
        {
            currentLevel = SceneLoader.Instance.selectedLevel;
        }

        if (currentLevel == null)
        {
            Debug.LogError("No LevelData assigned anywhere. Assign one in the Inspector.");
            return;
        }

        timeRemaining = currentLevel.levelDuration;
    }

    void Start()
    {
        isGameActive = false;

        LevelDialogueData dialogueData = currentLevel.dialogueData;
        DialogueSequence opening = dialogueData != null ? dialogueData.openingSequence : null;
        DialogueManager.Instance.PlayOpeningSequence(opening, OnOpeningDialogueComplete);
    }

    void Update()
    {
        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;
        timeElapsed += Time.deltaTime;

        CheckNotifications();

        int currentSecond = Mathf.CeilToInt(timeRemaining);
        if (currentSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = currentSecond;
            UIManager.Instance.UpdateTimer(currentSecond.ToString());
        }

        if (timeRemaining <= 0f) LevelWin();
    }

    public void WrongPit()
    {
        lives--;
        livesLost++;
        UIManager.Instance.UpdateLives(lives);
        CameraShake.Instance.Shake();
        ScreenFlash.Instance.Flash();

        if (lives <= 0)
            GameOver();
    }

    public void CorrectPit(GameObject box)
    {
        BoxSpawner.Instance.BoxSorted();
        BoxPool.Instance.ReturnBox(box);
    }

    void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0;
        BoxSpawner.Instance.StopSpawning();

        TimeSoulSpawner.Instance.StopSpawning();
        foreach (PitSpawner ps in FindObjectsOfType<PitSpawner>())
            ps.StopSpawning();

        UIManager.Instance.ShowGameOverScreen();

        if (TimeSoulSpawner.Instance != null)
            TimeSoulSpawner.Instance.StopSpawning();
    }

    void LevelWin()
    {
        isGameActive = false;
        Time.timeScale = 0;
        BoxSpawner.Instance.StopSpawning();

        TimeSoulSpawner.Instance.StopSpawning();
        foreach (PitSpawner ps in FindObjectsOfType<PitSpawner>())
            ps.StopSpawning();

        // Unlock the next level on win
        int completedIndex = GetCurrentLevelIndex();
        if (completedIndex >= 0)
            ProgressionManager.Instance.UnlockNextLevel(completedIndex);

        int finalScore = CalculateFinalScore();
        UIManager.Instance.ShowWinScreen(finalScore);

        if (TimeSoulSpawner.Instance != null)
            TimeSoulSpawner.Instance.StopSpawning();
    }

    int GetCurrentLevelIndex()
    {
        if (SceneLoader.Instance == null || SceneLoader.Instance.levelOrder == null) return -1;

        for (int i = 0; i < SceneLoader.Instance.levelOrder.Length; i++)
        {
            if (SceneLoader.Instance.levelOrder[i] == currentLevel)
                return i;
        }
        return -1;
    }

    void OnOpeningDialogueComplete()
    {
        Time.timeScale = 1;
        isGameActive = true;
        UIManager.Instance.EnablePauseButton();
    }

    private int notificationCheckIndex = 0;

    void CheckNotifications()
    {
        if (currentLevel.dialogueData == null) return;

        var notifications = currentLevel.dialogueData.notifications;
        if (notificationCheckIndex >= notifications.Count) return;

        for (int i = notificationCheckIndex; i < notifications.Count; i++)
        {
            LevelNotification notification = notifications[i];

            if (timeRemaining <= notification.triggerAtTimeRemaining)
            {
                notification.hasTriggered = true;
                notificationCheckIndex++;
                NotificationManager.Instance.ShowNotification(
                    notification.message,
                    notification.displayDuration
                );
            }
            else
            {
                break;
            }
        }
    }

    public void ModifyTime(float seconds)
    {
        timeRemaining += seconds;

        if (seconds > 0)
            totalPenaltySeconds += seconds;

        if (currentLevel.maxTimeCap > 0f)
            timeRemaining = Mathf.Min(timeRemaining, currentLevel.maxTimeCap);

        lastDisplayedSecond = -1;

        string prefix = seconds > 0 ? "+" : "";
        UIManager.Instance.ShowTimeModifierNotification(prefix + Mathf.RoundToInt(seconds) + "s");
    }

    public int CalculateFinalScore()
    {
        int score = currentLevel.baseScore;

        int penaltyFromTime = Mathf.RoundToInt(totalPenaltySeconds)
                              * currentLevel.scoreLostPerPenaltySecond;
        score -= penaltyFromTime;

        int[] lifePenalties = currentLevel.scoreLostPerLife;
        for (int i = 0; i < livesLost && i < lifePenalties.Length; i++)
        {
            score -= lifePenalties[i];
        }

        score = Mathf.Max(0, score);

        Debug.Log($"Final score: {score} " +
                  $"(penalty seconds: {totalPenaltySeconds}, lives lost: {livesLost})");

        return score;
    }
}