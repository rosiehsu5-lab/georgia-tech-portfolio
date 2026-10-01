using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StorylineController : MonoBehaviour
{
    [System.Serializable]
    public class Slide
    {
        [TextArea(3, 8)] public string text;
        public Color backgroundTint = Color.black;
        public AudioClip oneShotOnShow;
    }

    [Header("Slides")]
    public Slide[] slides;

    [Header("Text")]
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI continuePrompt;
    public string continuePromptText = "Press Space / Click to continue";

    [Header("Typewriter")]
    [Tooltip("Seconds per character. Smaller = faster.")]
    public float typeDelay = 0.035f;
    [Tooltip("Extra pause after punctuation for natural rhythm.")]
    public float punctuationPause = 0.18f;

    [Header("Background")]
    public UnityEngine.UI.Image backgroundTint;
    public float tintLerpSpeed = 2f;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip typeTickClip;
    [Range(0f, 1f)] public float typeTickVolume = 0.25f;
    [Tooltip("Play a tick every N characters to avoid a buzz.")]
    public int typeTickEveryNChars = 2;

    [Header("Flow")]
    public string nextSceneName = "Tutorial";
    [Tooltip("Pause after the typewriter finishes before auto-advancing to the next slide. Ignored on the last slide, which waits for player input.")]
    public float autoAdvanceDelay = 2.5f;

    private int currentSlide = -1;
    private Coroutine typeRoutine;
    private bool slideFullyShown = false;
    private Color targetTint;

    void Start()
    {
        if (bodyText != null) bodyText.text = "";
        if (continuePrompt != null)
        {
            continuePrompt.text = continuePromptText;
            continuePrompt.gameObject.SetActive(false);
        }
        if (musicSource != null) musicSource.Play();
        AdvanceSlide();
    }

    void Update()
    {
        if (backgroundTint != null)
        {
            backgroundTint.color = Color.Lerp(backgroundTint.color, targetTint, tintLerpSpeed * Time.deltaTime);
        }

        bool advanceKey = Input.GetKeyDown(KeyCode.Space)
                      || Input.GetKeyDown(KeyCode.Return)
                      || Input.GetMouseButtonDown(0);

        if (!advanceKey) return;

        if (!slideFullyShown)
        {
            FinishTypingImmediately();
        }
        else
        {
            AdvanceSlide();
        }
    }

    public void AdvanceSlide()
    {
        currentSlide++;
        if (currentSlide >= slides.Length)
        {
            FinishStoryline();
            return;
        }

        Slide s = slides[currentSlide];
        targetTint = s.backgroundTint;
        if (s.oneShotOnShow != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(s.oneShotOnShow);
        }

        if (continuePrompt != null) continuePrompt.gameObject.SetActive(false);
        slideFullyShown = false;

        if (typeRoutine != null) StopCoroutine(typeRoutine);
        typeRoutine = StartCoroutine(TypeSlide(s.text));
    }

    IEnumerator TypeSlide(string text)
    {
        bodyText.text = "";
        int i = 0;
        foreach (char c in text)
        {
            bodyText.text += c;

            if (typeTickClip != null && sfxSource != null
                && i % Mathf.Max(1, typeTickEveryNChars) == 0
                && c != ' ' && c != '\n')
            {
                sfxSource.PlayOneShot(typeTickClip, typeTickVolume);
            }

            float delay = typeDelay;
            if (c == '.' || c == ',' || c == '!' || c == '?' || c == ';' || c == ':')
                delay += punctuationPause;

            i++;
            yield return new WaitForSeconds(delay);
        }

        OnSlideFullyShown();
    }

    void FinishTypingImmediately()
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        if (currentSlide >= 0 && currentSlide < slides.Length)
            bodyText.text = slides[currentSlide].text;
        OnSlideFullyShown();
    }

    void OnSlideFullyShown()
    {
        slideFullyShown = true;
        bool isLastSlide = currentSlide == slides.Length - 1;
        if (continuePrompt != null) continuePrompt.gameObject.SetActive(isLastSlide);
        if (!isLastSlide) StartCoroutine(AutoAdvanceAfterDelay());
    }

    IEnumerator AutoAdvanceAfterDelay()
    {
        yield return new WaitForSeconds(autoAdvanceDelay);
        if (slideFullyShown) AdvanceSlide();
    }

    public void SkipStoryline()
    {
        FinishStoryline();
    }

    void FinishStoryline()
    {
        if (musicSource != null) musicSource.Stop();
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
