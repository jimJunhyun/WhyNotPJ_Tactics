using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitCreator : MonoBehaviour, IPointerDownHandler
{
	public UnitDatabase db;
    
	
	int curHoldingNo = -1;
	Vector3 mPos;

	public void Gen()
	{
		if(curHoldingNo != -1)
		{
			mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(mPos.x <= ScreenMover.STAGEX / 2 && mPos.x >= -ScreenMover.STAGEX / 2 && mPos.y <= ScreenMover.STAGEY / 2 && mPos.y >= -ScreenMover.STAGEY / 2)
			{
				Instantiate(db.units[curHoldingNo].unitInfo, ClampVectorHalfScale(mPos), Quaternion.identity);
				curHoldingNo = -1;
			}
		}
	}

    public void HoldDiscard(int num)
	{
		if(curHoldingNo == num)
		{
			curHoldingNo = -1;
		}
		else
		{
			curHoldingNo = num;
		}
	}

	Vector3 ClampVectorHalfScale(Vector3 v)
	{
		Vector3 ret = v * 2;
		Debug.Log(ret);
		ret.x = Mathf.Round(ret.x);
		ret.y = Mathf.Round(ret.y);
		ret.z = 10;
		ret *= 0.5f;
		return ret;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Gen();
	}
}
