using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Event")]
    [SerializeField] EventReference gameplayMusicEvent;

    [Header("Parameter")]
    [SerializeField] string progressionParam = "MusicProgression";

    [Header("Starting State")]
    [SerializeField] string startingLabel = "Value A"; // type exact label name here

    EventInstance musicInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(gameplayMusicEvent);
        musicInstance.setParameterByNameWithLabel(progressionParam, startingLabel);
        musicInstance.start();
    }

    public void SetMusicState(string label)
    {
        musicInstance.setParameterByNameWithLabel(progressionParam, label);
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}