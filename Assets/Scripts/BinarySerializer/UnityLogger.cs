using System;
using UnityEngine;
using ILogger = BinarySerializer.ILogger;

namespace Raymap
{
	public class UnityLogger : ILogger
	{
		public void Log(object log, params object[] args) => Debug.Log(String.Format(log?.ToString() ?? String.Empty, args));
		public void LogWarning(object log, params object[] args) => Debug.LogWarning(String.Format(log?.ToString() ?? String.Empty, args));
		public void LogError(object log, params object[] args) => Debug.LogError(String.Format(log?.ToString() ?? String.Empty, args));
	}
}