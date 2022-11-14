using System;
using BinarySerializer;
using UnityEngine;

namespace Raymap
{
	public class UnitySystemLogger : ISystemLogger
	{
		public void Log(LogLevel logLevel, object log, params object[] args) {
			switch (logLevel) {
				case LogLevel.Error:
					Debug.LogError(String.Format(log?.ToString() ?? String.Empty, args));
					break;
				case LogLevel.Warning:
					Debug.LogWarning(String.Format(log?.ToString() ?? String.Empty, args));
					break;
				case LogLevel.Info:
					Debug.Log(String.Format(log?.ToString() ?? String.Empty, args));
					break;
			}
		}
	}
}