using UnityEngine;

public class Pit : MonoBehaviour
{
    public SinType acceptedSin;

    public void ReceiveSoul(Soul soul)
    {
        if (soul.sinType == SinType.None)
        {
            // Innocent soul dropped in any pit — wrong!
            Debug.Log("An innocent soul was dropped in a pit! Lose a life.");
            Destroy(soul.gameObject);
        }
        else if (soul.sinType == acceptedSin)
        {
            // Correct sort
            Debug.Log($"Correct! {soul.sinType} soul sorted into the right pit.");
            Destroy(soul.gameObject);
        }
        else
        {
            // Sinful soul dropped in the wrong pit
            Debug.Log($"Wrong pit! {soul.sinType} soul does not belong in the {acceptedSin} pit.");
            Destroy(soul.gameObject);
        }
    }
}