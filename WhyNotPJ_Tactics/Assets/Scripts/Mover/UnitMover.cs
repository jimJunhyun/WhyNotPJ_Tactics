using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct AttackedBy
{
	public AttackRange range;
	public float multi;

	public AttackedBy(AttackRange rng, float mult)
	{
		range = rng;
		multi = mult;
	}
}

public class UnitMover : MonoBehaviour
{
    public float moveGap;
    public float moveDist = 1;

	public float rayDist = 0.5f;

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

	public List<AttackedBy> attackedBy;

	public List<InflictedAnomaly> curStatus = new List<InflictedAnomaly>();


	bool immunity = false;
	bool movable = true;
	
	float prevMove;

	private void Awake()
	{
		prevMove = 0;
		CurHp = hp;
	}

	private void Start()
	{
		UpdateHandler.instance.allUnits.Add(this);
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
				for (int i = 0; i < attackedBy.Count; i++)
				{
					UnDamage(attackedBy[i].range);
				}
				attackedBy.Clear();
				DisflictDistort(null, AnomalyIndex.Revive);
				transform.position += Vector3.one - Vector3.forward; // TEST
				StartCoroutine(ImmunitySecond(1f));
			}
			else
			{
				UpdateHandler.instance.destTargets.Add(this);
			}
		}
	}

	public void Damage(AttackRange rng, float mult = 1f)
	{
		attackedBy.Add(new AttackedBy(rng, mult));

		for (int i = 0; i < rng.anomaly.Count; i++)
		{
			InflictDistort(rng.owner, rng.anomaly[i], rng.anomalyAmount[i]);
		}
		int sum = 0;
		for (int i = 0; i < attackedBy.Count; i++)
		{
			sum += Mathf.Max((int)(attackedBy[i].range.totalAtk * attackedBy[i].multi) - defModifier, 0);
		}
		CurHp = hp + hpModifier - sum;
	}

	public void UnDamage(AttackRange rng)
	{
		attackedBy.Remove(attackedBy.Find(by => by.range == rng));

		for (int i = 0; i < rng.anomaly.Count; i++)
		{
			DisflictDistort(rng.owner, rng.anomaly[i], rng.anomalyAmount[i]);
		}

		int sum = 0;
		for (int i = 0; i < attackedBy.Count; i++)
		{
			sum += Mathf.Max((int)(attackedBy[i].range.totalAtk * attackedBy[i].multi) - defModifier, 0);
		}
		CurHp = hp + hpModifier - sum;
	}

	void InflictDistort(MoverChecker inflicter, AnomalyIndex anomaly, int amt = 1)
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
				else if (prevActivate)
				{
					found.info.onUpdated?.Invoke(this, inflicter, amt);
				}
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
		}
	}
	bool DisflictDistort(MoverChecker inflicter, AnomalyIndex anomaly, int amt = 1)
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
