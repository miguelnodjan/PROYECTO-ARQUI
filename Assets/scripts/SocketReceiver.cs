using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketReceiver : MonoBehaviour
{
    public LogicaPersonaje1 player;
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    private string movementCommand = "stop";  // Movimiento
    private string actionCommand = "stop";    // Acción

    private bool isJumping = false;  // Para controlar el salto

    void Start()
    {
        udpClient = new UdpClient(5005);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Debug.Log("Listening for UDP data on port 5005...");
    }

    void Update()
    {
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string command = Encoding.ASCII.GetString(data);
            ProcessCommand(command);
        }
    }

    private void ProcessCommand(string command)
    {
        Debug.Log("Received: " + command);
        if (player == null)
        {
            Debug.LogError("Player not assigned!");
            return;
        }

        // Controlar el movimiento
        if (command == "left" || command == "right" || command == "stop_rotation")
        {
            movementCommand = command;
        }

        // Controlar las acciones
        if (command == "forward" || command == "jump" || command == "stop" || command == "backward")
        {
            actionCommand = command;
        }

        // Aplicar los comandos
        ApplyMovement();
        ApplyAction();
    }

    private void ApplyMovement()
    {
        switch (movementCommand)
        {
            case "left":
                player.x = -1;
                player.y = 0;
                break;
            case "right":
                player.x = 1;
                player.y = 0;
                break;
            case "stop_rotation":
                player.x = 0;
                break;
            default:
                player.x = 0;
                player.y = 0;
                break;
        }
    }

    private void ApplyAction()
    {
        switch (actionCommand)
        {
            case "forward":
                player.y = 1;  // Mover hacia adelante
                break;
            case "jump":
                // Asegurarse de que el personaje salte si no está ya saltando
                if (!isJumping)
                {
                    isJumping = true;
                    player.Saltar();  // Llamada al método de salto de LogicaPersonaje1
                    Debug.Log("Comando de salto recibido");
                }
                break;
            case "stop":
                player.y = 0;  // Detener movimiento
                break;
            case "backward":
                player.y = -1; // Retroceder
                break;
            default:
                break;
        }
    }

    // Método para controlar el final del salto (cuando el personaje toca el suelo)
    public void OnLand()
    {
        isJumping = false;
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
