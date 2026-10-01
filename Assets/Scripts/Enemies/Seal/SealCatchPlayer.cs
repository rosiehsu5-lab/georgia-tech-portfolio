using UnityEngine;

public class SealCatchPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Seal caught the player.");
            other.SendMessage("OnCaughtByEnemy", SendMessageOptions.DontRequireReceiver);
        }
    }
}