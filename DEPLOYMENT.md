# Railway 部署指南

## 准备工作

1. **确保代码已推送到 GitHub**
```bash
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/你的用户名/orange-hr-management.git
git push -u origin main
```

## Railway 部署步骤

### 1. 创建 Railway 账号
- 访问 [railway.app](https://railway.app)
- 使用 GitHub 账号登录

### 2. 创建新项目
- 点击 "Start a New Project"
- 选择 "Deploy from GitHub repo"
- 授权 Railway 访问你的 GitHub
- 选择 `orange-hr-management` 仓库

### 3. 添加 PostgreSQL 数据库
- 在项目页面，点击 "+ New"
- 选择 "Database"
- 选择 "Add PostgreSQL"
- Railway 会自动创建数据库并设置 `DATABASE_URL` 环境变量

### 4. 配置应用
Railway 会自动检测 .NET 项目并使用 `nixpacks.toml` 配置。

**自动配置的内容：**
- ✅ 构建命令
- ✅ 启动命令
- ✅ 数据库连接（通过 DATABASE_URL）
- ✅ 端口配置（通过 PORT 环境变量）

### 5. 部署
- Railway 会自动开始构建和部署
- 等待 3-5 分钟完成首次部署
- 应用会自动运行数据库迁移和种子数据

### 6. 获取 URL
- 在项目设置中，点击 "Settings"
- 找到 "Domains" 部分
- 点击 "Generate Domain"
- Railway 会生成一个公开 URL，例如：`https://orange-hr-production.up.railway.app`

## 测试账号

部署成功后，使用以下账号登录：

**HR 管理员**
- Email: sarah.chen@orange.com
- Password: Orange123!

**经理**
- Email: michael.rodriguez@orange.com
- Password: Orange123!

**员工**
- Email: emily.martinez@orange.com
- Password: Orange123!

## 自动部署

每次推送到 GitHub main 分支，Railway 会自动：
1. 拉取最新代码
2. 重新构建应用
3. 运行数据库迁移
4. 重新部署

## 环境变量

Railway 自动配置以下变量：
- `DATABASE_URL` - PostgreSQL 连接字符串
- `PORT` - 应用端口（通常是 8080）

可选配置：
- `ASPNETCORE_ENVIRONMENT` - 设置为 `Production`（默认）

## 故障排查

### 查看日志
1. 在 Railway 项目页面
2. 点击你的服务
3. 点击 "Deployments"
4. 选择最新的部署
5. 查看构建和运行日志

### 常见问题

**问题：构建失败**
- 检查 `nixpacks.toml` 配置是否正确
- 确保所有 NuGet 包都能正常还原

**问题：应用启动失败**
- 检查数据库连接字符串
- 确保 PostgreSQL 服务正在运行
- 查看应用日志了解详细错误

**问题：数据库连接失败**
- 确保 PostgreSQL 数据库已添加到项目
- 检查 `DATABASE_URL` 环境变量是否存在
- 验证连接字符串格式

## 成本

Railway 提供：
- **免费额度**: $5/月
- **Hobby Plan**: $5/月（超出免费额度后）
- **Pro Plan**: $20/月（更多资源）

对于演示项目，免费额度通常足够使用。

## 数据库备份

Railway 自动备份 PostgreSQL 数据库。你也可以手动导出：

```bash
# 安装 Railway CLI
npm i -g @railway/cli

# 登录
railway login

# 连接到数据库
railway connect postgres

# 导出数据
pg_dump > backup.sql
```

## 更新应用

```bash
# 修改代码
git add .
git commit -m "Update feature"
git push

# Railway 会自动重新部署
```

## Preview 部署

Railway 支持 PR 预览：
1. 创建新分支
2. 推送到 GitHub
3. 创建 Pull Request
4. Railway 会自动为 PR 创建预览环境
5. 每个 PR 都有独立的 URL

## 监控

在 Railway 项目页面可以查看：
- CPU 使用率
- 内存使用率
- 网络流量
- 请求日志
- 错误日志

## 支持

如有问题：
- Railway 文档: https://docs.railway.app
- Railway Discord: https://discord.gg/railway
- GitHub Issues: 在你的仓库创建 issue
