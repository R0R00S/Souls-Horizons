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

    void Awake()
    {
        Instance = this;
        timeRemaining = currentLevel.levelDuration;
    }

    void Update()
    {
        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;
        timeElapsed += Time.deltaTime;

        // Only update UI once per second to avoid string allocation every frame
        int currentSecond = Mathf.CeilToInt(timeRemaining);
        if (currentSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = currentSecond;
            UIManager.Instance.UpdateTimer(currentSecond.ToString());
        }

        if (timeRemaining <= 0f)
            LevelWin();
    }

    public void WrongPit()
    {
        lives--;
        UIManager.Instance.UpdateLives(lives);
        Debug.Log("Wrong pit! Lives remaining: " + lives);

        if (lives <= 0)
            GameOver();
    }

    public void CorrectPit(GameObject box)
    {
        Debug.Log("Correct!");
        BoxSpawner.Instance.BoxSorted();
        BoxPool.Instance.ReturnBox(box);
    }
    void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0;
        BoxSpawner.Instance.StopSpawning();
        FlameSpawner.Instance.StopSpawning();
        UIManager.Instance.ShowGameOverScreen();
        Debug.Log("GAME OVER");
    }

    void LevelWin()
    {
        isGameActive = false;
        Time.timeScale = 0;
        BoxSpawner.Instance.StopSpawning();
        FlameSpawner.Instance.StopSpawning();
        UIManager.Instance.ShowWinScreen();
        Debug.Log("LEVEL WIN");
    }

    public void Retry()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}