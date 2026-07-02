using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance;

    public enum UnlockState { DoNothing, Level1Only, Levels1And2, AllLevels }

    [Header("Dev Tools — Editor Only")]
    public UnlockState devUnlockState = UnlockState.DoNothing;

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

    void OnValidate()
    {
        #if UNITY_EDITOR
        if (devUnlockState != UnlockState.DoNothing)
            ApplyDevUnlockState();
        #endif
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex == 0) return true;
        return PlayerPrefs.GetInt("Level_" + levelIndex + "_Unlocked", 0) == 1;
    }

    public void UnlockNextLevel(int completedLevelIndex)
    {
        int nextLevel = completedLevelIndex + 1;
        PlayerPrefs.SetInt("Level_" + nextLevel + "_Unlocked", 1);
        PlayerPrefs.Save();
    }

    [ContextMenu("Apply Dev Unlock State")]
    public void ApplyDevUnlockState()
    {
        switch (devUnlockState)
        {
            case UnlockState.DoNothing:
                Debug.Log("Dev: DoNothing — PlayerPrefs untouched");
                break;
            case UnlockState.Level1Only:
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("Dev: Level 1 only");
                break;
            case UnlockState.Levels1And2:
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("Level_1_Unlocked", 1);
                PlayerPrefs.Save();
                Debug.Log("Dev: Levels 1 and 2 unlocked");
                break;
            case UnlockState.AllLevels:
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("Level_1_Unlocked", 1);
                PlayerPrefs.SetInt("Level_2_Unlocked", 1);
                PlayerPrefs.Save();
                Debug.Log("Dev: All levels unlocked");
                break;
        }
    }

    [ContextMenu("Reset All Progression")]
    public void ResetProgression()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Progression reset — all levels locked except Level 1");
    }
}