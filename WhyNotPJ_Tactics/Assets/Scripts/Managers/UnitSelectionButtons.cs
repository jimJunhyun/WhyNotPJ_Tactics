using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionButtons : MonoBehaviour
{
	public UnitDatabase db;
	UnitCreator creator;
	private void Awake()
	{
		creator = GameObject.Find("MiddlePanel").GetComponent<UnitCreator>();
		for (int i = 0; i < db.units.Count; i++)
		{
			GameObject g = Instantiate(new GameObject(), transform);
			Image img = g.AddComponent<Image>();
			img.sprite = db.units[i].icon;
			img.color = Color.black;
			Button b = g.AddComponent<Button>();
			b.targetGraphic = img;
			int idx = i;
			b.onClick.AddListener(() =>{ creator.HoldDiscard(idx); });
		}
	}
}
