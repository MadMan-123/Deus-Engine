using System.Numerics;
using Silk.NET.Input;

namespace DeusEngine;

public class InputManager
{
    public static InputManager Instance;

    public IKeyboard CurrentKeyboard;
    public IMouse CurrentMouse;
    public IInputContext CurrentInputContext;
    private Dictionary<Key, bool> KeyStates;
    private Dictionary<MouseButton, bool> MouseButtonStates;
    private Vector2 MousePosition;
    private int iMouseScrollState = 0;
    private bool bMouseScroll = false;
    

    public InputManager(IKeyboard keyboard, IMouse mouse)
    {
        
        if (Instance == null)
            Instance = this;
        //set the keyboard
        CurrentKeyboard = keyboard;
        CurrentMouse = mouse;
        CurrentInputContext = RenderingEngine.window.CreateInput();
        
        KeyStates = new Dictionary<Key, bool>();
        MouseButtonStates = new Dictionary<MouseButton, bool>();

        
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            KeyStates[key] = false;
        }
        
        foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
        {
            MouseButtonStates[button] = false;
        }

        CurrentKeyboard.KeyDown += KeyDown;
        CurrentKeyboard.KeyUp += KeyUp;
        
        // Subscribe to mouse events
        CurrentMouse.MouseDown += MouseDown;
        CurrentMouse.MouseUp += MouseUp;
        CurrentMouse.MouseMove += MouseMove;
        CurrentMouse.Scroll += MouseScroll;
        
    }

    private void MouseScroll(IMouse arg1, ScrollWheel arg2)
    {
        iMouseScrollState = (int)arg2.Y;
        
        bMouseScroll = true;
        Application.MouseScroll = iMouseScrollState;
        
    }

    private void MouseMove(IMouse mouse, Vector2 Position)
    {
        MousePosition = Position;
        Application.MousePosition = MousePosition;
    }

    private void MouseUp(IMouse mouse, MouseButton button)
    {
        MouseButtonStates[button] = false;
    }

    private void MouseDown(IMouse mouse, MouseButton button)
    {
        MouseButtonStates[button] = true;
    }

    private void KeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        KeyStates[key] = true;
    }

    private void KeyUp(IKeyboard keyboard, Key key, int arg3)
    {
        KeyStates[key] = false;
    }
    
    public bool IsKeyDown(Key key)
    {
        if (KeyStates.TryGetValue(key, out var down))
        {
            return down;
        }
        return false;
    }
    public bool IsMouseButtonDown(MouseButton button)
    {
        if (MouseButtonStates.ContainsKey(button))
        {
            return MouseButtonStates[button];
        }
        return false;
    }

    public Vector2 GetMousePosition()
    {
        return MousePosition;
    }
    
}