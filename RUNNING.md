# 运行与发布指南

## 项目结构

```
fullstack/
├── requirement.md                         # 前置条件
├── RUNNING.md                             # 本文件
└── src/
    ├── server/
    │   └── Fullstack.Server/              # ASP.NET Core Web API 10.0
    │       ├── Controllers/               # API 控制器
    │       ├── Models/                    # 数据模型
    │       ├── Data/                      # EF Core 数据库上下文
    │       ├── uploads/avatars/           # 头像文件存储
    │       └── Program.cs                 # 入口
    ├── clients/
    │   ├── web/                           # Vue 3 + Vite SPA（唯一前端源）
    │   │   ├── src/
    │   │   │   ├── api/                   # API 调用封装
    │   │   │   ├── components/            # 组件
    │   │   │   └── types/                 # TypeScript 类型
    │   │   ├── index.html
    │   │   ├── package.json
    │   │   └── vite.config.ts
    │   ├── desktop/
    │   │   └── Fullstack.Desktop/         # Photino.NET 桌面壳
    │   │       ├── Program.cs
    │   │       └── Fullstack.Desktop.csproj
    │   └── mobile/                        # Capacitor 移动壳
    │       ├── capacitor.config.ts
    │       └── package.json
    └── ...
```

---

## 开发环境运行

### 1. 启动服务端

```bash
dotnet run --project src/server/Fullstack.Server
```

服务端默认监听 `http://localhost:5000`，自动创建 SQLite 数据库和头像上传目录。

### 2. 启动前端开发服务器

```bash
cd src/clients/web
npm install
npm run dev
```

前端默认监听 `http://localhost:5173`，已配置代理转发 `/api` 和 `/uploads` 到服务端。

### 3. 启动桌面端

```bash
dotnet run --project src/clients/desktop/Fullstack.Desktop
```

桌面端会打开一个原生 WebView2 窗口，加载服务器页面。可传入 URL 参数指定地址：

```bash
dotnet run --project src/clients/desktop/Fullstack.Desktop -- http://192.168.1.100:5000
```

### 4. 启动移动端

```bash
cd src/clients/mobile
npm install
npx cap add android       # 首次需要，生成 android/ 目录
npx cap add ios           # 首次需要，生成 ios/ 目录
npx cap sync
npx cap open android      # 或 npx cap open ios
```

> **注意：** Android 模拟器访问宿主机使用 `10.0.2.2`，iOS 模拟器可直接用 `localhost`。
> 修改 `capacitor.config.ts` 中的 `server.url` 为对应的服务器地址。

---

## 生产构建与发布

### 构建前端

```bash
cd src/clients/web
npm install
npm run build
# 产物输出到 dist/
```

### 发布服务端

```bash
# 先将前端构建产物复制到 wwwroot
cp -r src/clients/web/dist src/server/Fullstack.Server/wwwroot

# 发布服务端
dotnet publish src/server/Fullstack.Server -c Release -o publish/server
```

### 发布桌面端

```bash
dotnet publish src/clients/desktop/Fullstack.Desktop -c Release -o publish/desktop -r win-x64 --self-contained true
```

### 发布移动端

```bash
# 1. 构建前端
cd src/clients/web && npm run build

# 2. 复制到移动端
cd ../mobile
npx cap copy
npx cap sync

# 3. 用 Android Studio / Xcode 构建发布包
npx cap open android   # 构建 APK / AAB
npx cap open ios       # 构建 IPA
```

---

## API 接口

| 方法   | 路由                      | 说明               |
| ------ | ------------------------- | ------------------ |
| GET    | /api/users                | 获取用户列表       |
| GET    | /api/users/{id}           | 获取单个用户       |
| POST   | /api/users                | 创建用户           |
| PUT    | /api/users/{id}           | 更新用户信息       |
| DELETE | /api/users/{id}           | 删除用户及其头像   |
| POST   | /api/users/{id}/avatar    | 上传头像           |
| DELETE | /api/users/{id}/avatar    | 删除头像           |

### 请求示例

```bash
# 创建用户
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"张三","email":"zhangsan@example.com"}'

# 上传头像
curl -X POST http://localhost:5000/api/users/1/avatar \
  -F "file=@avatar.jpg"
```

---

## 环境变量

| 变量                   | 默认值                   | 说明                        |
| ---------------------- | ------------------------ | --------------------------- |
| ASPNETCORE_URLS        | http://localhost:5000    | 服务端监听地址              |
| ASPNETCORE_ENVIRONMENT | Production               | 环境名称                    |
| VITE_API_BASE_URL      | /api                     | 前端 API 基础路径           |

---

## 部署拓扑

```
                     ┌──────────────────────┐
                     │   ASP.NET Server     │
                     │   :5000              │
                     │   ├─ wwwroot/ (SPA)  │
                     │   └─ uploads/avatars/│
                     └──────────┬───────────┘
                                │
              ┌─────────────────┼─────────────────┐
              │                 │                 │
        ┌─────┴─────┐   ┌──────┴──────┐   ┌──────┴──────┐
        │  Desktop  │   │   Mobile    │   │    Web      │
        │  Photino  │   │  Capacitor  │   │  Browser    │
        │ WebView2  │   │  WebView    │   │  Any OS     │
        └───────────┘   └─────────────┘   └─────────────┘
              ↑                ↑                ↑
              └──── 共享 Vue 3 SPA 代码 ───────┘
```

---

---

## CI/CD — GitHub Actions

每次 `git push` 自动触发 `.github/workflows/ci.yml`，并行执行 7 个 Job：

| Job | 平台 | 产物 |
|-----|------|------|
| Server (win-x64) | windows | `server-win-x64/` |
| Server (osx-x64) | macos | `server-osx-x64/` |
| Server (linux-x64) | ubuntu | `server-linux-x64/` |
| Desktop (win-x64) | windows | `desktop-win-x64/` |
| Desktop (osx-x64) | macos | `desktop-osx-x64/` |
| Desktop (linux-x64) | ubuntu | `desktop-linux-x64/` |
| Web | ubuntu | `web-dist/` (Vue 构建产物) |
| Mobile (Android) | ubuntu | `mobile-android/*.apk` |
| Mobile (iOS) | macos | `mobile-ios/*.app` |

产物自动上传为 GitHub Actions Artifact，可在 Workflow 运行页下载。

> CI 中自动执行 `npx cap add android/ios`，无需提前将 `android/` 和 `ios/` 目录提交到仓库。

---

## 常见问题

### WebView2 Runtime 未安装

Photino 桌面端启动时如果报错，需要安装 WebView2 Runtime：
https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/

### Android 模拟器无法连接服务端

Android 模拟器使用 `10.0.2.2` 映射宿主机 `localhost`。修改 `capacitor.config.ts`：

```ts
server: {
  url: 'http://10.0.2.2:5000',
  cleartext: true,
}
```

### CORS 错误

开发时若前端无法调用 API，确认服务端 CORS 已启用（Program.cs 中配置）。

### SQLite 数据库位置

数据库文件 `fullstack.db` 生成在服务端项目目录下：
```
src/server/Fullstack.Server/fullstack.db
```
