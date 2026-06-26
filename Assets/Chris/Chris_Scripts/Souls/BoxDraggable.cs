using UnityEngine;
using System.Collections;

public class BoxDraggable : MonoBehaviour
{
    [Header("Box Identity")]
    public BoxType boxType;

    [Header("References")]
    public Animator visualAnimator; // drag the Visual child's Animator here in Inspector

    public bool isBeingHeld = false;

    private Renderer boxRenderer;
    private bool isFlashing = false;

    void Awake()
    {
        // Renderer is now on the Visual child � find it there
        boxRenderer = GetComponentInChildren<Renderer>();
    }

    void OnEnable()
    {
        isBeingHeld = false;
        isFlashing = false;

        // Make sure animation resets to idle when pulled from pool
        if (visualAnimator != null)
            visualAnimator.SetBool("IsHeld", false);
    }

    public void OnPickUp()
    {
        isBeingHeld = true;

        AudioManager.Instance.PlayPickupSoul();

        if (visualAnimator != null)
        {
            visualAnimator.SetBool("IsHeld", true);
            Debug.Log("IsHeld set to true, animator state: " + visualAnimator.GetCurrentAnimatorStateInfo(0).IsName("BoxPickedUp"));
        }
    }

    public void OnRelease()
    {
        isBeingHeld = false;

        AudioManager.Instance.PlayDropSoul();

        if (visualAnimator != null)
            visualAnimator.SetBool("IsHeld", false);

        PitTarget pitHit = GetOverlappingPit();

        if (pitHit != null)
        {
            if (pitHit.acceptedBoxType == boxType)
            {
                GameManager.Instance.CorrectPit(gameObject);
            }
            else
            {
                GameManager.Instance.WrongPit();
                BoxSpawner.Instance.BoxSorted();
                BoxPool.Instance.ReturnBox(gameObject);
            }
        }
        // No pit hit — coroutine resumes from current position toward gate
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