using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenMover : MonoBehaviour, IDragHandler
{
    public float speed;
	public const int STAGEX = 10;
	public const int STAGEY = 10;

	Camera mainCam;


	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.dragging)
		{
			mainCam.transform.Translate(eventData.delta * -0.1f * speed * Time.deltaTime);
			mainCam.transform.position = new Vector3(Mathf.Clamp(mainCam.transform.position.x, -STAGEX / 2, STAGEX / 2), Mathf.Clamp(mainCam.transform.position.y, -STAGEY / 2, STAGEY / 2), mainCam.transform.position.z);
		}
	}

	private void Awake()
	{
		mainCam = Camera.main;
	}

}
