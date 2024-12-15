using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicaPersonaje1 : MonoBehaviour
{
    public float velocidadMovimiento = 5.0f;
    public float velocidadRotacion = 200.0f;
    private Animator anim;
    public float x = 0, y = 0;  // Ahora x y y serán controladas externamente.
    public Rigidbody rb;
    public float fuerzaDeSalto = 8f;
    public bool puedoSaltar;
    public bool rotating = false;

    void Start()
    {
        puedoSaltar = false;
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
{
    // Si 'x' es 0, no rotamos
    if (x != 0)
    {
        transform.Rotate(0, x * Time.deltaTime * velocidadRotacion, 0);
    }

    // Movimiento hacia adelante o atrás
    transform.Translate(0, 0, y * Time.deltaTime * velocidadMovimiento);
}


    void Update()
    {
        // Actualizar animaciones con base en los valores de x e y
        anim.SetFloat("VelX", x);
        anim.SetFloat("VelY", y);

        if (puedoSaltar)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("salte", true);
                rb.AddForce(new Vector3(0, fuerzaDeSalto, 0), ForceMode.Impulse);
            }
            anim.SetBool("tocoSuelo", true);
        }
        else
        {
            EstoyCayendo();
        }
        if (rotating)
        {
            // Aquí aplicamos la rotación sobre el eje Y (ajústalo según tu preferencia)
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
        }

        // Lógica del movimiento (esto ya lo tenías en tu script original)
        if (y != 0)
        {
            transform.Translate(Vector3.forward * y * Time.deltaTime);
        }
    }
    public void Saltar()
{
    if (puedoSaltar)
    {
        anim.SetBool("salte", true);
        rb.AddForce(new Vector3(0, fuerzaDeSalto, 0), ForceMode.Impulse);
    }
}

    public void EstoyCayendo()
    {
        anim.SetBool("tocoSuelo", false);
        anim.SetBool("salte", false);
    }
    
    public void Rotate(float direction)
    {
        transform.Rotate(Vector3.up * direction * velocidadRotacion * Time.deltaTime);
    }
}
