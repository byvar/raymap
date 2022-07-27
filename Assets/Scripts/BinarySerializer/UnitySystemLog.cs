using System;
using UnityEngine;
using ISystemLog = BinarySerializer.ISystemLog;

namespace Raymap
{
	public class UnitySystemLog : ISystemLog
	{
		public void Log(BinarySerializer.LogLevel logLevel, object log, params object[] args) {
			switch (logLevel) {
				case BinarySerializer.LogLevel.Error:
					Debug.LogError(String.Format(log?.ToString() ?? String.Empty, args));
					break;
				case BinarySerializer.LogLevel.Warning:
					Debug.LogWarning(String.Format(log?.ToString() ?? String.Empty, args));
					break;
				case BinarySerializer.LogLevel.Info:
					Debug.Log(String.Format(log?.ToString() ?? String.Empty, args));
					break;
			}
		}
	}
}