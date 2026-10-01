using UnityEngine;
using System.Collections;
using TMPro;

public class GameOverScore : MonoBehaviour
{
    public TextMeshProUGUI countText;

    void Start()
    {
        countText.text = "Score: " + PenguController.count.ToString();
    }
}