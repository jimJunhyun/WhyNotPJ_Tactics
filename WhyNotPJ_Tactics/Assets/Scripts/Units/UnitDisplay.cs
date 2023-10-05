using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitDisplay : MonoBehaviour
{
    public static Color enemyColor = new Color(255, 0, 0);
    public static Color friendColor = new Color(0, 255, 0);
    public static Color neutralColor = new Color(0, 255, 255);

	int prevLayer;
	int prevHp;

	SpriteRenderer sprend;
	TextMeshProUGUI hpTxt;
	UnitBasic myBasic;

	Vector3 prevRot;

	private void Start()
	{
		sprend = GetComponent<SpriteRenderer>();
		hpTxt = GetComponentInChildren<TextMeshProUGUI>();
		myBasic = GetComponent<UnitBasic>();

		prevHp = myBasic.CurHp;
		hpTxt.text = prevHp.ToString();

		prevLayer = gameObject.layer;
		switch (prevLayer)
		{
			case 8:
				sprend.color = friendColor;
				break;
			case 9:
				sprend.color = friendColor;
				break;
			case 10:
				sprend.color = neutralColor;
				break;
		}

		prevRot = transform.eulerAngles;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).localEulerAngles = new Vector3(0, 0, -prevRot.z);
		}
	}

	private void LateUpdate()
	{
		if(prevLayer != gameObject.layer)
		{
			prevLayer = gameObject.layer;
			switch (prevLayer)
			{
				case 8:
					sprend.color = friendColor;
					break;
				case 9:
					sprend.color = friendColor;
					break;
				case 10:
					sprend.color = neutralColor;
					break;
			}
		}
		if(prevHp != myBasic.CurHp)
		{
			prevHp = myBasic.CurHp;
			hpTxt.text = prevHp.ToString();
		}
		if(prevRot != transform.eulerAngles)
		{
			prevRot = transform.eulerAngles;
			for (int i = 0; i < transform.childCount; i++)
			{
				Debug.Log(transform.GetChild(i).name);
				transform.GetChild(i).localEulerAngles = new Vector3(0, 0, -prevRot.z);
			}
		}
	}
}
