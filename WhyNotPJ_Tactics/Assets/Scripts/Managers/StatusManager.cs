using System.Collections;
using System.Collections.Generic;
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

		allAnomalies.allAnomalies[5].onActivated += OnPlagueActivate;
		allAnomalies.allAnomalies[5].onUpdated += OnPlagueUpdate;
		allAnomalies.allAnomalies[5].onDisactivated += OnPlagueDisactivate;
		//reflection을 사용하기보다 그냥 손수 하나하나 더하기로 결정.
		//실행속도를 높이기 위함.

	}

	public void OnDizzyActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.Immobilize();
	}

	public void OnDizzyUpdate(UnitMover effector, MoverChecker inflicter, int amt)
	{

	}

	public void OnDizzyDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.Mobilize();
	}

	public void OnEmpowerActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.atkModifier = effector.curStatus.Find(x=>x.info.Id == ((int)AnomalyIndex.Empower) + 1).stacks;
	}

	public void OnEmpowerUpdate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.atkModifier += amt;
	}

	public void OnEmpowerDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		
	}

	public void OnVitalActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.hpModifier = effector.curStatus.Find(x=>x.info.Id == ((int)AnomalyIndex.Vital) + 1).stacks;
	}

	public void OnVitalUpdate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.hpModifier += amt;
	}

	public void OnVitalDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		
	}

	public void OnReviveActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		
	}

	public void OnReviveUpdate(UnitMover effector, MoverChecker inflicter, int amt)
	{

	}

	public void OnReviveDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		
	}

	public void OnProtectActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		Debug.Log("PROT");
		effector.defModifier = effector.curStatus.Find(x => x.info.Id == ((int)AnomalyIndex.Protect) + 1).stacks;
	}

	public void OnProtectUpdate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.defModifier += amt;
	}

	public void OnProtectDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{

	}

	public void OnPlagueActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		
	}

	public void OnPlagueUpdate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		
	}

	public void OnPlagueDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{

	}
}
