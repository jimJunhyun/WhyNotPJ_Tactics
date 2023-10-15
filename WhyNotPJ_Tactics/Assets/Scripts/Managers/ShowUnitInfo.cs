using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowUnitInfo : MonoBehaviour
{
	public static ShowUnitInfo instance;

	public UnitDatabase db;

    RectTransform unitInfoPanel;

	Image descImg;
	TextMeshProUGUI[] infoTxt;


	private void Awake()
	{

		instance = this;
		unitInfoPanel = GameObject.Find("SecondaryPanel").GetComponent<RectTransform>();

		descImg = unitInfoPanel.Find("DescImage").GetComponent<Image>();
		infoTxt = new TextMeshProUGUI[unitInfoPanel.Find("InfoPanel").childCount];
		for (int i = 0; i < infoTxt.Length; i++)
		{
			infoTxt[i] = unitInfoPanel.Find("InfoPanel").GetChild(i).GetComponent<TextMeshProUGUI>();
		}
		Off();
	}

	public void On(int idx)
	{
		Debug.Log("on");
		unitInfoPanel.gameObject.SetActive(true);
		descImg.sprite =  db.units[idx].icon;
		infoTxt[0].text = db.units[idx].name;
		infoTxt[1].text = $"{db.units[idx].unitInfo.hp} | {db.units[idx].unitInfo.moveGap} | {db.units[idx].unitInfo.cost}";
		infoTxt[2].text = db.units[idx].description;
	}

	public void Off()
	{
		Debug.Log("off");
		unitInfoPanel.gameObject.SetActive(false);
	}
}
