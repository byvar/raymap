using OpenSpace.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.Exporter {
    abstract class EntryActionExporter {

        public static string EntryActionToCSharpClass(EntryAction action)
        {
            string str = EntryActionToCSharp(action);

            Regex entryActionRegex = new Regex("EntryAction\\{([^,]+), (.+)\\}");
            Match entryActionMatch = entryActionRegex.Match(str);

            if (!entryActionMatch.Success) {
                return "";
            }

            string entryActionName = entryActionMatch.Groups[1].Value.Replace("\"", "");

            str = entryActionRegex.Replace(str,
                "public class " + entryActionName + " : EntryAction { " + Environment.NewLine +
                "public override bool Check() {" + Environment.NewLine +
                "return $2;" + Environment.NewLine +
                "}" + Environment.NewLine +
                "}"
            );

            return str;
        }

        public static string EntryActionToCSharp(EntryAction action)
        {
            string name = "EntryAction_" + action.offset.offset;

            string str = action.ToBasicString();

            Regex entryActionRegex = new Regex("EntryAction\\{([^,]+), (.+)\\}");

            // Check if the EntryAction is not empty
            Match entryActionMatch = entryActionRegex.Match(str);

            if (!entryActionMatch.Success) {
                return "";
            }

            // Keyboard
            Regex regexKeyPressed = new Regex("KeyPressed\\(([^\\)]+)\\)");
            Regex regexKeyJustPressed = new Regex("KeyJustPressed\\(([^\\)]+)\\)");
            Regex regexKeyReleased = new Regex("KeyReleased\\(([^\\)]+)\\)");
            Regex regexKeyJustReleased = new Regex("KeyJustReleased\\(([^\\)]+)\\)");

            str = regexKeyPressed.Replace(str, (m) =>
            {
                return "UnityEngine.Input.GetKey(UnityEngine.KeyCode." + ConvertKeyCode(m.Groups[1].Value) + ")";
            });

            str = regexKeyJustPressed.Replace(str, (m) =>
            {
                return "UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode." + ConvertKeyCode(m.Groups[1].Value) + ")";
            });

            str = regexKeyReleased.Replace(str, (m) =>
            {
                return "(!UnityEngine.Input.GetKey(UnityEngine.KeyCode." + ConvertKeyCode(m.Groups[1].Value) + "))";
            });

            str = regexKeyJustReleased.Replace(str, (m) =>
            {
                return "UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode." + ConvertKeyCode(m.Groups[1].Value) + ")";
            });

            // Joystick
            Regex regexJoystickAxeValue = new Regex("JoystickAxeValue\\(([XY]), ([\\-0-9]+), ([\\-0-9]+)\\)");
            str = regexJoystickAxeValue.Replace(str, "Controller.InputManager.JoystickAxeValue(\"$1\", $2, $3)");

            Regex regexJoystickPressed = new Regex("JoystickOrPadPressed\\(([^)]+)\\)");
            str = regexJoystickPressed.Replace(str, "UnityEngine.Input.GetButton(\"$1\")");

            // ActionValidated
            Regex regexActionValidated = new Regex("ActionValidated\\{EntryAction\\{(\\\"[^\\\"]+\\\"), ([^\\}]+)[\\}]+");
            str = regexActionValidated.Replace(str, "Controller.InputManager.IsActionValidated($1)");

            // Sequences
            str = str.Replace("Sequence", "Controller.InputManager.CheckSequence");

            return str;
        }

        private static string ConvertKeyCode(string originalKeyCode)
        {
            switch (originalKeyCode) {
                case "Left": return UnityEngine.KeyCode.LeftArrow.ToString();
                case "Right": return UnityEngine.KeyCode.RightArrow.ToString(); ;
                case "Up": return UnityEngine.KeyCode.UpArrow.ToString(); ;
                case "Down": return UnityEngine.KeyCode.DownArrow.ToString();

                case "Enter": return UnityEngine.KeyCode.Return.ToString();
                case "LControl": return UnityEngine.KeyCode.LeftControl.ToString();
                case "RControl": return UnityEngine.KeyCode.RightControl.ToString();
                case "Shift": return UnityEngine.KeyCode.LeftShift.ToString();

                case "Num0": return UnityEngine.KeyCode.Keypad0.ToString();
                case "Num1": return UnityEngine.KeyCode.Keypad1.ToString();
                case "Num2": return UnityEngine.KeyCode.Keypad2.ToString();
                case "Num3": return UnityEngine.KeyCode.Keypad3.ToString();
                case "Num4": return UnityEngine.KeyCode.Keypad4.ToString();
                case "Num5": return UnityEngine.KeyCode.Keypad5.ToString();
                case "Num6": return UnityEngine.KeyCode.Keypad6.ToString();
                case "Num7": return UnityEngine.KeyCode.Keypad7.ToString();
                case "Num8": return UnityEngine.KeyCode.Keypad8.ToString();
                case "Num9": return UnityEngine.KeyCode.Keypad9.ToString();
                case "NumAdd": return UnityEngine.KeyCode.KeypadPlus.ToString();
                case "NumSubtract": return UnityEngine.KeyCode.KeypadMinus.ToString();

                case "Next": return UnityEngine.KeyCode.PageDown.ToString();

                case "One": return UnityEngine.KeyCode.Alpha1.ToString();
                case "Two": return UnityEngine.KeyCode.Alpha2.ToString();
            }

            return originalKeyCode;
        }
    }
}
