using UnityEngine;
using System.Collections;

public class BoxDraggable : MonoBehaviour
{
    [Header("Box Identity")]
    public BoxType boxType;
    public bool isTimeSoul = false; // tick this on time soul prefabs in Inspector

    [Header("References")]
    public Animator visualAnimator; // drag the Visual child's Animator here in Inspector

    public bool isBeingHeld = false;

    private Renderer boxRenderer;
    private bool isFlashing = false;

    void Awake()
    {
        // Renderer is now on the Visual child — find it there
        boxRenderer = GetComponentInChildren<Renderer>();
    }

    void OnEnable()
    {
        isBeingHeld = false;
        isFlashing = false;

        if (visualAnimator != null)
        {
            visualAnimator.SetBool("IsHeld", false);
            visualAnimator.Play("BoxIdle", 0, 0f);
            visualAnimator.Update(0f); // force-sample the pose immediately
        }
    }
    public void OnPickUp()
    {
        isBeingHeld = true;

        AudioManager.Instance.PlayPickupSoul();

        if (visualAnimator != null)
        {
            visualAnimator.SetBool("IsHeld", true);
            
        }
    }

    public void OnRelease()
    {
        isBeingHeld = false;

        if (visualAnimator != null)
            visualAnimator.SetBool("IsHeld", false);

        PitTarget pitHit = GetOverlappingPit();

        if (pitHit != null)
        {
            if (pitHit.acceptedBoxType == boxType)
            {
                // Correct pit
                if (isTimeSoul)
                {
                    Debug.Log("Time soul sorted correctly — reward!");
                    GameManager.Instance.ModifyTime(
                        -GameManager.Instance.currentLevel.timeSoulRewardSeconds
                    );
                    if (TimeSoulSpawner.Instance != null)
                        TimeSoulSpawner.Instance.TimeSoulResolved(gameObject);
                    BoxSpawner.Instance.BoxSorted();
                    return;
                }
                GameManager.Instance.CorrectPit(gameObject);
            }
            else
            {
                // Wrong pit
                GameManager.Instance.WrongPit();
                BoxSpawner.Instance.BoxSorted();

                if (isTimeSoul)
                {
                    // Wrong pit on time soul still triggers time punishment
                    GameManager.Instance.ModifyTime(
                        GameManager.Instance.currentLevel.timeSoulPenaltySeconds
                    );
                    if (TimeSoulSpawner.Instance != null)
                        TimeSoulSpawner.Instance.TimeSoulResolved(gameObject);
                    return;
                }

                BoxPool.Instance.ReturnBox(gameObject);
            }
        }
    }

    PitTarget GetOverlappingPit()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, 0.6f);

        foreach (Collider col in nearby)
        {
            PitTarget pit = col.GetComponent<PitTarget>();
            if (pit != null) return pit;
        }

        return null;
    }

    public void FlashWarning()
    {
        if (!isFlashing) StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        isFlashing = true;
        Color originalColor = boxRenderer.material.color;

        for (int i = 0; i < 3; i++)
        {
            boxRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            boxRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        isFlashing = false;
    }

    void OnDrawGizmos()
    {
        if (isBeingHeld)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.6f);
        }
    }
}