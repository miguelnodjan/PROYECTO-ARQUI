import cv2
import mediapipe as mp
import socket
import speech_recognition as sr
import threading

# Configura MediaPipe
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(min_detection_confidence=0.7, min_tracking_confidence=0.7)
mp_drawing = mp.solutions.drawing_utils

# Configura la cámara
cap = cv2.VideoCapture(0)

# Configura el socket para enviar datos a Unity
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_address = ('localhost', 5005)

# Función para reconocer comandos de voz en español
def recognize_speech():
    recognizer = sr.Recognizer()
    with sr.Microphone() as source:
        print("Escuchando... (di algo en español)")
        audio = recognizer.listen(source)
        
        try:
            # Reconocimiento de voz en español
            command = recognizer.recognize_google(audio, language="es-ES")  # Aquí cambiamos el idioma
            print(f"Comando reconocido: {command}")
            return command.lower()  # Convertimos el comando a minúsculas para facilitar la comparación
        except sr.UnknownValueError:
            print("No se pudo entender el audio.")
        except sr.RequestError:
            print("No se pudo obtener resultados de Google Speech Recognition.")
        return None

# Función para escuchar en un hilo separado (para reconocimiento de voz en paralelo)
def listen_for_voice_commands():
    while True:
        command = recognize_speech()
        if command:
            if "adelante" in command or "avanzar" in command:
                action = "forward"
            elif "salta" in command or "saltar" in command:
                action = "jump"
            elif "detente" in command or "detenerse" in command:
                action = "stop"
            elif "retrocede" in command or "volver atrás" in command:
                action = "backward"
            else:
                action = "stop"  # Por si no se reconoce el comando
            # Enviar el comando de voz a Unity
            sock.sendto(action.encode(), unity_address)
            print(f"Comando de voz enviado: {action}")

# Iniciar el hilo para escuchar comandos de voz
voice_thread = threading.Thread(target=listen_for_voice_commands, daemon=True)
voice_thread.start()

while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        break

    # Convierte la imagen a RGB para MediaPipe
    frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = hands.process(frame_rgb)

    movement = "stop"
    action = "stop"

    if results.multi_hand_landmarks:
        for i, hand_landmarks in enumerate(results.multi_hand_landmarks):
            # Coordenadas relevantes para controlar rotación y movimiento
            thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP]
            thumb_mcp = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_MCP]
            wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST]

            # Condición para detectar pulgar hacia arriba para rotación
            if thumb_tip.y < thumb_mcp.y and abs(thumb_tip.x - wrist.x) < 0.1:
                if i == 0:  # Primera mano controla rotación
                    movement = "stop_rotation"
            else:
                # Control de izquierda y derecha (movimiento de rotación)
                x_thumb = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].x
                x_index = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].x

                if i == 0:  # Primera mano
                    if x_thumb < x_index - 0.1:
                        movement = "left"
                    elif x_thumb > x_index + 0.1:
                        movement = "right"
                    else:
                        movement = "stop_rotation"

            # Control de acción con la segunda mano (avanzar, retroceder, detener, saltar)
            if i == 1:  # Segunda mano
                z_wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].z
                # Detectar si el puño está cerrado
                fingers_closed = True
                for finger_tip, finger_mcp in [
                    (mp_hands.HandLandmark.THUMB_TIP, mp_hands.HandLandmark.THUMB_MCP),
                    (mp_hands.HandLandmark.INDEX_FINGER_TIP, mp_hands.HandLandmark.INDEX_FINGER_MCP),
                    (mp_hands.HandLandmark.MIDDLE_FINGER_TIP, mp_hands.HandLandmark.MIDDLE_FINGER_MCP),
                    (mp_hands.HandLandmark.RING_FINGER_TIP, mp_hands.HandLandmark.RING_FINGER_MCP),
                    (mp_hands.HandLandmark.PINKY_TIP, mp_hands.HandLandmark.PINKY_MCP),
                ]:
                    if hand_landmarks.landmark[finger_tip].y < hand_landmarks.landmark[finger_mcp].y:
                        fingers_closed = False
                        break

                if fingers_closed:
                    action = "jump"
                elif z_wrist < -0.1:  # Mano cerca de la cámara
                    action = "walk"
                elif z_wrist > 0.1:  # Mano lejos de la cámara
                    action = "stop"

            # Dibujar los landmarks
            mp_drawing.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)

    # Enviar comandos a Unity
    if movement != "stop":
        sock.sendto(movement.encode(), unity_address)
        print(f"Movement: {movement}")
    if action != "stop":
        sock.sendto(action.encode(), unity_address)
        print(f"Action: {action}")

    # Mostrar la cámara
    cv2.imshow('MediaPipe Hand Detection', frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
