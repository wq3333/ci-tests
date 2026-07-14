<script setup lang="ts">
import { ref } from 'vue'
import { createUser, updateUser, uploadAvatar, deleteAvatar } from '../api/user'
import type { User } from '../types/user'

const props = defineProps<{ user: User | null }>()
const emit = defineEmits<{ close: []; save: [] }>()

const name = ref(props.user?.name || '')
const email = ref(props.user?.email || '')
const avatarFile = ref<File | null>(null)
const avatarPreview = ref(props.user?.avatar || '')
const saving = ref(false)

function onFileChange(event: Event) {
  const input = event.target as HTMLInputElement
  if (input.files && input.files[0]) {
    avatarFile.value = input.files[0]
    avatarPreview.value = URL.createObjectURL(input.files[0])
  }
}

async function handleSubmit() {
  saving.value = true
  try {
    if (props.user?.id) {
      await updateUser(props.user.id, { name: name.value, email: email.value })
      if (avatarFile.value) {
        await uploadAvatar(props.user.id, avatarFile.value)
      }
    } else {
      const newUser = await createUser({ name: name.value, email: email.value })
      if (avatarFile.value && newUser.id) {
        await uploadAvatar(newUser.id, avatarFile.value)
      }
    }
    emit('save')
  } finally {
    saving.value = false
  }
}

async function handleRemoveAvatar() {
  if (props.user?.id) {
    await deleteAvatar(props.user.id)
    avatarPreview.value = ''
    avatarFile.value = null
  }
}
</script>

<template>
  <div class="modal-overlay" @click.self="emit('close')">
    <div class="modal">
      <h2>{{ user ? '编辑用户' : '添加用户' }}</h2>
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label>姓名</label>
          <input v-model="name" required />
        </div>
        <div class="form-group">
          <label>邮箱</label>
          <input v-model="email" type="email" required />
        </div>
        <div class="form-group">
          <label>头像</label>
          <div class="avatar-upload">
            <img v-if="avatarPreview" :src="avatarPreview" class="preview" />
            <input type="file" accept="image/*" @change="onFileChange" />
            <button
              v-if="user?.id && avatarPreview"
              type="button"
              @click="handleRemoveAvatar"
              class="btn btn-small btn-danger"
            >
              删除头像
            </button>
          </div>
        </div>
        <div class="actions">
          <button type="button" @click="emit('close')" class="btn">取消</button>
          <button type="submit" class="btn btn-primary" :disabled="saving">
            {{ saving ? '保存中...' : '保存' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  min-width: 400px;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
}

.form-group input[type="text"],
.form-group input[type="email"] {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.avatar-upload {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.preview {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  object-fit: cover;
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  margin-top: 1rem;
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
  font-size: 0.85em;
}
</style>
