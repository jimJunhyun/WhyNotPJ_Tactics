using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Anomaly Table")]
public class Anomalies : ScriptableObject
{
    public List<Anomaly> allAnomalies;
}
