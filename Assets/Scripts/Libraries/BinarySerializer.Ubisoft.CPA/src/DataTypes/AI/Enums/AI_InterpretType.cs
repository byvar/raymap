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
        VisualMaterial,
        ModelRef,
        DataType42,
        CustomBits,
        Caps,
        MacroRef__Subroutine,
        Null,
        GraphRef,
        // Types below here added for engineversions < R2
        ConstantRef,
        RealRef,
        SurfaceRef,
        Way,
        DsgVar,
        SectorRef,
        EnvironmentRef,
        FontRef,
        Color,
        Module, // Different from ModuleRef
        LightInfoRef,

        // R3:
        EndTree,
        SOLinksRef,
        Light,
        Graph,
        CineRef
    };
}
