using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TextMeshProUGUI timerText;
    public Image[] lifeIcons;

    [Header("Screens")]
    public GameObject gameOverScreen;
    public GameObject winScreen;

    private Camera cam;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;

        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (winScreen != null) winScreen.SetActive(false);
    }

    public void UpdateTimer(string time)
    {
        if (timerText == null) return;
        timerText.text = time;

        // Flash red in last 10 seconds
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

    public void ShowWinScreen()
    {
        if (winScreen != null) winScreen.SetActive(true);
    }

    // Hook this to your Retry button's OnClick in the Inspector
    public void OnRetryPressed()
    {
        GameManager.Instance.Retry();
    }
}