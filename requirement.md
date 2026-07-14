# 前置条件

## 必需

### .NET SDK 10.0

ASP.NET Core 服务端和 Photino 桌面端都需要 .NET 10.0 SDK。

- 版本要求: 10.0.301+
- 下载地址: https://dotnet.microsoft.com/download/dotnet/10.0
- 验证命令:

```bash
dotnet --version
```

### Node.js 18+ 与 npm

Vue 3 前端开发和 Capacitor 移动端构建需要 Node.js。

- 下载地址: https://nodejs.org/
- 验证命令:

```bash
node --version
npm --version
```

### WebView2 Runtime

Photino 桌面端依赖 WebView2 渲染网页。

- Windows 11 / Windows 10 已内置
- 旧系统手动安装: https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/

---

## 可选（按需安装）

| 平台    | 依赖                              | 验证命令           |
| ------- | --------------------------------- | ------------------ |
| Android | JDK 17+                           | `java --version`   |
| Android | Android Studio (含 SDK 34+)       | 通过 SDK Manager   |
| iOS     | macOS + Xcode 15+                 | `xcode-select -p`  |
| iOS     | CocoaPods                         | `pod --version`    |

---

## 推荐工具

| 工具 | 用途 |
|------|------|
| Visual Studio 2022 / VS Code + C# 扩展 | 开发 .NET 项目 |
| VS Code + Volar 扩展 | 开发 Vue 3 |
| Android Studio | 编译 Android 端 |
| Xcode | 编译 iOS 端 |
| Postman / curl | 测试 API |
