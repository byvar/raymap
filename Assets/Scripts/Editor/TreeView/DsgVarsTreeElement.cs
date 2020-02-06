using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DsgVarsTreeElement : TreeElement
{
	public DsgVarComponent.DsgVarEditableEntry entry;
	public int? arrayIndex = null;
	public bool sourceCurrentFromDSGMem = false;
	public bool sourceInitialFromDSGMem = false;

	public DsgVarsTreeElement (string name, int depth, int id) : base (name, depth, id)
	{
	}
}