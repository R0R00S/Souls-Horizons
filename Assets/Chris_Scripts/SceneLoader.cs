using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    

    // Scene name constants — update these to match your actual scene names
    public const string MAIN_MENU = "Chris_StartScreen";
    public const string LEVEL_SELECT = "Chris_LevelSelect";
    

    [Header("Level Order — same order as Level Select buttons")]
    public LevelData[] levelOrder; // drag Level1, Level2, Level3 in order

    private static SceneLoader instance;
    public static SceneLoader Instance
    {
        get
        {
            if (instance == null)
            {
                // No SceneLoader in the scene yet — create one on the fly
                GameObject go = new GameObject("SceneLoader");
                instance = go.AddComponent<SceneLoader>();
                DontDestroyOnLoad(go);
                Debug.Log("SceneLoader auto-created");
            }
            return instance;
        }
        private set { instance = value; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(MAIN_MENU);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(LEVEL_SELECT);
    }

    public void LoadLevel(LevelData levelData)
    {
        selectedLevel = levelData;
        Time.timeScale = 1;

        // Load whatever scene this LevelData specifies
        SceneManager.LoadScene(levelData.sceneName);
    }

    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(selectedLevel.sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        // In Editor this does nothing — the log below confirms it fired
        Debug.Log("Quit called");
    }

    public LevelData GetNextLevel()
    {
        if (selectedLevel == null || levelOrder == null) return null;

        for (int i = 0; i < levelOrder.Length - 1; i++)
        {
            if (levelOrder[i] == selectedLevel)
                return levelOrder[i + 1];
        }

        return null; // already on last level
    }

    // Holds the chosen LevelData between scenes
    // GameManager reads this in Awake to know which level to run
    [HideInInspector] public LevelData selectedLevel;
}