import cv2
import socket
import time

# Configura el socket UDP para enviar datos a Unity
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_address = ('localhost', 5005)

# Configura la cámara
cap = cv2.VideoCapture(0)
_, frame = cap.read()
height, width, _ = frame.shape
center_x = width // 2
center_y = height // 2

while True:
    ret, frame = cap.read()
    if not ret:
        break

    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    blur = cv2.GaussianBlur(gray, (5, 5), 0)
    _, thresh = cv2.threshold(blur, 60, 255, cv2.THRESH_BINARY_INV)

    contours, _ = cv2.findContours(thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    movement = ""
    if contours:
        max_contour = max(contours, key=cv2.contourArea)
        x, y, w, h = cv2.boundingRect(max_contour)

        if x < center_x - 50:
            movement = "left"
        elif x > center_x + 50:
            movement = "right"
        elif y < center_y - 50:
            movement = "forward"
        elif y > center_y + 50:
            movement = "backward"

        if movement:
            sock.sendto(movement.encode(), unity_address)
            print(f"Enviando: {movement}")

    else:
        movement = "stop"
        sock.sendto(movement.encode(), unity_address)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
