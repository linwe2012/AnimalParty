#### UI 部分

| 文件 or 目录                            | 作用                  |
| --------------------------------------- | --------------------- |
| `UIManager.cs`                          | UI 的 API 接口与管理  |
| `UIHUD.cs`, `UIJoycon`, `UIIventory.cs` | UI 分管不同的子组件   |
| `UIUtils`                               | UI 动画、颜色辅助工具 |
| `UIInternal/`                           | UI 的组件             |
| 其他 `UI*.cs`                           | UI 其他辅助性脚本     |

 

#### 游戏逻辑部分
| 文件 or 目录                 | 作用                                                      |
| ---------------------------- | --------------------------------------------------------- |
| `Process/`                   | 每一个关卡的 UI 控制 (教学, 提示等)，控制每个关卡游戏进程 |
| `Player/*Camera.cs`          | 控制视角的脚本                                            |
| `Player/PlayerController.cs` | 全局的 UI 控制 (开关背包等)                               |
| `Player/PlayerAction.cs`     | 用户动作分析，用户的状态机管理                            |



#### 背包系统

| 文件 or 目录 | 作用                           |
| ------------ | ------------------------------ |
| `ScriptObj/` | 背包系统的物品、背包等数据定义 |



#### AI

在 `Assets\polyperfect\Low Poly Animated Animals\- Prefabs\AI_Script`目录下

包含了每一个 AI 的控制脚本



#### 其他

| 文件 or 目录             | 作用                           |
| ------------------------ | ------------------------------ |
| `Animal/`                | 第五关的小鸡脚本               |
| `Item/`                  | 第五关的玉米粒脚本             |
| `Joycon/`                | Joycon 输入输出和键盘映射      |
| `Objects/`               | 让手持物品跟着手动             |
| `Tutorial/`              | UI 测试脚本                    |
| `FishCurve.cs`           | 钓鱼关卡用到的曲线             |
| `Player/PlayerAction.cs` | 用户动作分析，用户的状态机管理 |

