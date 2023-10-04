using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

struct UpdateAct
{
	public Action<AttackRange> act;
	public AttackRange range;

	public UpdateAct(Action<AttackRange> act, AttackRange rng)
	{
		this.act = act;
		range = rng;
	}
}

public class UpdateHandler : MonoBehaviour
{
	public static UpdateHandler instance;

	List<UpdateAct> updActs = new List<UpdateAct>();

	public List<UnitBasic> destTargets = new List<UnitBasic>();

	public List<UnitBasic> allUnits = new List<UnitBasic>();

	Action fieldUpdateAct;

	Coroutine c;

	int requestCalls = 1;

	private void Awake()
	{
		instance = this;
	}

	// Update is called once per frame
	void LateUpdate()
    {
		for (int i = 0; i < allUnits.Count; i++)
		{
			//Debug.Log(allUnits[i].name + " : " + allUnits[i].CurHp);
			if (allUnits[i].CurHp <= 0)
			{
				StartCoroutine(DelayDie(0, allUnits[i].OnDead));
			}
		}
		if (updActs.Count > 0)
		{
			InvokeActs();
		}
		if (destTargets.Count > 0)
		{
			if (c == null)
			{
				Debug.Log("START DEST");
				c = StartCoroutine(DelayDie(4, InvokeDests));
			}
		}
		if(requestCalls > 0)
		{
			StartCoroutine(DelInvoker());
		}
	}

	IEnumerator DelInvoker()
	{
		yield return null;
		Debug.Log("UPD");
		fieldUpdateAct?.Invoke();
		requestCalls = 0;
	}

	void InvokeActs()
	{
		for (int i = 0; i < updActs.Count; i++)
		{
			updActs[i].act.Invoke(updActs[i].range);
		}
		updActs.Clear();
	}

	void InvokeDests()
	{
		for (int i = 0; i < destTargets.Count; i++)
		{
			if(destTargets[i] != null)
			{
				destTargets[i].GetComponent<UnitBasic>().DestActs();
			}
			
		}
		for (int i = 0; i < destTargets.Count; i++)
		{
			if (destTargets[i] != null)
			{
				allUnits.Remove(destTargets[i]);
				destTargets[i].gameObject.SetActive(false);
				StopAllCoroutines();
			}

		}
				
		destTargets.Clear();
		StopAllCoroutines();
	}

	public void AddFieldUpdateActs(Action act)
	{
		fieldUpdateAct += act;
	}
	public void RemoveFieldUpdateActs(Action act)
	{
		fieldUpdateAct -= act;
	}

	public void RequestFUpdate()
	{
		requestCalls += 1;
	}

    public void AddUpdater(Action<AttackRange> act, AttackRange rng)
	{
		UpdateAct updAct = new UpdateAct(act, rng);

		updActs.Add(updAct);
	}

	IEnumerator DelayDie(float frame, Action act)
	{
		yield return null;
		for (int i = 0; i < frame - 1; i++)
		{
			yield return null;
		}
		act.Invoke();
		c = null;
	}
}
