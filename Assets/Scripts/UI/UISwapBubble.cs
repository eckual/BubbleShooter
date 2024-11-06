using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Bubbles;
using Utils;

public class UISwapBubble : MonoBehaviour
{
    [SerializeField]private Button button;
    [SerializeField]private TMP_Text numberText;
    [SerializeField]private Image back;
    [SerializeField]private Image border;

    public Button Button
    {
        get { return button; }
    }

    public TMP_Text NumberText
    {
        get { return numberText; }
    }

    public Image Back
    {
        get { return back; }
    }

    public Image Border
    {
        get { return border; }
    }

    public int Power { get; set; }

    public void Init(int power)
    {
        Power = power;
        var settings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BubbleSettings);
        var number = Bubble.GetNumber(power);
        var bubbleDataIndex = settings.Bubbles.FindIndex(x => x.number == number);
        if (bubbleDataIndex == -1)
            return;

        var bubbleData = settings.Bubbles[bubbleDataIndex];

        back.color = bubbleData.backColor;
        border.color = bubbleData.borderColor;

        numberText.text = number.ToString();
    }

}
