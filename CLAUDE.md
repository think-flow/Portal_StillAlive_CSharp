# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Portal_StillAlive 是一个 .NET 9.0 控制台应用程序，用于在终端中播放游戏《Portal》的 "Still Alive" 音乐，同时同步显示歌词、ASCII 艺术动画和制作人员名单。

## Build & Run Commands

```bash
# 构建并运行
dotnet run

# 无音频模式运行（用于终端不支持音频的环境）
dotnet run -- --no-sound

# 发布 AOT 编译版本（Windows）
dotnet publish -c Release -r win-x64 -p:PublishAot=true

# 交叉编译 Linux 版本（需要安装 zig）
./publish_linux.ps1
```

## Architecture

**Core/** - 核心业务逻辑
- `Stage.cs` - 主控制器，管理终端屏幕布局、绘制框架、协调各组件运行
- `Lyric.cs` - 歌词引擎，基于时间轴同步显示歌词和触发事件
- `Credit.cs` - 制作人员名单滚动显示
- `Player.cs` - 跨平台音频播放（Windows: winmm.dll MCI，Linux: mpg123）
- `OutputMsg.cs` - 输出消息结构和 Channel 扩展方法

**Data/** - 静态数据
- `LyricData.cs` - 歌词时间轴数据，定义 Mode 字段控制事件类型（0/1: 歌词, 2: ASCII 艺术, 3: 清屏, 4: 播放音乐, 5: 启动 credits, 9: 结束）
- `AsciiArtsData.cs` - ASCII 艺术图案数组
- `CreditsData.cs` - 制作人员名单文本

**Communication Pattern**: 使用 `System.Threading.Channels` 实现生产者-消费者模式。Lyric/Credit 线程作为生产者写入 `OutputMsg`，Stage 主线程消费并渲染到终端。

## Key Technical Details

- 终端通过 ANSI 转义序列控制（光标定位、屏幕缓冲区切换、颜色）
- 最小终端尺寸要求：80x24
- Windows 下使用 `winmm.dll` P/Invoke 播放音频
- Linux 下依赖 `mpg123` 播放音频
- 支持 VT 终端（vt100 等）和现代终端模拟器
