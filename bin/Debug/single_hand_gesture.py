import cv2
import mediapipe as mp
import numpy as np
import time
import socket
import sys

class HandGestureRecognizer:
    def __init__(self):
        self.mp_hands = mp.solutions.hands
        self.mp_draw = mp.solutions.drawing_utils
        self.hands = self.mp_hands.Hands(
            static_image_mode=False,
            max_num_hands=1,
            model_complexity=1,
            min_detection_confidence=0.8,
            min_tracking_confidence=0.8
        )
    
    def recognize_gesture(self, frame):
        frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = self.hands.process(frame_rgb)
        
        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                self.mp_draw.draw_landmarks(
                    frame, 
                    hand_landmarks, 
                    self.mp_hands.HAND_CONNECTIONS
                )
            
            landmarks = results.multi_hand_landmarks[0]
            fingers = self.get_finger_states(landmarks)
            
            if self.is_rock(fingers, landmarks):
                return "Rock"
            elif self.is_paper(fingers, landmarks):
                return "Paper"
            elif self.is_scissors(fingers, landmarks):
                return "Scissors"
        
        return "Unknown"
    
    def is_rock(self, fingers, landmarks):
        return sum(fingers) <= 1
    
    def is_paper(self, fingers, landmarks):
        return sum(fingers) >= 4
        
    def is_scissors(self, fingers, landmarks):
        return sum(fingers) >= 2
    
    def get_finger_states(self, landmarks):
        fingers = []
        
        # 拇指
        thumb_tip = landmarks.landmark[4]
        thumb_ip = landmarks.landmark[3]
        if (thumb_tip.x - thumb_ip.x) > 0.02:
            fingers.append(1)
        else:
            fingers.append(0)
        
        # 其他手指
        tips = [8, 12, 16, 20]
        pips = [6, 10, 14, 18]
        for tip, pip in zip(tips, pips):
            if landmarks.landmark[tip].y < landmarks.landmark[pip].y:
                fingers.append(1)
            else:
                fingers.append(0)
                
        return fingers


def main(ip, port):
    try:
        # 初始化 Socket 服務器
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.bind((ip, port))
        server.listen(1)
        print("等待 C# 客戶端連接...")

        # 初始化攝像頭
        cap = cv2.VideoCapture(0, cv2.CAP_DSHOW)
        cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
        cap.set(cv2.CAP_PROP_FPS, 30)
        
        time.sleep(2)
        
        if not cap.isOpened():
            print("錯誤：無法開啟攝像頭")
            return
            
        print("攝像頭初始化成功！")
        recognizer = HandGestureRecognizer()
        
        # 等待客戶端連接
        conn, addr = server.accept()
        print(f"C# 客戶端已連接：{addr}")
        
        score_text = "Score: Player 0 - Computer 0"  # 初始比分

        while True:
            ret, frame = cap.read()
            if not ret:
                print("錯誤：無法讀取影像")
                break
                
            frame = cv2.flip(frame, 1)
            gesture = recognizer.recognize_gesture(frame)
            
            # 發送手勢到 C# 客戶端
            try:
                conn.send(gesture.encode('utf-8'))
                
                # 接收 C# 傳來的比分
                conn.settimeout(0.1)  # 設定超時
                try:
                    score_data = conn.recv(1024).decode('utf-8').strip()
                    if score_data.startswith("Score:"):
                        score_text = score_data
                except socket.timeout:
                    pass  # 超時則不處理，繼續讀取影像
                
            except:
                print("客戶端連接已斷開")
                break
            
            # 顯示手勢與比分
            cv2.putText(frame, f"Gesture: {gesture}", (10, 30),
                        cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
            cv2.putText(frame, score_text, (10, 70),
                        cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
            cv2.putText(frame, "Press 'q' to quit", (10, 110),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 1)
            
            cv2.imshow("Hand Gesture Recognition", frame)
            
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
    
    except Exception as e:
        print(f"發生錯誤：{str(e)}")
    
    finally:
        print("正在清理資源...")
        if 'cap' in locals():
            cap.release()
        if 'conn' in locals():
            conn.close()
        if 'server' in locals():
            server.close()
        cv2.destroyAllWindows()
        print("程式已正常關閉")


if __name__ == "__main__":
    # main()
    ip_address = sys.argv[1]
    port = int(sys.argv[2])

    # 调用主函数
    main(ip_address, port)
