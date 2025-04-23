# ✋🤚✌️ C# Mediapipe Rock-Paper-Scissors Game

這是一個跨語言的視覺互動專案，結合 C# Windows Form 與 Python Mediapipe，實作出一款使用手勢辨識進行連線猜拳的遊戲 🎮。

## 🔧 專案架構

- `C# (WinForms)`: 提供 GUI 操作介面與遊戲邏輯控制。
- `Python (Mediapipe)`: 負責攝影機影像讀取與手勢辨識，並透過 TCP 傳送資料至 C#。
- `Socket(TCP/UDP)`: 建立 C# 與 Python 之間的通訊橋樑，實現即時互動。
