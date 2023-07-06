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
	public int prot;
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
		get
		{
			int sum = 0;
			for (int i = 0; i < attackedBy.Count; i++)
			{
				sum += Mathf.Max(attackedBy[i].totalAtk - prot, 0);
				//Debug.Log($"{attackedBy[i].atk} + {attackedBy[i].atkModifier} = {attackedBy[i].totalAtk}");
			}
			return hp + hpModifier - sum;
		}
	}
	bool pSide = true;
	public bool PSide
	{
		get=>pSide;
		set=>pSide = value;
	}

	public bool controlable = false;

	public List<AttackRange> attackedBy;

	public List<InflictedAnomaly> curStatus = new List<InflictedAnomaly>();


	bool immunity = false;
	bool movable = true;
	
	float prevMove;

	private void Awake()
	{
		prevMove = 0;
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
						transform.eulerAngles = new Vector3(0, 0, 90);
						
						transform.Translate(new Vector3(0, moveDist), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}
				else if (Input.GetAxisRaw("Vertical") > 0)
				{
					if (!Physics2D.Raycast(transform.position + Vector3.up * (transform.localScale.y / 1.8f), Vector3.up, rayDist))
					{
						transform.eulerAngles = new Vector3(0, 0, 0);
						transform.Translate(new Vector3(0, moveDist), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}
				else if (Input.GetAxisRaw("Vertical") < 0)
				{
					if (!Physics2D.Raycast(transform.position - Vector3.up * (transform.localScale.y / 1.8f), Vector3.down, rayDist))
					{
						transform.eulerAngles = new Vector3(0, 0, 180);
						transform.Translate(new Vector3(0, moveDist), Space.Self);
						movable = false;
						prevMove = Time.time;
					}
				}
			}
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

	public void OnDead()
	{
		if (!immunity)
		{
			InflictedAnomaly inf = curStatus.Find(x => x.info.Id == ((int)AnomalyIndex.Revive) + 1);
			if (inf != null && inf.stacks > 0)
			{
				attackedBy.Clear(); //?
				DisflictDistort(GetComponent<MoverChecker>(), AnomalyIndex.Revive);
				//UnityEditor.EditorApplication.isPaused = true;
				transform.position += Vector3.one - Vector3.forward; // TEST
				StartCoroutine(ImmunitySecond(1f));
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public void InflictDistort(MoverChecker inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if(amt <= 0)
			return;
		if(curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
		{
			InflictedAnomaly found = curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
			if(found != null)
			{
				bool prevActivate = found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate;
				found.stacks += amt;
				found.stacks = Mathf.Clamp(found.stacks, 1, StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].maxActivate);
				if(!prevActivate && found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
				{
					found.info.onActivated?.Invoke(this, inflicter, amt);
				}
				found.info.onUpdated?.Invoke(this, inflicter, amt);
			}
		}
		else
		{
			InflictedAnomaly ano = new InflictedAnomaly(StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)], amt);
			curStatus.Add(ano);
			if(amt >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
			{
				ano.info.onActivated?.Invoke(this, inflicter, amt);
			}
			ano.stacks = Mathf.Clamp(ano.stacks, 1, StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].maxActivate);
			ano.info.onUpdated?.Invoke(this, inflicter, amt);
		}
	}
	public bool DisflictDistort(MoverChecker inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if(amt <= 0)
			return false;
		if (curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
		{
			InflictedAnomaly found = curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
			if (found != null)
			{
				bool prevActivate = found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate;
				found.stacks -= amt;
				found.info.onUpdated?.Invoke(this, inflicter, -amt);
				if (prevActivate && found.stacks < StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
				{
					found.info.onDisactivated?.Invoke(this, inflicter, amt);
				}
				if (found.stacks <= 0)
				{
					curStatus.Remove(found);
					return true;
				}
			}
		}
		return false;
	}

	IEnumerator ImmunitySecond(float sec)
	{
		immunity = true;
		yield return new WaitForSeconds(sec);
		immunity = false;
	}
}
