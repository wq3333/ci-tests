import axios from 'axios'
import type { User } from '../types/user'

const api = axios.create({
  baseURL: 'http://192.168.10.36:5000/api',
})

export async function getUsers(): Promise<User[]> {
  const res = await api.get('/users')
  return res.data
}

export async function getUser(id: number): Promise<User> {
  const res = await api.get(`/users/${id}`)
  return res.data
}

export async function createUser(user: { name: string; email: string }): Promise<User> {
  const res = await api.post('/users', user)
  return res.data
}

export async function updateUser(id: number, user: { name: string; email: string }): Promise<void> {
  await api.put(`/users/${id}`, user)
}

export async function deleteUser(id: number): Promise<void> {
  await api.delete(`/users/${id}`)
}

export async function uploadAvatar(id: number, file: File): Promise<string> {
  const formData = new FormData()
  formData.append('file', file)
  const res = await api.post(`/users/${id}/avatar`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return res.data.avatarUrl
}

export async function deleteAvatar(id: number): Promise<void> {
  await api.delete(`/users/${id}/avatar`)
}
