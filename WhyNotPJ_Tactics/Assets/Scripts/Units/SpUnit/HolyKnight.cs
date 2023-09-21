using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyKnight : UnitBasic
{
	protected override void InflictDistort(UnitBasic inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if (StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].isBuff)
		{
			if (amt <= 0)
				return;
			if (curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
			{
				InflictedAnomaly found = curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
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
				curStatus.Add(ano);
				if (amt >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
				{
					ano.info.onActivated?.Invoke(this, inflicter, amt);
				}
				ano.stacks = Mathf.Clamp(ano.stacks, 1, StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].maxActivate);
			}
		}
		
	}
}
