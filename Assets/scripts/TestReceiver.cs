using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TestReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    void Start()
    {
        udpClient = new UdpClient(5005);  // Configura el puerto 5005
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Debug.Log("Listening for UDP data on port 5005...");
    }

    void Update()
    {
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);  // Recibe datos
            string receivedMessage = Encoding.ASCII.GetString(data);  // Convierte los datos a texto
            Debug.Log("Received: " + receivedMessage);  // Muestra el mensaje recibido en la consola de Unity
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close();  // Cierra el cliente al salir
    }
}
