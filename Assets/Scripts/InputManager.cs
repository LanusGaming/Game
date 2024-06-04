using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Game;

public class InputManager : MonoBehaviour
{
    public static bool MoveUp { get => Held(Settings.Controls.MoveUp); }
    public static bool MoveDown { get => Held(Settings.Controls.MoveDown); }
    public static bool MoveLeft { get => Held(Settings.Controls.MoveLeft); }
    public static bool MoveRight { get => Held(Settings.Controls.MoveRight); }
    public static bool InteractPressed { get => Pressed(Settings.Controls.Interact); }
    public static bool OpenMapPressed { get => Pressed(Settings.Controls.OpenMap); }
    public static bool ExitPressed { get => Pressed(Settings.Controls.Exit); }

    public static Vector2 MoveDirection {
        get
        {
            Vector2 direction = Vector2.zero;

            if (MoveUp)
                direction += Vector2.up;
            if (MoveDown)
                direction += Vector2.down;
            if (MoveLeft)
                direction += Vector2.left;
            if (MoveRight)
                direction += Vector2.right;

            return direction.normalized;
        }
    }

    public static KeyCode[] ActiveKeys { get => activeKeys.ToArray(); }

    private static InputManager instance;
    private static Stack<string> focusStack = new Stack<string>();
    private static List<KeyCode> activeKeys = new List<KeyCode>();

    private void Awake()
    {
        instance = this;
    }

    private void OnGUI()
    {
        Event current = Event.current;

        if (current != null && current.isKey && current.keyCode != KeyCode.None)
        {
            if (current.type == EventType.KeyDown && !activeKeys.Contains(current.keyCode))
                activeKeys.Add(current.keyCode);
            else if (current.type == EventType.KeyUp)
                activeKeys.Remove(current.keyCode);
        }
    }

    public static bool Held(Settings.Controls.Control control, string id = null)
    {
        if (!InFocus(id))
            return false;

        return Input.GetKey(control.primaryKey) || Input.GetKey(control.secondaryKey);
    }

    public static bool Pressed(Settings.Controls.Control control, string id = null)
    {
        if (!InFocus(id))
            return false;

        return Input.GetKeyDown(control.primaryKey) || Input.GetKeyDown(control.secondaryKey);
    }

    public static void Focus(string id)
    {
        focusStack.Push(id);
    }

    public static void Unfocus()
    {
        focusStack.Pop();
    }

    public static void UnfocusAll()
    {
        focusStack.Clear();
    }

    private static bool InFocus(string id)
    {
        return focusStack.Count == 0 || focusStack.Peek() == id;
    }
}
