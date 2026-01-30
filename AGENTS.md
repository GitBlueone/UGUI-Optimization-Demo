# Unity UGUI Optimization Demo - 代理开发指南

> 本文档为AI编码代理（Agent）在此Unity项目中的工作指南

## 项目概述

- **Unity版本**: 6000.0.5f1
- **渲染管线**: Universal Render Pipeline (URP) 17.0.3
- **C#语言版本**: 9.0
- **目标平台**: StandaloneWindows64
- **项目类型**: URP模板项目（目前主要是教程说明，非实际UGUI优化演示）

---

## 构建与测试

### 构建命令
```bash
# Unity项目通过Unity Editor构建，无命令行构建脚本
# 在Unity Editor中: File > Build Settings > Build
```

### 测试运行
```bash
# 使用Unity Test Framework运行测试
# 方式1: Unity Editor > Window > General > Test Runner
# 方式2: 命令行（需要Unity路径）
Unity.exe -runTests -batchmode -projectPath [项目路径] -testResults [结果文件]
```

### 运行单个测试
```bash
# 在Unity Test Runner中右键点击具体测试 > Run
# 或使用测试脚本路径过滤
```

---

## 代码风格指南

### 命名约定

| 类型 | 约定 | 示例 |
|------|------|------|
| **类名** | PascalCase | `Readme`, `ReadmeEditor`, `Section` |
| **方法名** | PascalCase | `SelectReadmeAutomatically()`, `RemoveTutorial()` |
| **公共字段** | camelCase | `icon`, `title`, `heading` |
| **私有字段** | `m_`前缀 + PascalCase | `m_LinkStyle`, `m_Initialized` |
| **静态字段** | `s_`前缀 + camelCase | `s_ShowedReadmeSessionStateName` |
| **常量** | `k_`前缀 + PascalCase | `k_Space` |
| **参数/局部变量** | camelCase | `readme`, `position` |

### Using语句组织
```csharp
// 顺序：System命名空间 > 第三方库 > Unity命名空间 > Editor命名空间
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
```

### 命名空间使用
⚠️ **当前项目未使用命名空间** - 新代码应考虑添加命名空间进行组织：
```csharp
namespace UGUOptimization.Scripts
{
    public class YourScript : MonoBehaviour { }
}
```

### 注释风格
- **公共API**: 使用XML文档注释（`///`）
```csharp
/// <summary>
/// ScriptableObject数据容器，用于存储项目README信息
/// </summary>
public class Readme : ScriptableObject { }
```
- **内部逻辑**: 使用中文单行注释
```csharp
// 移除教程资源
void RemoveTutorial() { }
```

### 封装原则
- **公共数据**: 考虑使用属性代替公共字段
```csharp
[SerializeField] private Texture2D m_Icon;
public Texture2D Icon => m_Icon;
```
- **序列化字段**: 使用`[SerializeField]`特性保持封装性

---

## Unity特定约定

### 目录结构
```
Assets/
├── Scripts/          # Runtime脚本（MonoBehaviour, ScriptableObject等）
├── Editor/           # Editor专用脚本（必须在Editor文件夹或使用#UNITY_EDITOR）
├── Scenes/           # 场景文件
└── Settings/         # 项目设置资源
```

### Assembly Definition（.asmdef）
⚠️ **当前项目未使用Assembly Definition文件**

推荐使用以下结构组织代码：
- `ProjectName.Runtime.asmdef` - Runtime代码
- `ProjectName.Editor.asmdef` - Editor代码（引用Runtime）

### Unity特性使用

| 特性 | 用途 |
|------|------|
| `[Serializable]` | 使类/结构体可在Inspector中序列化 |
| `[SerializeField]` | 序列化私有字段 |
| `[CreateAssetMenu]` | 在Create菜单中添加ScriptableObject |
| `[CustomEditor]` | 自定义Inspector编辑器 |
| `[InitializeOnLoad]` | Editor启动时初始化静态类 |
| `[RuntimeInitializeOnLoadMethod]` | Runtime启动时调用静态方法 |

### Editor脚本规范
```csharp
using UnityEditor;
using UnityEngine;

// 自定义Inspector
[CustomEditor(typeof(YourTargetType))]
public class YourEditor : Editor
{
    // 目标对象访问
    private YourTargetType Target => (YourTargetType)target;

    public override void OnInspectorGUI()
    {
        // 自定义GUI代码
    }
}
```

### MonoBehaviour生命周期
```csharp
public class YourBehaviour : MonoBehaviour
{
    // 初始化顺序：Awake > OnEnable > Start
    void Awake() { }        // 脚本实例化时调用
    void OnEnable() { }     // 对象启用时调用
    void Start() { }        // 第一帧前调用

    // 物理更新（固定时间步）
    void FixedUpdate() { }

    // 帧更新
    void Update() { }

    // 后处理
    void LateUpdate() { }

    // 清理
    void OnDisable() { }    // 对象禁用时调用
    void OnDestroy() { }    // 对象销毁时调用
}
```

---

## 错误处理

### Unity日志
```csharp
Debug.Log("普通信息");
Debug.LogWarning("警告信息");
Debug.LogError("错误信息");
```

### 编辑器验证
```csharp
// 在Editor脚本中使用
if (string.IsNullOrEmpty(fieldName))
{
    EditorGUILayout.HelpBox("字段不能为空", MessageType.Error);
    return;
}
```

### Null检查
```csharp
// Unity对象使用Unity专用null检查
if (unityObject == null) { }

// 避免使用？.操作符处理Unity对象（可能导致问题）
// 推荐：显式null检查
```

---

## 性能优化建议

### UGUI优化
- Canvas分组：减少Canvas数量，合理使用RenderMode
- 合批：相同图集的UI元素会自动合批
- 布局:避免嵌套过深的Layout Group
- 填充：避免每帧动态修改布局元素
- 图集：使用Sprite Atlas合并图片
- 字体：使用动态字体合理设置字符集

### 通用优化
- 对象池：重用GameObject而非频繁Instantiate/Destroy
- 协程：使用Coroutine替代定时Update
- 缓存：缓存GetComponent结果到私有字段
- 清理：及时销毁未使用的资源

---

## 注意事项

### 禁止事项
- ❌ 修改Library目录中的文件（自动生成）
- ❌ 修改Assembly-CSharp.csproj（自动生成）
- ❌ 使用`@SuppressMessage`抑制警告（修复根源问题）
- ❌ 在循环中分配内存（导致GC压力）
- ❌ 使用`as any`或`@ts-expect-error`（Unity C#不需要）

### 推荐做法
- ✅ 使用ScriptableObject存储配置数据
- ✅ 使用AssetBundle或Addressables管理资源
- ✅ 使用事件系统解耦组件通信
- ✅ 使用Object Pooling管理频繁创建销毁的对象
- ✅ Editor工具放在Editor文件夹

---

## 资源链接

- [Unity UGUI官方文档](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/index.html)
- [Unity ScriptableObject指南](https://docs.unity3d.com/Manual/class-ScriptableObject.html)
- [Unity Test Framework文档](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
- [URP文档](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
