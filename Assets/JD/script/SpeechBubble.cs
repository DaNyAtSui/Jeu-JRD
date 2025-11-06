using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{

    public TextMeshProUGUI dialogueText;
    public Button continueButton;
    public RectTransform rectTransform;

    [SerializeField] private Image bubbleImage;

    [SerializeField] private Sprite normalBubbleSprite;
    [SerializeField] private Sprite thoughtBubbleSprite;
    [SerializeField] private Sprite narratorBubbleSprite;
    [SerializeField] private float baseWidth = 350f;
    [SerializeField] private float narratorWidth = 350f;


    public void SetBubbleSprite(Speaker speaker)
    {
        switch (speaker)
        {
            case Speaker.PlayerThought:
                bubbleImage.sprite = thoughtBubbleSprite;
                rectTransform.sizeDelta = new Vector2(baseWidth, rectTransform.sizeDelta.y);
                break;
            case Speaker.Narrator:
                bubbleImage.sprite = narratorBubbleSprite;
                rectTransform.sizeDelta = new Vector2(narratorWidth, rectTransform.sizeDelta.y);
                break;
            default:
                bubbleImage.sprite = normalBubbleSprite;
                rectTransform.sizeDelta = new Vector2(baseWidth, rectTransform.sizeDelta.y);
                break;
        }
    }
}
