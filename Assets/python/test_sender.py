import socket
import time

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_address = ('localhost', 5005)

while True:
    message = "test"
    sock.sendto(message.encode(), unity_address)
    print(f"Sending: {message}")
    time.sleep(1)  # Envía cada segundo
