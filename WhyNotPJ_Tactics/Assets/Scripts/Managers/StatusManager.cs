using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager instance;


    public Anomalies allAnomalies;


	private void Awake()
	{
		instance = this;

		allAnomalies.allAnomalies[0].onActivated += OnDizzyActivate;
		allAnomalies.allAnomalies[0].onUpdated += OnDizzyUpdate;
		allAnomalies.allAnomalies[0].onDisactivated += OnDizzyDisactivate;

		allAnomalies.allAnomalies[1].onActivated += OnEmpowerActivate;
		allAnomalies.allAnomalies[1].onUpdated += OnEmpowerUpdate;
		allAnomalies.allAnomalies[1].onDisactivated += OnEmpowerDisactivate;

		allAnomalies.allAnomalies[2].onActivated += OnVitalActivate;
		allAnomalies.allAnomalies[2].onUpdated += OnVitalUpdate;
		allAnomalies.allAnomalies[2].onDisactivated += OnVitalDisactivate;

		allAnomalies.allAnomalies[3].onActivated += OnReviveActivate;
		allAnomalies.allAnomalies[3].onUpdated += OnReviveUpdate;
		allAnomalies.allAnomalies[3].onDisactivated += OnReviveDisactivate;

		allAnomalies.allAnomalies[4].onActivated += OnProtectActivate;
		allAnomalies.allAnomalies[4].onUpdated += OnProtectUpdate;
		allAnomalies.allAnomalies[4].onDisactivated += OnProtectDisactivate;

		allAnomalies.allAnomalies[5].onActivated += OnInfSourceActivate;
		allAnomalies.allAnomalies[5].onUpdated += OnInfSourceUpdate;
		allAnomalies.allAnomalies[5].onDisactivated += OnInfSourceDisactivate;

		allAnomalies.allAnomalies[6].onActivated += OnInfActivate;
		allAnomalies.allAnomalies[6].onUpdated += OnInfUpdate;
		allAnomalies.allAnomalies[6].onDisactivated += OnInfDisactivate;

		allAnomalies.allAnomalies[7].onActivated += OnLinkActivate;
		allAnomalies.allAnomalies[7].onUpdated += OnLinkUpdate;
		allAnomalies.allAnomalies[7].onDisactivated += OnLinkDisactivate;

		allAnomalies.allAnomalies[8].onActivated += OnAmbActivate;
		allAnomalies.allAnomalies[8].onUpdated += OnAmbUpdate;
		allAnomalies.allAnomalies[8].onDisactivated += OnAmbDisactivate;
		//reflection을 사용하기보다 그냥 손수 하나하나 더하기로 결정.
		//실행속도를 높이기 위함.

	}

	public void OnDizzyActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.Immobilize();
	}

	public void OnDizzyUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{

	}

	public void OnDizzyDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.Mobilize();
	}

	public void OnEmpowerActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.atkModifier += effector.curStatus.Find(x=>x.info.Id == ((int)AnomalyIndex.Empower) + 1).stacks;
	}

	public void OnEmpowerUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.atkModifier += amt;
	}

	public void OnEmpowerDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		
	}

	public void OnVitalActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.hpModifier = effector.curStatus.Find(x=>x.info.Id == ((int)AnomalyIndex.Vital) + 1).stacks;
	}

	public void OnVitalUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.hpModifier += amt;
	}

	public void OnVitalDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		
	}

	public void OnReviveActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		
	}

	public void OnReviveUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{

	}

	public void OnReviveDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		
	}

	public void OnProtectActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.defModifier = effector.curStatus.Find(x => x.info.Id == ((int)AnomalyIndex.Protect) + 1).stacks;
	}

	public void OnProtectUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.defModifier += amt;
	}

	public void OnProtectDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{

	}

	public void OnInfSourceActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		int srcAmt = effector.curStatus.Find(x => x.info.Id == ((int)AnomalyIndex.InfSource) + 1).stacks;
		for (int i = 0; i < effector.ranges.Count; i++)
		{
			effector.ranges[i].AppendAno(AnomalyIndex.Infect, srcAmt);
		}
	}

	public void OnInfSourceUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		for (int i = 0; i < effector.ranges.Count; i++)
		{
			effector.ranges[i].AppendAno(AnomalyIndex.Infect, amt);
		}
	}

	public void OnInfSourceDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		for (int i = 0; i < effector.ranges.Count; i++)
		{
			effector.ranges[i].RemoveAno(AnomalyIndex.Infect);
		}
	}

	public void OnInfActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		int infAmt = effector.curStatus.Find(x => x.info.Id == ((int)AnomalyIndex.Infect) + 1).stacks;
		effector.hpModifier -= infAmt;
	}

	public void OnInfUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.hpModifier -= amt;
	}

	public void OnInfDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
	}

	public void OnLinkActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		Debug.Log(effector.attackedBy.Count + " & " + inflicter.attackedBy.Count);
		effector.attackedBy.AddRange(inflicter.attackedBy.Except(effector.attackedBy));
		inflicter.attackedBy.AddRange(effector.attackedBy.Except(inflicter.attackedBy));
	}

	public void OnLinkUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		if(amt < 0)
		{
			effector.attackedBy = effector.attackedBy.Except(inflicter.attackedBy).ToList();
			inflicter.attackedBy = inflicter.attackedBy.Except(effector.attackedBy).ToList();
		}
		else
		{
			effector.attackedBy.AddRange(inflicter.attackedBy.Except(effector.attackedBy));
			inflicter.attackedBy.AddRange(effector.attackedBy.Except(inflicter.attackedBy));
		}
		
	}

	public void OnLinkDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.attackedBy = effector.attackedBy.Except(inflicter.attackedBy).ToList();
		inflicter.attackedBy = inflicter.attackedBy.Except(effector.attackedBy).ToList();
	}

	public void OnAmbActivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		effector.ChangeSide(10);
	}

	public void OnAmbUpdate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		
	}

	public void OnAmbDisactivate(UnitBasic effector, UnitBasic inflicter, int amt)
	{
		Debug.Log($"{effector.name} <-- {inflicter.name} ? {amt}");
		effector.RecallPrevSide();
	}
}
