using UnityEngine;

public class TutorialEndTrigger : MonoBehaviour
{
    public TutorialManager tutorialManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tutorialManager.CanFinishTutorial())
        {
            tutorialManager.LoadMainGame();
        }
    }
}