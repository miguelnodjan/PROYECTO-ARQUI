using System.Collections;
using UnityEngine;

public class MovimientoAleatorio : MonoBehaviour
{
    public float velocidad = 3f; // Velocidad de movimiento
    public float cambioDireccionTiempo = 2f; // Tiempo entre cambios de dirección
    private Vector3 direccionMovimiento; // Dirección actual de movimiento
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        CambiarDireccion(); // Define una dirección inicial
        InvokeRepeating("CambiarDireccion", 0, cambioDireccionTiempo); // Cambia la dirección cada cierto tiempo
    }

    void Update()
    {
        // Movimiento del personaje
        transform.Translate(direccionMovimiento * velocidad * Time.deltaTime, Space.World);

        // Rotación hacia la dirección de movimiento
        if (direccionMovimiento != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
        }

        // Actualizar animación
        if (direccionMovimiento.magnitude > 0.1f)
        {
            anim.SetBool("estaCaminando", true);
        }
        else
        {
            anim.SetBool("estaCaminando", false);
        }
    }

    void CambiarDireccion()
    {
        // Genera una nueva dirección aleatoria
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);
        direccionMovimiento = new Vector3(randomX, 0, randomZ).normalized;
    }
}
