﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WebJSON {
	public class Message {
		// Mandatory
		public MessageType Type { get; set; }

		// Optional
		public Settings Settings { get; set; }
		public Highlight Highlight { get; set; }
		public Selection Selection { get; set; }
		public Hierarchy Hierarchy { get; set; }
		public Localization Localization { get; set; }
		public Request Request { get; set; }
		public SuperObject SuperObject { get; set; }
		public Perso Perso { get; set; }
		public Script Script { get; set; }
	}
	public class Settings {
		public bool? ViewCollision { get; set; }
		public bool? ViewGraphs { get; set; }
		public bool? ViewInvisible { get; set; }
		public bool? EnableLighting { get; set; }
		public bool? EnableFog { get; set; }
		public float? Luminosity { get; set; }
		public bool? Saturate { get; set; }
		public bool? DisplayInactive { get; set; }
		public bool? PlayAnimations { get; set; }
		public bool? PlayTextureAnimations { get; set; }
		public bool? ShowPersos { get; set; }
	}
	public class SuperObject {
		public OpenSpace.Object.SuperObject.Type? Type { get; set; }

		public SuperObject[] Children { get; set; }

		// Optional
		public OpenSpace.Pointer Offset { get; set; }
		public Perso Perso { get; set; }
		public string Name { get; set; }
		public Vector3? Position { get; set; }
		public Vector3? Rotation { get; set; }
		public Vector3? Scale { get; set; }
	}
	public class Perso {
		public PersoType Type { get; set; } = PersoType.Instance;
		public OpenSpace.Pointer Offset { get; set; }

		// Optional
		public string Name { get; set; }
		public Vector3? Position { get; set; }
		public Vector3? Rotation { get; set; }
		public Vector3? Scale { get; set; }

		// PersoBehaviour Stuff
		public string NameFamily { get; set; }
		public string NameModel { get; set; }
		public string NameInstance { get; set; }
		public bool? IsEnabled { get; set; }
		public int? State { get; set; }
		public int? ObjectList { get; set; }
		public bool? PlayAnimation { get; set; }
		public float? AnimationSpeed { get; set; }
		public bool? AutoNextState { get; set; }

		// Extra lists
		public State[] States { get; set; }
		public string[] ObjectLists { get; set; }
		public Brain Brain { get; set; }
	}
	public class State {
		public string Name { get; set; }
		public Transition[] Transitions { get; set; }

		public class Transition {
			public int StateToGo { get; set; }
			public int TargetState { get; set; }
			public int LinkingType { get; set; }
		}
	}
	public class Brain {
		public Comport[] Intelligence { get; set; }
		public Comport[] Reflex { get; set; }
		public Macro[] Macros { get; set; }
		public DsgVar[] DsgVars { get; set; }
	}
	public class Comport {
		public string Name { get; set; }
		public Script FirstScript { get; set; }
		public Script[] Scripts { get; set; }
	}
	public class Macro {
		public string Name { get; set; }
		public Script Script { get; set; }
	}
	public class Script {
		public OpenSpace.Pointer Offset { get; set; }
		public string Translation { get; set; }
	}

	public class Selection {
		public bool View { get; set; }
		public Perso Perso { get; set; }
		public SuperObject SuperObject { get; set; }
	}
	public class Highlight {
		public Perso Perso { get; set; }
	}
	public class Hierarchy {
		public SuperObject ActualWorld { get; set; }
		public SuperObject TransitDynamicWorld { get; set; }
		public SuperObject DynamicWorld { get; set; } // PS1
		public SuperObject FatherSector { get; set; } // ROM, PS1
		public SuperObject[] DynamicSuperObjects { get; set; } // ROM
		public Always Always { get; set; }
	}
	public class Always {
		public Perso[] SpawnablePersos { get; set; }
	}
	public class DsgVar {
		public string Name { get; set; }
		public OpenSpace.AI.DsgVarInfoEntry.DsgVarType Type { get; set; }
		public DsgVarValue ValueCurrent { get; set; }
		public DsgVarValue ValueInitial { get; set; }
		public DsgVarValue ValueModel { get; set; }
	}
	public class DsgVarValue {
		public OpenSpace.AI.DsgVarInfoEntry.DsgVarType Type { get; set; }
		public bool? AsBoolean { get; set; }
		public sbyte? AsByte { get; set; }
		public byte? AsUByte { get; set; }
		public short? AsShort { get; set; }
		public ushort? AsUShort { get; set; }
		public int? AsInt { get; set; }
		public uint? AsUInt { get; set; }
		public float? AsFloat { get; set; }
		public uint? AsCaps { get; set; }
		public Vector3? AsVector { get; set; }
		public int? AsText { get; set; }
		public Perso AsPerso { get; set; }
		public SuperObject AsSuperObject { get; set; }
		public WayPoint AsWayPoint { get; set; }
		public Graph AsGraph { get; set; }
		public DsgVarValue[] AsArray { get; set; }
		public DsgState AsAction { get; set; }
	}
	public class DsgState {
		public string Name { get; set; }
	}
	public class WayPoint {
		public string Name { get; set; }
	}
	public class Graph {
		public string Name { get; set; }
	}
	public class Request {
		public RequestType Type { get; set; }

		// Optional
		public OpenSpace.Pointer ScriptOffset { get; set; }
		public BehaviorType? BehaviorType { get; set; }
	}
	public class Localization {
		public Language Common { get; set; }
		public Language[] Languages { get; set; }
		public int CommonStart { get; set; }
		public int LanguageStart { get; set; }

		public class Language {
			public string Name { get; set; }
			public string NameLocalized { get; set; }
			public string[] Entries { get; set; }
		}
	}

	#region Enums
	public enum MessageType {
		Hierarchy,
		Settings,
		Highlight,
		Selection,
		Request,
		Script
	}
	public enum PersoType {
		Instance,
		Always,
	}
	public enum RequestType {
		None,
		Script
	}
	public enum BehaviorType {
		Intelligence,
		Reflex,
		Macro
	}
	#endregion
}