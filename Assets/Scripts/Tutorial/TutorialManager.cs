using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("Existing text (required)")]
    public TextMeshProUGUI tutorialText;

    [Header("Extra UI (optional — auto-wired by Tools > Setup Tutorial Scene)")]
    public TextMeshProUGUI stepCounter;
    public TextMeshProUGUI keyHint;
    public RectTransform instructionPanel;

    [Header("Feedback")]
    public AudioSource audioSource;
    public AudioClip stepCompleteClip;
    public AudioClip tutorialCompleteClip;
    public float punchScale = 1.12f;
    public float punchDuration = 0.25f;

    private const int TotalSteps = 5;
    private bool moveDone = false;
    private bool jumpDone = false;
    private bool doubleJumpDone = false;
    private bool slideDone = false;
    private bool fishDone = false;

    private CharacterController playerController;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerController = player.GetComponent<CharacterController>();
        UpdateText("Move", "[W]  [A]  [S]  [D]", stepIndex: 1);
    }

    void Update()
    {
        if (!moveDone)
        {
            if (Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.D))
            {
                CompleteMove();
            }
        }
        else if (!jumpDone)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CompleteJump();
            }
        }
        else if (!doubleJumpDone)
        {
            bool airborne = playerController != null && !playerController.isGrounded;
            if (Input.GetKeyDown(KeyCode.Space) && airborne)
            {
                CompleteDoubleJump();
            }
        }
        else if (!slideDone)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                CompleteSlide();
            }
        }
    }

    public void CompleteMove()
    {
        if (moveDone) return;
        moveDone = true;
        PlayStepSound();
        UpdateText("Jump", "[SPACE]", stepIndex: 2);
    }

    public void CompleteJump()
    {
        if (jumpDone) return;
        jumpDone = true;
        PlayStepSound();
        UpdateText("Double jump", "Press [SPACE] again in mid-air", stepIndex: 3);
    }

    public void CompleteDoubleJump()
    {
        if (doubleJumpDone) return;
        doubleJumpDone = true;
        PlayStepSound();
        UpdateText("Slide", "Hold [LEFT SHIFT]", stepIndex: 4);
    }

    public void CompleteSlide()
    {
        if (slideDone) return;
        slideDone = true;
        PlayStepSound();
        UpdateText("Collect a fish", "Walk into any fish", stepIndex: 5);
    }

    public void CompleteFish()
    {
        if (!slideDone || fishDone) return;
        fishDone = true;
        PlayStepSound();
        UpdateText("Reach the log house", "Follow the path", stepIndex: 5, allDone: true);
    }

    public bool CanFinishTutorial()
    {
        return moveDone && jumpDone && doubleJumpDone && slideDone && fishDone;
    }

    public void LoadMainGame()
    {
        if (audioSource != null && tutorialCompleteClip != null)
            audioSource.PlayOneShot(tutorialCompleteClip);
        SceneManager.LoadScene("Easy");
    }

    void UpdateText(string main, string hint, int stepIndex, bool allDone = false)
    {
        if (tutorialText != null) tutorialText.text = main;
        if (keyHint != null) keyHint.text = hint;
        if (stepCounter != null)
            stepCounter.text = allDone ? "All done!" : $"Step {stepIndex} / {TotalSteps}";
        if (instructionPanel != null)
            StartCoroutine(PunchPanel());
    }

    void PlayStepSound()
    {
        if (audioSource != null && stepCompleteClip != null)
            audioSource.PlayOneShot(stepCompleteClip);
    }

    IEnumerator PunchPanel()
    {
        if (instructionPanel == null) yield break;
        float t = 0f;
        Vector3 baseScale = Vector3.one;
        Vector3 peakScale = Vector3.one * punchScale;
        while (t < punchDuration)
        {
            t += Time.deltaTime;
            float p = t / punchDuration;
            float ease = Mathf.Sin(p * Mathf.PI);
            instructionPanel.localScale = Vector3.Lerp(baseScale, peakScale, ease);
            yield return null;
        }
        instructionPanel.localScale = baseScale;
    }
}
