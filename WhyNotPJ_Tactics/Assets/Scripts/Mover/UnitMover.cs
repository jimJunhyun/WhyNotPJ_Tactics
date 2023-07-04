using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class UnitMover : MonoBehaviour
{
    public float moveGap;
    public float moveDist = 1;

	public float rayDist = 0.5f;

	public int hp;
	public RefableInt hpModifier
	{
		get
		{
			RefableInt sum = new RefableInt(0);
			for (int i = 0; i < hpScopes.Count; i++)
			{
				sum += hpScopes[i].val;
			}
			return sum;
		}
		set
		{
			hpScopes.Add(value);
		}
	}
	public RefableInt atkModifier 
	{
		get
		{
			RefableInt sum = new RefableInt(0);
			for (int i = 0; i < hpScopes.Count; i++)
			{
				sum += atkScopes[i];
			}
			return sum;
		}
		set
		{
			atkScopes.Add(value);
		}
	}
	public int CurHp
	{
		get
		{
			int sum = 0;
			for (int i = 0; i < attackedBy.Count; i++)
			{
				sum += attackedBy[i].atk;
			}
			return hp + hpModifier.val - sum;
		}
	}

	public bool controlable = false;

	public List<AttackRange> attackedBy;

	public List<InflictedAnomaly> curStatus = new List<InflictedAnomaly>();
	List<RefableInt> hpScopes = new List<RefableInt>();
	List<RefableInt> atkScopes = new List<RefableInt>();
	

	MoverChecker mover;

	bool movable = true;
	float prevMove;

	private void Awake()
	{
		prevMove = 0;
		mover = GetComponent<MoverChecker>();
	}

	private void Update()
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
				if (Input.GetAxisRaw("Horizontal") > 0)
				{
					if (!Physics2D.Raycast(transform.position + Vector3.right * (transform.localScale.x / 1.8f), Vector3.right, rayDist))
					{
						transform.eulerAngles = new Vector3(0, 0, 0);
						transform.Translate(new Vector3(moveDist, 0), Space.Self);
						movable = false;
						prevMove = Time.time;
					}

				}
				else if (Input.GetAxisRaw("Horizontal") < 0)
				{
					if (!(hit = Physics2D.Raycast(transform.position - Vector3.right * (transform.localScale.x / 1.8f), Vector3.left, rayDist)))
					{
						transform.eulerAngles = new Vector3(0, 0, 180);
						transform.Translate(new Vector3(moveDist, 0), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}
				else if (Input.GetAxisRaw("Vertical") > 0)
				{
					if (!Physics2D.Raycast(transform.position + Vector3.up * (transform.localScale.y / 1.8f), Vector3.up, rayDist))
					{
						transform.eulerAngles = new Vector3(0, 0, 90);
						transform.Translate(new Vector3(moveDist, 0), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}
				else if (Input.GetAxisRaw("Vertical") < 0)
				{
					if (!Physics2D.Raycast(transform.position - Vector3.up * (transform.localScale.y / 1.8f), Vector3.down, rayDist))
					{
						transform.eulerAngles = new Vector3(0, 0, 270);
						transform.Translate(new Vector3(moveDist, 0), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}

			}
		}
		
		
		
	}

	private void LateUpdate()
	{
		if (CurHp <= 0)
		{
			Destroy(gameObject);
		}
	}

	public void Immobilize()
	{
		movable = false;
	}

	public void Mobilize()
	{
		movable = true;
	}

	public void RemoveHpScope(RefableInt val)
	{
		hpScopes.Remove(val);
	}

	public void RemoveAtkScope(RefableInt val)
	{
		atkScopes.Remove(val);
	}

	public void InflictDistort(MoverChecker inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if(amt <= 0)
			return;
		if(curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
		{
			InflictedAnomaly found = curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
			if(found != null && found.stacks < StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].maxActivate)
			{
				bool prevActivate = found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate;
				found.stacks += amt;
				if(!prevActivate && found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
				{
					found.info.onActivated?.Invoke(this, inflicter);
				}
			}
		}
		else
		{
			InflictedAnomaly ano = new InflictedAnomaly(StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)], amt);
			curStatus.Add(ano);
			if(amt >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
			{
				ano.info.onActivated?.Invoke(this, inflicter);
			}
		}
	}
	public void DisflictDistort(MoverChecker inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if(amt <= 0)
			return;
		if (curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
		{
			InflictedAnomaly found = curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
			if (found != null)
			{
				bool prevActivate = found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate;
				found.stacks -= amt;
				if (prevActivate && found.stacks < StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
				{
					found.info.onDisactivated?.Invoke(this, inflicter);
				}
				if (found.stacks <= 0)
				{
					curStatus.Remove(found);
				}
			}
		}
	}
}
