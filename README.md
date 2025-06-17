# Netick2Peekaboo- 项目 README

## 一、项目概述
Netick2Peekaboo- 是基于Unity引擎的网络项目，整合Netick网络框架与多种插件，支持网络场景加载、玩家相机设置等功能。
## 项目结构
### 2.1 主要文件夹
- **Assets**：项目所有资源文件的存放处，包含脚本、预制体、材质、模型等。
  - **Peekaboo**：项目核心资源文件夹，有多个子文件夹。
    - **Content**：存储模型、材质、UI 等内容资源。
    - **Prefabs**：包含网络相关的预制体。
    - **Scripts**：项目的所有脚本文件，分为 Core、Utils、Editor 等子文件夹。
    - **Settings**：存放脚本化对象设置文件。
    - **Plugins**：包含第三方插件，如 NuGet、CocKleBurs.Steamworks、Facepunch 等。
  - **Netick**：Netick 网络框架相关资源。
  - **TextMesh Pro**：TextMesh Pro 插件相关资源。
- **Packages**：项目使用的 Unity 包配置文件。
- **ProjectSettings**：Unity 项目的设置文件。


  
## 二、主要脚本
2. **PlayerCameraSetup**
   - 实现玩家相机的设置与控制，包括旋转和跟随目标设置。

3. **NetickPrefabLoadingTask**
   - 加载网络预制体并配置Netick网络框架。

4. **NetworkEditorHierarchy**
   - 在Unity编辑器层级视图中显示网络化对象图标。

5. **SimpleNetworkAuthenticator**
   - 实现基于密码比较的网络认证功能。

6. **ManualRefresh**
   - 支持快捷键或菜单选项手动刷新资源。

7. **ClientInstance.RPC**
   - 定义客户端实例的远程过程调用方法。

## 三、依赖项
项目依赖Netick网络框架、UniTask等插件，具体见`Packages/manifest.json`。

## 四、使用方法
1. 克隆项目：`git clone https://github.com/Canglangzi/Netick2Peekaboo-.git`
2. 用Unity 6000.1.0f1或兼容版本打开
3. 自动安装依赖项
4. 选择场景并运行

## 五、许可证
本项目采用MIT许可证，详见`LICENSE`文件。

## 六、贡献
欢迎提交Pull Request或提出Issue。

## 七、联系方式
通过GitHub的Issue功能联系我们。
