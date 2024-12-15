import cv2
import mediapipe as mp
import socket

# Configura MediaPipe
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(min_detection_confidence=0.7, min_tracking_confidence=0.7)
mp_drawing = mp.solutions.drawing_utils

# Configura la cámara
cap = cv2.VideoCapture(0)

# Configura el socket para enviar datos a Unity
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_address = ('localhost', 5005)

while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        break

    # Convierte la imagen a RGB para MediaPipe
    frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = hands.process(frame_rgb)

    movement = "stop"
    action = "stop"  # Variable para la segunda mano

    if results.multi_hand_landmarks:
        for i, hand_landmarks in enumerate(results.multi_hand_landmarks):
            # Obtenemos las coordenadas de los puntos clave para cada mano
            x_thumb = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].x
            x_index = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].x

            # Detectamos la primera mano (movimiento)
            if i == 0:
                if x_thumb < x_index - 0.1:
                    movement = "left"
                elif x_thumb > x_index + 0.1:
                    movement = "right"
            
            # Detectamos la segunda mano (acciones)
            if i == 1:
                if x_thumb < x_index - 0.1:
                    action = "forward"
                elif x_thumb > x_index + 0.1:
                    action = "jump"
                else:
                    action = "stop"

            # Dibujar los landmarks en la imagen
            mp_drawing.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)

    # Enviar movimiento y acción a Unity
    if movement != "stop":
        sock.sendto(movement.encode(), unity_address)
        print(f"Movement: {movement}")
    if action != "stop":
        sock.sendto(action.encode(), unity_address)
        print(f"Action: {action}")

    # Mostrar la cámara en tiempo real
    cv2.imshow('MediaPipe Hand Detection', frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
