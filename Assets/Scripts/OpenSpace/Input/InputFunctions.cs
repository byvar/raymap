using UnityEngine;
using System.Collections;
using System;

public abstract class InputFunctions{

    public enum FunctionType {
        _UNKNOWN,
        And,
        Or,
        Not,
        KeyJustPressed,
        KeyJustReleased,
        KeyPressed,
        KeyReleased,
        ActionJustValidated,
        ActionJustInvalidated,
        ActionValidated,
        ActionInvalidated,
        PadJustPressed,
        PadJustReleased,
        PadPressed,
        PadReleased,
        JoystickAxeValue,
        JoystickAngularValue,
        JoystickTrueNormValue,
        JoystickCorrectedNormValue,
        JoystickJustPressed,
        JoystickJustReleased,
        JoystickPressed,
        JoystickReleased,
        JoystickOrPadJustPressed,
        JoystickOrPadJustReleased,
        JoystickOrPadPressed,
        JoystickOrPadReleased,
        MouseAxeValue,
        MouseAxePosition,
        MouseJustPressed,
        MouseJustReleased,
        MousePressed,
        Sequence,
        SequenceKey,
        SequenceKeyEnd,
        SequencePad,
        SequencePadEnd
    }

    public static FunctionType GetFunctionType(uint index)
    {
        try {
            return (FunctionType)(index);
        } catch (Exception) {
            return FunctionType._UNKNOWN;
        }
    }
}
