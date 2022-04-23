using BinarySerializer;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using Raymap;
using System;
using System.Diagnostics;
using UnityEngine;

public class EnvironmentController : MonoBehaviour {
	// Set in inspector
	public EnvironmentEntry[] Environments;

	public Unity_Environment InitializeEnvironment(string key) {
		var env = Environments.FindItem(e => e.Key.ToLowerInvariant() == key.ToLowerInvariant());
		Unity_Environment newEnv = Instantiate(env.Value);
		return newEnv;
	}

	[Serializable]
	public struct EnvironmentEntry {
		public string Key;
		public Unity_Environment Value;
	}
}
