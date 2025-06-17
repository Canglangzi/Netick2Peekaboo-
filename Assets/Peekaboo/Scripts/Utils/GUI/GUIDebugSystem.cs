//using CocKleBurs.Console;
using UnityEngine;

public class GUIDebugSystem : MonoBehaviour
{
    public static GUIDebugSystem  Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // 检查当前对象是否是子级对象
            if (transform.parent != null)
            {
                // 如果是子级，先移除它与父对象的关系
                transform.SetParent(null);
            }
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public delegate void ToggleHUD();
    public static event ToggleHUD OnToggleHUD;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            OnToggleHUD?.Invoke();
          //  GameDebug.ToggleDebug(!GameDebug.debugGUIVisible);
        }
    }
    private void OnGUI()
    {
       // GameDebug.DrawDebugGUI();
    }
}