# import cv2
# import mediapipe as mp
# import socket
# import time

# class HandGestureRecognizer:
#     def __init__(self):
#         self.mp_hands = mp.solutions.hands
#         self.mp_draw = mp.solutions.drawing_utils
#         self.hands = self.mp_hands.Hands(
#             static_image_mode=False,
#             max_num_hands=1,
#             model_complexity=1,
#             min_detection_confidence=0.8,
#             min_tracking_confidence=0.8
#         )
    
#     def recognize_gesture(self, frame):
#         frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
#         results = self.hands.process(frame_rgb)
        
#         if results.multi_hand_landmarks:
#             for hand_landmarks in results.multi_hand_landmarks:
#                 self.mp_draw.draw_landmarks(
#                     frame, 
#                     hand_landmarks, 
#                     self.mp_hands.HAND_CONNECTIONS
#                 )
            
#             landmarks = results.multi_hand_landmarks[0]
#             fingers = self.get_finger_states(landmarks)
            
#             if self.is_rock(fingers, landmarks):
#                 return "Rock"
#             elif self.is_paper(fingers, landmarks):
#                 return "Paper"
#             elif self.is_scissors(fingers, landmarks):
#                 return "Scissors"
        
#         return "Unknown"
    
#     def is_rock(self, fingers, landmarks):
#         return sum(fingers) <= 1
    
#     def is_paper(self, fingers, landmarks):
#         return sum(fingers) >= 4
    
#     def is_scissors(self, fingers, landmarks):
#         # 這邊你可再加強判斷，
#         # 簡單示範：只要超過等於2隻手指伸直，就判定為剪刀
#         return sum(fingers) == 2
    
#     def get_finger_states(self, landmarks):
#         fingers = []
        
#         # 拇指
#         thumb_tip = landmarks.landmark[4]
#         thumb_ip = landmarks.landmark[3]
#         if (thumb_tip.x - thumb_ip.x) > 0.02:
#             fingers.append(1)
#         else:
#             fingers.append(0)
            
#         # 其他手指
#         tips = [8, 12, 16, 20]
#         pips = [6, 10, 14, 18]
#         for tip, pip in zip(tips, pips):
#             if landmarks.landmark[tip].y < landmarks.landmark[pip].y:
#                 fingers.append(1)
#             else:
#                 fingers.append(0)
                
#         return fingers

# def main(server_ip="127.0.0.1", server_port=5000):
#     # 與 C# 主遊戲伺服器建立連線
#     client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#     client_socket.connect((server_ip, server_port))
    
#     # 初始化攝像頭
#     cap = cv2.VideoCapture(0)
#     cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
#     cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
#     cap.set(cv2.CAP_PROP_FPS, 30)
    
#     recognizer = HandGestureRecognizer()

#     try:
#         while True:
#             ret, frame = cap.read()
#             if not ret:
#                 print("錯誤：無法讀取影像")
#                 break
            
#             frame = cv2.flip(frame, 1)
#             gesture = recognizer.recognize_gesture(frame)

#             # 將手勢透過 Socket 傳給伺服器
#             try:
#                 client_socket.send(gesture.encode('utf-8'))
#             except:
#                 print("無法傳送手勢到伺服器，可能已斷線")
#                 break

#             cv2.putText(frame, f"Gesture: {gesture}", (10, 30),
#                         cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
#             cv2.imshow("Hand Gesture Recognition", frame)

#             if cv2.waitKey(1) & 0xFF == ord('q'):
#                 break
        
#     except Exception as e:
#         print(f"發生錯誤：{e}")
#     finally:
#         cap.release()
#         client_socket.close()
#         cv2.destroyAllWindows()

# if __name__ == "__main__":
#     # 若你的 C# 主機在同一台電腦上，可用預設 127.0.0.1
#     # 若在不同電腦，請改成 C# 主機在區域網路或公網的 IP
#     main("127.0.0.1", 5000)

import cv2
import mediapipe as mp
import socket
import threading

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
        return sum(fingers) == 2
    
    def get_finger_states(self, landmarks):
        fingers = []
        thumb_tip = landmarks.landmark[4]
        thumb_ip = landmarks.landmark[3]
        if (thumb_tip.x - thumb_ip.x) > 0.02:
            fingers.append(1)
        else:
            fingers.append(0)
        
        tips = [8, 12, 16, 20]
        pips = [6, 10, 14, 18]
        for tip, pip in zip(tips, pips):
            if landmarks.landmark[tip].y < landmarks.landmark[pip].y:
                fingers.append(1)
            else:
                fingers.append(0)
                
        return fingers

def handle_client(conn, addr):
    print(f"客戶端 {addr} 已連線")
    try:
        while True:
            data = conn.recv(1024).decode('utf-8')
            if not data:
                break
            print(f"收到來自 {addr} 的數據：{data}")
    except Exception as e:
        print(f"客戶端 {addr} 連線發生錯誤：{e}")
    finally:
        conn.close()
        print(f"客戶端 {addr} 已斷線")

def start_server(host="127.0.0.1", port=5000):
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((host, port))
    server_socket.listen(5)
    print(f"伺服器啟動，監聽中 {host}:{port}...")

    while True:
        conn, addr = server_socket.accept()
        threading.Thread(target=handle_client, args=(conn, addr)).start()

def main():
    threading.Thread(target=start_server).start()

    cap = cv2.VideoCapture(0)
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
    cap.set(cv2.CAP_PROP_FPS, 30)
    
    recognizer = HandGestureRecognizer()

    try:
        while True:
            ret, frame = cap.read()
            if not ret:
                print("錯誤：無法讀取影像")
                break
            
            frame = cv2.flip(frame, 1)
            gesture = recognizer.recognize_gesture(frame)

            cv2.putText(frame, f"Gesture: {gesture}", (10, 30),
                        cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
            cv2.imshow("Hand Gesture Recognition", frame)

            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
    except Exception as e:
        print(f"發生錯誤：{e}")
    finally:
        cap.release()
        cv2.destroyAllWindows()

if __name__ == "__main__":
    main()
