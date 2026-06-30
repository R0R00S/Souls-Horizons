using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Events")]
    [SerializeField] EventReference loseLifeSfx;
    [SerializeField] EventReference pickupSoulSfx;
    [SerializeField] EventReference dropSoulSfx;
    [SerializeField] EventReference buttonClickSfx;
    [SerializeField] EventReference soulDisappearSfx;

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

    public void PlayLoseLife()       => PlayOneShot(loseLifeSfx);
    public void PlayPickupSoul()     => PlayOneShot(pickupSoulSfx);
    public void PlayDropSoul()       => PlayOneShot(dropSoulSfx);
    public void PlayButtonClick()    => PlayOneShot(buttonClickSfx);
    public void PlaySoulDisappear()  => PlayOneShot(soulDisappearSfx);

    void PlayOneShot(EventReference sfxEvent)
    {
        if (sfxEvent.IsNull) return;
        RuntimeManager.PlayOneShot(sfxEvent);
    }
}