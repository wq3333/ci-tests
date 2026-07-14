import type { CapacitorConfig } from '@capacitor/cli'

const config: CapacitorConfig = {
  appId: 'com.fullstack.demo',
  appName: 'Fullstack Demo',
  webDir: '../web/dist',
  server: {
    url: 'http://192.168.10.36:5000',
    cleartext: true,
  },
}

export default config
