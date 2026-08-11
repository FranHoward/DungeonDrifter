# Dungeon Drifter

一款俯视角动作 Roguelite Demo（Unity / C#）

## 截图

## 下载

## 核心特性

- 数据驱动的武器系统（ScriptableObject）
- NavMesh 敌人 AI（巡逻 / 追击 / 攻击）
- 程序化随机地图
- 三选一构筑系统
- 掉落与拾取（对象池）

## 操作说明

### 键盘与鼠标

| 操作 | 按键 / 方式 | 说明 |
| --- | --- | --- |
| 移动 | `W` `A` `S` `D` 或方向键 | 控制角色在地面上向前、后、左、右移动 |
| 攻击 | `Space` | 使用当前武器攻击；攻击伤害、范围和冷却由武器及升级共同决定 |
| 选择升级 | 鼠标左键点击升级卡片 | 每击败 3 个敌人后出现 3 个随机升级，选择期间游戏暂停 |
| 拾取金币 | 角色接触金币 | 自动拾取并加入玩家背包 |
| 恢复生命 | 角色接触治疗物品 | 生命未满时自动恢复；满血时不会消耗拾取物 |
| 更换武器 | 角色接触武器掉落 | 自动装备掉落的剑、弓或锤 |
| 死亡重开 | 鼠标左键点击重开按钮 | 玩家死亡后游戏暂停，点击按钮重新加载当前场景 |

### 其他输入设备

移动 Input Action 已配置以下输入：

- Gamepad：左摇杆
- Joystick：摇杆
- XR Controller：Primary 2D Axis

当前攻击逻辑仍直接监听键盘 `Space`。Input Action 资源虽然配置了鼠标左键、手柄右扳机、Joystick Trigger、触屏点击和 XR Primary Action 的 `Fire` 映射，但它们尚未接入实际攻击脚本，因此暂不能视为完整支持。

### 自动行为

- 镜头自动平滑跟随玩家，无需手动控制视角。
- 敌人会在巡逻、追击和攻击状态之间自动切换。
- 房间、NavMesh 和敌人会在场景开始时自动生成。
- 击杀敌人后自动结算升级进度，并按权重生成掉落物。

## 技术栈

### 核心环境

| 类别 | 技术 / 版本 | 用途 |
| --- | --- | --- |
| 游戏引擎 | Unity `6000.3.10f1` | 场景、组件、资源和运行时框架 |
| 编程语言 | C# | 玩法、AI、UI、数据与编辑器工具开发 |
| 渲染管线 | Universal Render Pipeline `17.3.0` | PC 与移动端的通用渲染配置 |
| 输入 | Unity Input System `1.18.0` | 玩家移动和 UI 输入；攻击暂时使用旧版 `Input` API |
| AI 与导航 | AI Navigation `2.0.10`、NavMesh | 敌人寻路，以及随机地图生成后的运行时导航网格构建 |
| UI | Unity UGUI `2.0.0`、TextMesh Pro | 血条、三选一升级卡片、死亡和重开界面 |

### 玩法与工程技术

| 技术 | 项目中的应用 |
| --- | --- |
| ScriptableObject 数据驱动 | 配置武器、敌人、升级项和加权掉落表 |
| 有限状态机 | 管理敌人的巡逻、追击和攻击状态 |
| 程序化生成 | 随机选择房间，通过入口/出口对齐并拼接地图 |
| 运行时 NavMesh | 房间全部生成后构建统一导航网格，并校正敌人出生点 |
| Object Pooling | 复用金币、治疗和武器掉落物，减少重复实例化与销毁 |
| Unity Physics | 使用 Rigidbody 移动、Collider 触发拾取、`Physics.OverlapSphere` 检测攻击命中 |
| 事件驱动 | 用生命、攻击、拾取等事件连接 UI、掉落、升级、音效和反馈系统 |
| AudioSource / AudioClip | 播放武器、受伤、治疗、死亡和拾取音效 |
| Particle System | 播放攻击命中粒子 |
| Coroutine 与非缩放时间 | 实现屏幕震动，并保证暂停状态下反馈仍能正确更新 |
| SceneManager | 玩家死亡后重新加载当前场景 |
| Unity Editor 扩展 | 自动创建升级、掉落资源和 UI，并配置相关预制体 |

### 开发与验证支持

- Unity Test Framework `1.6.0`
- Visual Studio Editor `2.0.26`
- JetBrains Rider Editor `3.0.39`
- Unity Version Control / Collaborate Proxy `2.11.3`
