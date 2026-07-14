<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getUsers, deleteUser } from '../api/user'
import type { User } from '../types/user'
import UserForm from './UserForm.vue'

const users = ref<User[]>([])
const showForm = ref(false)
const editingUser = ref<User | null>(null)

async function loadUsers() {
  users.value = await getUsers()
}

function handleEdit(user: User) {
  editingUser.value = { ...user }
  showForm.value = true
}

function handleAdd() {
  editingUser.value = null
  showForm.value = true
}

async function handleDelete(id: number) {
  if (confirm('确认删除该用户？')) {
    await deleteUser(id)
    await loadUsers()
  }
}

function handleFormClose() {
  showForm.value = false
  editingUser.value = null
}

async function handleFormSave() {
  showForm.value = false
  editingUser.value = null
  await loadUsers()
}

onMounted(loadUsers)
</script>

<template>
  <div class="user-list">
    <div class="header">
      <h1>用户管理</h1>
      <button @click="handleAdd" class="btn btn-primary">添加用户</button>
    </div>

    <table>
      <thead>
        <tr>
          <th>头像</th>
          <th>姓名</th>
          <th>邮箱</th>
          <th>创建时间</th>
          <th>操作</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in users" :key="user.id">
          <td>
            <img v-if="user.avatar" :src="user.avatar" class="avatar" />
            <span v-else class="avatar-placeholder">无</span>
          </td>
          <td>{{ user.name }}</td>
          <td>{{ user.email }}</td>
          <td>{{ user.createdAt ? new Date(user.createdAt).toLocaleDateString() : '' }}</td>
          <td>
            <button @click="handleEdit(user)" class="btn btn-small">编辑</button>
            <button @click="handleDelete(user.id!)" class="btn btn-small btn-danger">删除</button>
          </td>
        </tr>
        <tr v-if="users.length === 0">
          <td colspan="5" class="empty">暂无用户数据</td>
        </tr>
      </tbody>
    </table>

    <UserForm
      v-if="showForm"
      :user="editingUser"
      @close="handleFormClose"
      @save="handleFormSave"
    />
  </div>
</template>

<style scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.avatar,
.avatar-placeholder {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
}

.avatar-placeholder {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: #eee;
  font-size: 0.8em;
  color: #999;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  padding: 0.5rem;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

.empty {
  text-align: center;
  color: #999;
  padding: 2rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.btn-primary {
  background: #409eff;
  color: white;
}

.btn-danger {
  background: #f56c6c;
  color: white;
}

.btn-small {
  padding: 0.25rem 0.5rem;
  margin-right: 0.25rem;
  font-size: 0.85em;
}
</style>
