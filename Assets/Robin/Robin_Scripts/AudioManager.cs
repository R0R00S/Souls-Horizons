using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Events")]
    [SerializeField] EventReference loseLifeSfx;
    [SerializeField] EventReference pickupSoulSfx;
    [SerializeField] EventReference dropSoulSfx;

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

    public void PlayLoseLife()
    {
        Debug.Log("PlayLoseLife called");
        PlayOneShot(loseLifeSfx);
    }

    public void PlayPickupSoul()  => PlayOneShot(pickupSoulSfx);
    public void PlayDropSoul()    => PlayOneShot(dropSoulSfx);

    void PlayOneShot(EventReference sfxEvent)
    {
        if (sfxEvent.IsNull) return;
        RuntimeManager.PlayOneShot(sfxEvent);
    }
}