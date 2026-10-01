using UnityEngine;

public class TutorialFish : MonoBehaviour
{
    public TutorialManager tutorialManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.CompleteFish();
            gameObject.SetActive(false);
        }
    }
}