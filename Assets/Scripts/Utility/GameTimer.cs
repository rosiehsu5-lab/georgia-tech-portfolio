using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject player;

    public float timeRemaining = 60f;
    private bool timerRunning = true;

    public SceneSwitcher gameOverSceneSwitcher;
    public SceneSwitcher successSceneSwitcher;

    void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;
        timerText.text = "Time Remaining: " + timeRemaining.ToString("F1");

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;
            timerText.gameObject.SetActive(false);
            GameOver();
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    // Called by PenguGoalTrigger when the penguin reaches the goal
    public void TriggerSuccess()
    {
        if (!timerRunning) return; // timer already ran out, ignore

        timerRunning = false;
        timerText.gameObject.SetActive(false);
        successSceneSwitcher.SwitchScene();
    }

    void GameOver()
    {
        if (player != null)
            Destroy(player);

        gameOverSceneSwitcher.SwitchScene();
    }
}