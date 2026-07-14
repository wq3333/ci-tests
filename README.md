使用Photino.NET和capacitor来实现服务端，客户端全栈开发，并利用git actions自动打包

1.linux需要安装依赖
sudo apt update
#sudo apt install libnotify4 libx11-dev libxrandr-dev libxcomposite-dev libxcursor-dev libxinerama-dev libxi-dev libglu1-mesa-dev
sudo apt install -y libwebkit2gtk-4.1-0
#sudo apt install -y libgtk-3-0 libx11-6 libxft2 libxinerama1 libglu1-mesa libgdiplus libxrender1 libfontconfig1

2.wsl中乱码
sudo apt update
sudo apt install fonts-wqy-microhei fonts-wqy-zenhei
sudo fc-cache -fv
wsl --shutdown