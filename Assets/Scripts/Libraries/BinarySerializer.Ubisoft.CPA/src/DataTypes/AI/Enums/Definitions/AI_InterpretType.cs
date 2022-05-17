﻿namespace BinarySerializer.Ubisoft.CPA {
    public enum AI_InterpretType {
        Unknown,
        KeyWord,
        Condition,
        Operator,
        Function,
        Procedure,
        MetaAction,
        BeginMacro,
        EndMacro,
        Field,
        DsgVarRef,
        Constant,
        Real,
        Button,
        ConstantVector,
        Vector,
        Mask,
        ModuleRef,
        DsgVarId,
        String,
        LipsSynchroRef,
        FamilyRef,
        PersoRef,
        ActionRef,
        SuperObjectRef,
        WayPointRef,
        TextRef,
        ComportRef,
        SoundEventRef,
        ObjectTableRef,
        GameMaterialRef,
        ParticleGenerator,
        VisualMaterialRef,
        ModelRef,
        CustomBits,
        Caps,
        MacroRef__Subroutine,
        Null,
        GraphRef,
        // Types below here added for engineversions < R2
        //ConstantRef,
        //RealRef,
        SurfaceRef,
        Way,
        DsgVar,
        SectorRef,
        EnvironmentRef,
        FontRef,
        Color,
        Module, // Different from ModuleRef
        //LightInfoRef,

        // R3:
        EndTree,
        SOLinksRef,
        Light,
        Graph,
        CineRef,

		// Revo:
		Placeholder__R2PS2__Type2E,
		Placeholder__Largo__Type2F,
		Placeholder__Largo__Type30,
	};
}
