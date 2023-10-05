using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMover : MonoBehaviour
{
    public float speed;
	public Vector2 stageXY;

	Camera mainCam;

    Vector3 prevPos;
	bool mouseDown = false;


	private void Awake()
	{
		prevPos = Vector3.zero;
		mainCam = Camera.main;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			prevPos = Input.mousePosition;
			mouseDown = true;
		}
		if (mouseDown)
		{
			Vector3 posDelta = Input.mousePosition - prevPos;
			prevPos = Input.mousePosition;

			mainCam.transform.Translate(posDelta * -0.1f * speed * Time.deltaTime);
			mainCam.transform.position = new Vector3(Mathf.Clamp(mainCam.transform.position.x, -stageXY.x / 2, stageXY.x / 2), Mathf.Clamp(mainCam.transform.position.y, -stageXY.y / 2, stageXY.y / 2), mainCam.transform.position.z);
		}
		if (Input.GetMouseButtonUp(0))
		{
			mouseDown = false;
		}
	}

}
