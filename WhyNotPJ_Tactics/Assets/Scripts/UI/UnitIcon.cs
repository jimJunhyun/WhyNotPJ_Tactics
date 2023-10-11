using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Transform basePos;
    public UnitBasic unit;

	private bool dragging = false;

	private void Awake()
	{
		basePos = transform.parent;
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = eventData.position;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		dragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(transform.position);
		pos.x = Mathf.RoundToInt(pos.x * 2f) / 2f;
		pos.y = Mathf.RoundToInt(pos.y * 2f) / 2f;
		pos.z = 10;
		transform.position = basePos.position;
		dragging = false;

		if (!Physics2D.CircleCast(pos, 0.2f, Vector2.zero, 20f))
		{
			Instantiate(unit, pos, Quaternion.identity);
		}
	}
}
