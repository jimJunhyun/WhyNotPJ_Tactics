using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class UnitDetails : MonoBehaviour
{

    public float moveGap;
    public float moveDist = 1;

	[SerializeField]
	 float rayDist = 0.5f;

	public int hp;
	public int hpModifier
	{
		get; set;
	}
	public int atkModifier 
	{
		get; set;
	}
	public int defModifier
	{
		get; set;
	}
	public int CurHp
	{
		get;
		set;
	}
	bool pSide = true;
	public bool PSide
	{
		get=>pSide;
		set=>pSide = value;
	}

	public bool controlable = false;

	public List<AttackRange> attackedBy = new List<AttackRange>();

	public List<InflictedAnomaly> curStatus = new List<InflictedAnomaly>();

	[HideInInspector]
	public bool immunity = false;
	bool movable = true;
	
	float prevMove;

	public virtual void Awake()
	{
		prevMove = 0;
		CurHp = hp;
	}


	public virtual void Update()
	{
		if (controlable) //Temp
		{
			if (Time.time - prevMove >= moveGap)
			{
				movable = true;
			}
			if (movable)
			{
				RaycastHit2D hit;
				bool moved = false;
				if (Input.GetAxisRaw("Horizontal") > 0)
				{
					if (!Physics2D.Raycast(transform.position + Vector3.right * (transform.localScale.x / 1.8f), Vector3.right, rayDist))
					{
						moved = true;
						transform.eulerAngles = new Vector3(0, 0, 270);
						
						transform.Translate(new Vector3(0, moveDist), Space.Self);
						movable = false;
						prevMove = Time.time;
					}

				}
				else if (Input.GetAxisRaw("Horizontal") < 0)
				{
					if (!(hit = Physics2D.Raycast(transform.position - Vector3.right * (transform.localScale.x / 1.8f), Vector3.left, rayDist)))
					{
						
						moved = true;
						transform.eulerAngles = new Vector3(0, 0, 90);
						
						transform.Translate(new Vector3(0, moveDist), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}
				else if (Input.GetAxisRaw("Vertical") > 0)
				{
					if (!(hit = Physics2D.Raycast(transform.position + Vector3.up * (transform.localScale.y / 1.8f), Vector3.up, rayDist)))
					{
						moved = true;
						transform.eulerAngles = new Vector3(0, 0, 0);
						transform.Translate(new Vector3(0, moveDist), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}
				else if (Input.GetAxisRaw("Vertical") < 0)
				{
					
					if (!(hit = Physics2D.Raycast(transform.position - Vector3.up * (transform.localScale.y / 1.8f), Vector3.down, rayDist)))
					{

						moved = true;
						transform.eulerAngles = new Vector3(0, 0, 180);
						transform.Translate(new Vector3(0, moveDist), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}

				if (moved)
				{
					UpdateHandler.instance.fieldUpdateAct.Invoke();
				}
			}
		}
	}

	public virtual void Immobilize()
	{
		movable = false;
	}

	public virtual void Mobilize()
	{
		movable = true;
	}

	

	

	

	
}
