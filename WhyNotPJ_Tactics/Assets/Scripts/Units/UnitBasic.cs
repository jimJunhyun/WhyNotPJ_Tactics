using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Side
{
	Friendly = 1,
	Hostile = 2,
	Neutral = 4,

	
}

[Serializable]
public class AttackRange
{
	public int xDistance;
	public int yDistance;
	public int atk;
	public int atkModifier;

	[HideInInspector]
	public int totalMod;
	[HideInInspector]
	public float totalModMult = 1;

	public int totalAtk { get => (int)((atk + totalMod) * totalModMult); }
	public List<AnomalyIndex> anomaly;
	public List<int> anomalyAmount;
	public void AppendAno(AnomalyIndex info, int amt)
	{
		if (anomaly.Contains(info))
		{
			int idx = anomaly.FindIndex(x => x == info);
			anomalyAmount[idx] += amt;
		}
		else
		{
			anomaly.Add(info);
			anomalyAmount.Add(amt);
		}
	}
	public bool RemoveAno(AnomalyIndex info)
	{
		if (anomaly.Contains(info))
		{
			int idx = anomaly.FindIndex(x => x == info);
			anomaly.RemoveAt(idx);
			anomalyAmount.RemoveAt(idx);
			return true;
		}
		return false;

	}
	[SerializeField]
	int targetSide;
	public int TargetSide
	{
		get => targetSide << 8;
	}

	[HideInInspector]
	public UnitBasic owner;
	public Vector2 myPos => (Vector2)owner.transform.position + new Vector2(xDistance, yDistance);
}


public class UnitBasic : UnitBase
{
    public List<AttackRange> ranges;
	Dictionary<AttackRange, UnitBasic> rangeAttackingPair = new Dictionary<AttackRange, UnitBasic>();

	[HideInInspector]
	public UnitDetails myDet;

	private void Awake()
	{
		
		myDet = GetComponent<UnitDetails>();
		for (int i = 0; i < ranges.Count; i++)
		{
			ranges[i].owner = this;
		}
	}

	private void Start()
	{
		UpdateHandler.instance.allUnits.Add(this);
		UpdateHandler.instance.fieldUpdateAct += OnUpdateAct;
	}

	//부활 이후의 위치 변경이 적용되기 전에 타격이 들어가서 죽어버림.
	//그래서 무적 시간을 적용함.
	//부활 연출 등에도 사용할 수 있을 듯.

	private void OnUpdateAct()
	{
		for (int i = 0; i < ranges.Count; i++)
		{
			ranges[i].totalMod = ranges[i].atkModifier + myDet.atkModifier;
			if (CheckCell(ranges[i], out UnitBasic foundUnit))
			{
				if (!rangeAttackingPair.ContainsKey(ranges[i]))
				{
					rangeAttackingPair.Add(ranges[i], foundUnit);
					foundUnit.Damage(ranges[i]);
				}
			}
			else
			{
				if (rangeAttackingPair.ContainsKey(ranges[i]))
				{
					rangeAttackingPair[ranges[i]].UnDamage(ranges[i]);
					rangeAttackingPair.Remove(ranges[i]);
					
				}
			}
			
		}
		//Debug.Log(myBase.name + " : " + myBase.atkModifier.val);
	}

	bool CheckCell(AttackRange rng, out UnitBasic mover)
	{
		mover = null;
		Vector3 dest = transform.position + (transform.right * 0.5f * rng.xDistance) + (transform.up * 0.5f * rng.yDistance);
		Collider2D c = Physics2D.OverlapBox(dest, Vector2.one * 0.3f, 0, rng.TargetSide);
		if(c != null)
		{
			mover = c.GetComponent<UnitBasic>();
			if (mover != null)
			{

				return true;
			}
		}
		
		return false;
	}

	void InflictDistort(UnitBasic inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if (amt <= 0)
			return;
		if (myDet.curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
		{
			InflictedAnomaly found = myDet.curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
			if (found != null)
			{
				bool prevActivate = found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate;
				found.stacks += amt;
				found.stacks = Mathf.Clamp(found.stacks, 1, StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].maxActivate);
				if (!prevActivate && found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
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
			myDet.curStatus.Add(ano);
			if (amt >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
			{
				ano.info.onActivated?.Invoke(this, inflicter, amt);
			}
			ano.stacks = Mathf.Clamp(ano.stacks, 1, StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].maxActivate);
		}
	}
	bool DisflictDistort(UnitBasic inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if (amt <= 0)
			return false;
		if (myDet.curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
		{
			InflictedAnomaly found = myDet.curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
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
					myDet.curStatus.Remove(found);
					return true;
				}
			}
		}
		return false;
	}

	public virtual void Damage(AttackRange rng, float mult = 1)
	{
		rng.totalModMult = mult;
		myDet.attackedBy.Add(rng);

		for (int i = 0; i < rng.anomaly.Count; i++)
		{
			InflictDistort(rng.owner, rng.anomaly[i], rng.anomalyAmount[i]);
		}
		int sum = 0;
		for (int i = 0; i < myDet.attackedBy.Count; i++)
		{
			sum += Mathf.Max((int)(myDet.attackedBy[i].totalAtk) - myDet.defModifier, 0);
		}
		myDet.CurHp = myDet.hp + myDet.hpModifier - sum;
	}

	public virtual void UnDamage(AttackRange rng)
	{
		rng.totalModMult = 1;
		myDet.attackedBy.Remove(myDet.attackedBy.Find(by => by == rng));

		for (int i = 0; i < rng.anomaly.Count; i++)
		{
			DisflictDistort(rng.owner, rng.anomaly[i], rng.anomalyAmount[i]);
		}

		int sum = 0;
		for (int i = 0; i < myDet.attackedBy.Count; i++)
		{
			sum += Mathf.Max((int)(myDet.attackedBy[i].totalAtk) - myDet.defModifier, 0);
		}
		myDet.CurHp = myDet.hp + myDet.hpModifier - sum;
	}

	public virtual void OnDead()
	{
		if (!myDet.immunity)
		{
			InflictedAnomaly inf = myDet.curStatus.Find(x => x.info.Id == ((int)AnomalyIndex.Revive) + 1);
			if (inf != null && inf.stacks > 0)
			{
				for (int i = 0; i < myDet.attackedBy.Count; i++)
				{
					UnDamage(myDet.attackedBy[i]);
				}
				myDet.attackedBy.Clear();
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

	public void DestActs()
	{
		//Debug.Log("Dest Ref " + name);
		foreach (var item in rangeAttackingPair.Keys)
		{
			rangeAttackingPair[item].UnDamage(item);
		}
		rangeAttackingPair.Clear();

		UpdateHandler.instance.fieldUpdateAct -= OnUpdateAct;
	}

	public void OnDrawGizmos()
	{
		for (int i = 0; i < ranges.Count; i++)
		{
			Vector3 dest = transform.position + (transform.right * 0.5f * ranges[i].xDistance) + (transform.up * 0.5f * ranges[i].yDistance);
			Gizmos.DrawWireCube(dest, Vector3.one * 0.3f);
		}
	}
	IEnumerator ImmunitySecond(float sec)
	{
		myDet.immunity = true;
		yield return new WaitForSeconds(sec);
		myDet.immunity = false;
	}
}
