using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    public bool isTutorial = false;
    public GameTimer gameTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (isTutorial) return;

        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }

        SceneManager.LoadScene("GameSuccess");
    }
}