using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // lets other scripts call GameManager.Instance.WrongPit()
    public LevelData currentLevel;

    public int lives = 3;
    public float timeRemaining;
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

        int currentSecond = Mathf.CeilToInt(timeRemaining);
        if (currentSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = currentSecond;
            UIManager.Instance.UpdateTimer(currentSecond.ToString());
        }

        if (timeRemaining <= 0) LevelWin();
    }

    public void WrongPit()
    {
        lives--;
        UIManager.Instance.UpdateLives(lives);
        Debug.Log("Wrong pit! Lives left: " + lives);

        if (lives <= 0) GameOver();
    }

    public void CorrectPit(GameObject box)
    {
        BoxPool.Instance.ReturnBox(box);
    }

    void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0;
        Debug.Log("GAME OVER");
        UIManager.Instance.ShowGameOverScreen();
    }

    void LevelWin()
    {
        isGameActive = false;
        Time.timeScale = 0;
        Debug.Log("YOU WIN");
        UIManager.Instance.ShowWinScreen();
    }
}