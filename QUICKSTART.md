# 🚀 快速部署到 Railway

## 5 分钟部署指南

### 第 1 步：推送到 GitHub (2 分钟)

```bash
# 初始化 Git（如果还没有）
git init

# 添加所有文件
git add .

# 提交
git commit -m "Ready for Railway deployment"

# 创建 main 分支
git branch -M main

# 添加远程仓库（替换成你的 GitHub 仓库地址）
git remote add origin https://github.com/你的用户名/orange-hr-management.git

# 推送
git push -u origin main
```

### 第 2 步：部署到 Railway (3 分钟)

1. **访问 Railway**
   - 打开 https://railway.app
   - 点击 "Login" 使用 GitHub 登录

2. **创建新项目**
   - 点击 "New Project"
   - 选择 "Deploy from GitHub repo"
   - 选择你的 `orange-hr-management` 仓库

3. **添加数据库**
   - 点击 "+ New"
   - 选择 "Database"
   - 选择 "Add PostgreSQL"
   - 等待数据库创建完成

4. **等待部署**
   - Railway 会自动开始构建
   - 等待 3-5 分钟
   - 看到 "Success" 表示部署成功

5. **生成域名**
   - 点击你的服务
   - 进入 "Settings" 标签
   - 找到 "Networking" 部分
   - 点击 "Generate Domain"
   - 复制生成的 URL（例如：`https://orange-hr-production.up.railway.app`）

### 第 3 步：测试应用

访问你的 Railway URL，使用以下账号登录：

**HR 管理员**
```
Email: sarah.chen@orange.com
Password: Orange123!
```

**经理**
```
Email: michael.rodriguez@orange.com
Password: Orange123!
```

**员工**
```
Email: emily.martinez@orange.com
Password: Orange123!
```

## ✅ 完成！

你的应用现在已经在线了！

## 🔄 自动部署

每次你推送代码到 GitHub，Railway 会自动重新部署：

```bash
# 修改代码
git add .
git commit -m "Update feature"
git push

# Railway 会自动检测并重新部署
```

## 📊 查看日志

1. 在 Railway 项目页面
2. 点击你的服务
3. 点击 "Deployments" 标签
4. 选择最新的部署
5. 查看实时日志

## 🎯 Preview 部署

想要为 Pull Request 创建预览环境？

1. 创建新分支：`git checkout -b feature/new-feature`
2. 推送到 GitHub：`git push origin feature/new-feature`
3. 在 GitHub 创建 Pull Request
4. Railway 会自动为 PR 创建独立的预览环境
5. 每个 PR 都有自己的 URL

## 💰 成本

- **免费额度**: $5/月
- 对于演示项目完全够用
- 超出后按使用量计费

## ❓ 遇到问题？

查看详细的部署指南：[DEPLOYMENT.md](./DEPLOYMENT.md)

## 🎉 分享你的项目

现在你可以：
- 把 Railway URL 添加到简历
- 分享给招聘人员
- 在 GitHub README 中添加 Live Demo 链接

```markdown
## Live Demo

🔗 [View Live Demo](https://your-app.up.railway.app)

Test Accounts:
- HR: sarah.chen@orange.com / Orange123!
- Manager: michael.rodriguez@orange.com / Orange123!
- Employee: emily.martinez@orange.com / Orange123!
```
