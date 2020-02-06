using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class StateTransitionsTreeElement : TreeElement
{
	public int stateToGoIndex;
	public int targetStateIndex;
	public string stateToGoName;
	public string targetStateName;
	public int linkingType;

	public StateTransitionsTreeElement (string name, int depth, int id) : base (name, depth, id)
	{
	}
}