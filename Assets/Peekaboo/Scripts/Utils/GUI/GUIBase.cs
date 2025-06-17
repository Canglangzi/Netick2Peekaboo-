using UnityEngine;

public abstract class GUIBase : NetworkCherry
{
    private static bool _isDragging = false;
    private static Vector2 _dragOffset = Vector2.zero;

    protected bool isVisible = true;

    protected virtual void OnEnable()
    {
        GUIDebugSystem.OnToggleHUD += ToggleVisibility;
    }

    protected virtual void OnDisable()
    {
        GUIDebugSystem.OnToggleHUD -= ToggleVisibility;
    }

    private void ToggleVisibility()
    {
        isVisible = !isVisible;
    }

    protected virtual void OnGUI()
    {
        if (isVisible)
        {
            DisplayGUI();
        }
    }

    protected abstract void DisplayGUI();

    protected static void CreateDraggableWindow(string title, ref Rect windowRect, System.Action windowContent)
    {
        windowRect = GUI.Window(0, windowRect, id =>
        {
            GUILayout.BeginVertical();
            GUILayout.Label(title, GUI.skin.window);
            GUILayout.Space(20);
            windowContent?.Invoke();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }, title);

        HandleMouseEvents(ref windowRect);
    }

    private static void HandleMouseEvents(ref Rect windowRect)
    {
        Event currentEvent = Event.current;
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                if (currentEvent.button == 0 && windowRect.Contains(currentEvent.mousePosition))
                {
                    _isDragging = true;
                    _dragOffset = currentEvent.mousePosition - new Vector2(windowRect.x, windowRect.y);
                    GUI.FocusWindow(GUIUtility.GetControlID(FocusType.Passive));
                }
                break;
            case EventType.MouseDrag:
                if (_isDragging && currentEvent.button == 0)
                {
                    windowRect.position = currentEvent.mousePosition - _dragOffset;
                }
                break;
            case EventType.MouseUp:
                if (currentEvent.button == 0)
                {
                    _isDragging = false;
                }
                break;
        }
    }
}