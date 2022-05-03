namespace BinarySerializer.Ubisoft.CPA {
    public enum AI_DsgVarType {
        None,
        Boolean,
        Byte,
        UByte, // Unsigned
        Short,
        UShort, // Unsigned
        Int,
        UInt, // Unsigned
        Float,
        Vector,
        List,
        Comport,
        Action,
        Caps, // Capabilities
        Input,
        SoundEvent,
        Light,
        GameMaterial,
        VisualMaterial, // Also an array?
        Perso,
        WayPoint,
        Graph,
        Text,
        SuperObject,
        SOLinks,
        PersoArray,
        VectorArray,
        FloatArray,
        IntegerArray,
        WayPointArray,
        TextArray,
        TextRefArray,
        GraphArray,
        SOLinksArray,
        SoundEventArray,
        VisualMatArray,
        Way, // TT SE only
        ActionArray, // Hype
        SuperObjectArray,
        ObjectList, // Largo
    }
}