using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageSelect : MonoBehaviour, IPointerClickHandler
{
    static public RectTransform selected = null;
    public TextMeshProUGUI startButtonText;
    private RectTransform rectTransform;
    private TextMeshProUGUI stageText;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        stageText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (transform.name == "Image")
        {
            selected = rectTransform;
            rectTransform.sizeDelta = new Vector2(150f, 150f);
            stageText.fontSize = 110f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selected != null)
        {
            selected.sizeDelta = new Vector2(125f, 125f);
            selected.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 100f;
        }

        selected = rectTransform;
        rectTransform.sizeDelta = new Vector2(150f, 150f);
        stageText.fontSize = 110f;
        startButtonText.text = $"Start\n\n0 - {stageText.text}";
    }
}
