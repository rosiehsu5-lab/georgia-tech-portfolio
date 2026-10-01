using UnityEngine;
using System.Collections;
using TMPro;

// Attach to each checkpoint light pole.
// Requires a Light component (Spot) on the same or child GameObject.
// Assign an AudioSource and activation clip in the Inspector.
public class CheckpointLight : MonoBehaviour
{
    [Header("Light")]
    public Light spotLight;               // Drag the Spot Light here
    public Color inactiveColour = new Color(0.6f, 0.8f, 1.0f);   // Cool blue-white
    public Color activeColour = new Color(1.0f, 0.85f, 0.2f);  // Warm gold
    public float lightTransitionSpeed = 2f;

    [Header("Spot Light Settings")]
    public float spotAngle = 60f;         // Width of the light cone
    public float lightRange = 10f;        // How far the beam reaches down
    public float lightIntensity = 2f;

    [Header("Pulse")]
    public bool pulseWhenActive = true;
    public float pulseSpeed = 2f;
    public float pulseIntensity = 0.4f;   // How much intensity varies during pulse

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip activationClip;

    [Header("UI")]
    public TextMeshProUGUI checkpointText;  // Assign a World Space or Screen Space TMP text
    public string checkpointMessage = "Checkpoint Saved!";
    public float textDisplayDuration = 2f;
    public float textFadeSpeed = 2f;

    private bool isActivated = false;
    private Color targetColour;
    private float baseIntensity;

    void Start()
    {
        if (spotLight == null)
            spotLight = GetComponentInChildren<Light>();

        if (spotLight != null)
        {
            // Set up spot light pointing downward
            spotLight.type = LightType.Spot;
            spotLight.spotAngle = spotAngle;
            spotLight.range = lightRange;
            spotLight.intensity = lightIntensity;
            spotLight.color = inactiveColour;
            // Make sure light points down — rotate the light object if needed
            spotLight.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        targetColour = inactiveColour;
        baseIntensity = lightIntensity;
    }

    void Update()
    {
        if (spotLight == null) return;

        // Smoothly lerp to target colour
        spotLight.color = Color.Lerp(spotLight.color, targetColour, lightTransitionSpeed * Time.deltaTime);

        // Pulse intensity when activated
        if (isActivated && pulseWhenActive)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            spotLight.intensity = baseIntensity + pulse;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;
        if (!other.CompareTag("Player")) return;

        Activate();
    }

    void Activate()
    {
        isActivated = true;
        targetColour = activeColour;

        if (audioSource != null && activationClip != null)
        {
            audioSource.Stop();
            audioSource.clip = activationClip;
            audioSource.Play();
        }

        StartCoroutine(ActivationBurst());
        StartCoroutine(ShowCheckpointText());
    }

    // Briefly boosts intensity on activation for a satisfying flash
    IEnumerator ActivationBurst()
    {
        float burstIntensity = baseIntensity * 3f;
        float timer = 0f;
        float burstDuration = 0.3f;

        while (timer < burstDuration)
        {
            if (spotLight != null)
                spotLight.intensity = Mathf.Lerp(burstIntensity, baseIntensity, timer / burstDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        if (spotLight != null)
            spotLight.intensity = baseIntensity;
    }

    IEnumerator ShowCheckpointText()
    {
        if (checkpointText == null) yield break;

        checkpointText.text = checkpointMessage;

        // Fade in
        Color c = checkpointText.color;
        c.a = 0f;
        checkpointText.color = c;
        checkpointText.gameObject.SetActive(true);

        while (c.a < 1f)
        {
            c.a = Mathf.MoveTowards(c.a, 1f, textFadeSpeed * Time.deltaTime);
            checkpointText.color = c;
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(textDisplayDuration);

        // Fade out
        while (c.a > 0f)
        {
            c.a = Mathf.MoveTowards(c.a, 0f, textFadeSpeed * Time.deltaTime);
            checkpointText.color = c;
            yield return null;
        }

        checkpointText.gameObject.SetActive(false);
    }

    // Called externally if you need to reset the checkpoint (e.g. level restart)
    public void Reset()
    {
        isActivated = false;
        targetColour = inactiveColour;
        if (spotLight != null)
            spotLight.color = inactiveColour;
        if (checkpointText != null)
            checkpointText.gameObject.SetActive(false);
    }
}