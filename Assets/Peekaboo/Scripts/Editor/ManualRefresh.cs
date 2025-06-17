
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad]
public class ManualRefresh : EditorWindow
{
    static ManualRefresh()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        // Check if Ctrl (Cmd on Mac) and S are pressed
        if (Event.current != null && Event.current.type == EventType.KeyDown &&
            (Event.current.control || Event.current.command) && Event.current.keyCode == KeyCode.S)
        {
            PerformRefresh();
        }
    }
    [MenuItem("CLz_GameFrameWork/Tool/ManualRefresh", false, 0)]
    public static void Open()
    {
        PerformRefresh();

    }
    [MenuItem( " Edit/Active GameObject _F1 ")] 
    private static void PerformRefresh()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }

        AssetDatabase.Refresh();
        Debug.Log("开始刷新资源");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (EditorUtility.scriptCompilationFailed)
        {
            Debug.Log("编译错误");
            Close();
            return;
        }

        if (EditorApplication.isCompiling)
        {
            EditorGUILayout.LabelField("正在编译");
            return;
        }

        Close();
    }
}