using System;
using System.Collections.Generic;
using UnityEngine;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class FieldHelpAttribute : PropertyAttribute
{
    public string HelpText { get; }

    public FieldHelpAttribute(string helpText)
    {
        HelpText = helpText;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ThreadSafeAttribute : Attribute
{
    public bool ThrowsException { get; set; }

    public ThreadSafeAttribute(bool throwsException)
    {
        ThrowsException = throwsException;
    }
}

public enum HashAlgorithmType
{
    SHA256,
    MD5
}
public enum GameObjectType
{
    None,
    Player,
    Enemy,
    Item,
    NPC,
    Environment
}
public class PrefabPreviewAttribute : PropertyAttribute
{
    public string Label { get; private set; }

    public PrefabPreviewAttribute(string label = "Prefab Preview")
    {
        Label = label;
    }
}
[Serializable]
public class WeakReferenceObject
{
    [SerializeField]
    private string resourcePath; // 资源路径

    public WeakReferenceObject(string path)
    {
        resourcePath = path;
    }

    public string ResourcePath => resourcePath;

    // 使用此方法来更新路径
    public void SetResourcePath(string path)
    {
        resourcePath = path;
    }
}
public class BoxGroup : PropertyAttribute
{
    public string name;

    public BoxGroup(string name)
    {
        this.name = name;
    }
}
public class ProgressBarAttribute : PropertyAttribute
{
}
public class ResetButtonAttribute : PropertyAttribute
{
}
public class UserHintAttribute : PropertyAttribute
{
    public string Hint;

    public UserHintAttribute(string hint)
    {
        Hint = hint;
    }
}
public class SeparatorAttribute : PropertyAttribute
{
}
public class FilePathSelectorAttribute : PropertyAttribute
{
}
public class ConditionalShowAttribute : PropertyAttribute
{
    public string ConditionField;

    public ConditionalShowAttribute(string conditionField)
    {
        ConditionField = conditionField;
    }
}
public class ShowIfAttribute : PropertyAttribute
{
    public string ConditionField;
    public bool DesiredValue;

    public ShowIfAttribute(string conditionField, bool desiredValue)
    {
        ConditionField = conditionField;
        DesiredValue = desiredValue;
    }
}
public class MinMaxSliderAttribute : PropertyAttribute
{
    public float MinLimit, MaxLimit;

    public MinMaxSliderAttribute(float min, float max)
    {
        MinLimit = min;
        MaxLimit = max;
    }
}
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class DisableNetworkEntityCheckAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ListenerTagAttribute : Attribute
{
    public string Tag { get; }

    public ListenerTagAttribute(string tag)
    {
        Tag = tag;
    }
}
public enum CalculationType
{
    Additive,
    Multiplicative,
    Percentage
}
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class EarlyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class LateAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BeforeAttribute : Attribute
{
    public Type TargetType { get; }

    public BeforeAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AfterAttribute : Attribute
{
    public Type TargetType { get; }

    public AfterAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}
public class SerializableDictionaryAttribute : PropertyAttribute { }

public class SliderAttribute : PropertyAttribute
{
    public float min;
    public float max;

    public SliderAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

public class ImagePreviewAttribute : PropertyAttribute { }
public class ColorAttribute : PropertyAttribute { }
public class FoldoutAttribute : PropertyAttribute
{
    public string label;

    public FoldoutAttribute(string label)
    {
        this.label = label;
    }
}

public class ReadOnlyAttribute : PropertyAttribute { }
public class RequireComponentExAttribute : PropertyAttribute{}


public class CKBPropertyAttribute : PropertyAttribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class CodeGeneratorAttribute : Attribute
{
    public CodeGeneratorFlags Flags { get; }
    public string Callback { get; }

    public CodeGeneratorAttribute(CodeGeneratorFlags flags, string callback)
    {
        Flags = flags;
        Callback = callback;
    }
}

[Flags]
public enum CodeGeneratorFlags
{
    WrapMethod = 1,
    WrapPropertySet = 2,
    WrapPropertyGet = 4,
    Instance = 8
}


[AttributeUsage(AttributeTargets.Method)]
public class ServerAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public class ClientAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public class HostAttribute : Attribute {}
[AttributeUsage(AttributeTargets.Method)]
public class DebugAttribute : Attribute{
}
[AttributeUsage(AttributeTargets.Method)]
public class GameDebugAttribute : Attribute
{
}
public enum NetworkSpawnMethod
{
    Default,
}
public enum TickType
{
    Predicted,      // 预测的 Tick（客户端）
    Authoritative,  // 权威的 Tick（服务器）
    Current         // 当前的 Tick，无论是在客户端还是服务器
}
public enum Stage
{
    PhysicsStep,
    Update,
    LateUpdate,
    Render,
    // 其他阶段可以根据需求添加
}
public enum ScriptHeaderBackColor
{
    None,
    Gray,
    Blue,
    Red,
    Green,
    Orange,
    Black,
    Steel,
    Sand,
    Olive,
    Cyan,
    Violet
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CKBScriptInfo : Attribute
{
    public string Title { get; }
    public string Content { get; }
    public ScriptHeaderBackColor BackgroundColorOption { get; }

    // Mapping from enum to actual color
    private static readonly Dictionary<ScriptHeaderBackColor, Color> ColorMap = new Dictionary<ScriptHeaderBackColor, Color>
    {
        { ScriptHeaderBackColor.None, Color.clear },
        { ScriptHeaderBackColor.Gray, new Color(0.35f, 0.35f, 0.35f) },
        { ScriptHeaderBackColor.Blue, new Color(0.15f, 0.25f, 0.6f) },
        { ScriptHeaderBackColor.Red, new Color(0.5f, 0.1f, 0.1f) },
        { ScriptHeaderBackColor.Green, new Color(0.1f, 0.4f, 0.1f) },
        { ScriptHeaderBackColor.Orange, new Color(0.6f, 0.35f, 0.1f) },
        { ScriptHeaderBackColor.Black, new Color(0.1f, 0.1f, 0.1f) },
        { ScriptHeaderBackColor.Steel, new Color(0.32f, 0.35f, 0.38f) },
        { ScriptHeaderBackColor.Sand, new Color(0.38f, 0.35f, 0.32f) },
        { ScriptHeaderBackColor.Olive, new Color(0.25f, 0.33f, 0.15f) },
        { ScriptHeaderBackColor.Cyan, new Color(0.25f, 0.5f, 0.5f) },
        { ScriptHeaderBackColor.Violet, new Color(0.35f, 0.2f, 0.4f) }
    };

    public CKBScriptInfo(string title = "", string content = "", ScriptHeaderBackColor backgroundColor = ScriptHeaderBackColor.Blue)
    {
        Title = title;
        Content = content;
        BackgroundColorOption = backgroundColor;
    }

    public Color GetBackgroundColor()
    {
        return ColorMap.TryGetValue(BackgroundColorOption, out Color color) ? color : Color.blue;
    }
}


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class CKBFieldHelpAttribute : PropertyAttribute
{
    public string HelpText { get; }

    public CKBFieldHelpAttribute(string helpText)
    {
        HelpText = helpText;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class StartAfterAttribute : Attribute
{
    public Type TargetType { get; }

    public StartAfterAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class StartBeforeAttribute : Attribute
{
    public Type TargetType { get; }

    public StartBeforeAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}


public class AssetReferenceAttribute : PropertyAttribute
{
    public System.Type AssetType { get; private set; }

    public AssetReferenceAttribute(System.Type assetType)
    {
        AssetType = assetType;
    }
}
public enum NetworkManagerMode
{
    Offline,
    ServerOnly,
    ClientOnly,
    Host,
}  public interface ILocalPlayer
{
    int LocalPlayerId { get;set; }
    void LocalPlayerInitialize();
    void LocalPlayerCleanup();
}
public interface IManagedSubsystem
{
   
    void OnStart();
    void Destroy();
}
