
using Netick.Unity; // 确保 NetworkObject 定义在这个命名空间中
using UnityEngine;
using UnityEditor;

[InitializeOnLoad] // 在编辑器加载时自动初始化
public class NetworkEditorHierarchy : MonoBehaviour
{
    public static Texture2D Icon;  // 网络化对象图标
    public static Texture2D UnnetworkedIcon;  // 非网络化对象图标

    // 静态构造函数，编辑器加载时调用
    static NetworkEditorHierarchy()
    {
        // 使用 AssetUtility 加载图标
        Icon =  Resources.Load<Texture2D>("NetworkedIcon");  // 加载网络化图标
        UnnetworkedIcon = Resources.Load<Texture2D>("UnNetworkedIcon");  // 加载非网络化图标

        
        // 注册层级视图的回调函数，更新显示
        EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemOnGUI;
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemOnGUI;
    }
  

    // 在层级视图中的每一项（GameObject）上绘制GUI
    private static void OnHierarchyItemOnGUI(int instanceId, Rect selectionRect)
    {
        // 获取与此 instanceId 相关联的 GameObject
        GameObject obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

        // 如果对象为空或它是 Prefab 实例的一部分，则跳过
        if (obj == null || PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) != null)
            return;

        // 检查 GameObject 是否具有 NetworkObject 组件
        if (obj.TryGetComponent(out NetworkObject networkObject))
        {
            // 确保 networkObject 和它的 Entity 都不为空，然后检查 HasValidId 是否有效
            bool hasValidId = networkObject != null && networkObject.Entity != null && networkObject.HasValidId;

            // 根据 HasValidId 决定显示哪个图标
            var content = new GUIContent(hasValidId ? Icon : UnnetworkedIcon); // 根据网络状态使用相应的图标

            // 调整图标的矩形位置和大小
            var iconRect = selectionRect;
            iconRect.x += (iconRect.height + 200);  // 将图标向左移动
            iconRect.y += !PrefabUtility.IsPartOfAnyPrefab(obj) ? 1f : -1f; // 根据是否是 Prefab 调整 Y 坐标
            iconRect.height = 16f;                 // 设置图标的高度
            iconRect.width = 16f;                  // 设置图标的宽度

            // 创建图标的样式，设置固定大小
            var style = new GUIStyle
            {
                fixedWidth = 15,  // 设置图标宽度
                fixedHeight = 15  // 设置图标高度
            };

            // 在层级视图中绘制图标
            GUI.Label(iconRect, content, style);
        }
    }
}
