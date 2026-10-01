using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to a UI GameObject. Assign the fill Image and optionally a label.
// Reads CurrentNoiseLevel from PenguNoise each frame and updates the bar.
public class SlideUI : MonoBehaviour
{
    [Header("References")]
    public PenguController penguController;

    [Header("Cooldown UI")]
    public Image cooldownRadial;
    public TextMeshProUGUI cooldownLabel;
    public Image slideIcon;

    [Header("Charge UI (only matters if limitSlideCharges = true)")]
    public GameObject chargeUIRoot;
    public TextMeshProUGUI chargeText;

    [Header("Colors")]
    public Color readyColor = Color.white;
    public Color cooldownColor = new Color(0.4f,0.4f,0.4f,1f);

    void Update()
    {
        if (penguController == null) return;

        bool onCooldown = penguController.IsOnCooldown;
        float cooldownTimer = penguController.SlideCooldownTimer;
        float cooldownTime = penguController.slideCooldownTime;

        if (cooldownRadial != null)
            cooldownRadial.fillAmount = onCooldown? (cooldownTimer / cooldownTime) : 1f;
        
        if (cooldownLabel != null)
            cooldownLabel.text = onCooldown? cooldownTimer.ToString("F1") + "s" : "Ready to Slide!";

        if (slideIcon != null)
            slideIcon.color = onCooldown ? cooldownColor : readyColor;
        
        if (chargeUIRoot != null)
            chargeUIRoot.SetActive(penguController.limitSlideCharges);
        
        if (chargeText != null && penguController.limitSlideCharges)
            chargeText.text = penguController.CurrentSlideCharges + "/" + penguController.maxSlideCharges;

    }
}