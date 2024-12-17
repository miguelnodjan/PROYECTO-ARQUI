using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuInicial : MonoBehaviour
{
    public RawImage pantallaInicial;
    public RawImage pantallaTutorial;
    public Image panelOscuro;
    public GameObject juego;
    public float duracionFade = 1.5f;

    public AudioSource musicaInicial; // Música para las pantallas iniciales
    public AudioSource musicaJuego; // Música para el juego

    private bool enPantallaInicial = true;
    private bool enPantallaTutorial = false;

    void Start()
    {
        pantallaInicial.gameObject.SetActive(true);
        pantallaTutorial.gameObject.SetActive(false);
        juego.SetActive(false);
        panelOscuro.gameObject.SetActive(true);

        StartCoroutine(FadeIn(panelOscuro));

        // Reproducir la música inicial con Fade In
        StartCoroutine(FadeInAudio(musicaInicial, 0.2f));
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (enPantallaInicial)
            {
                StartCoroutine(CambiarPantalla(pantallaInicial, pantallaTutorial));
                enPantallaInicial = false;
                enPantallaTutorial = true;
            }
            else if (enPantallaTutorial)
            {
                StartCoroutine(CambiarPantalla(pantallaTutorial, null, true));
                enPantallaTutorial = false;
            }
        }
    }

    IEnumerator CambiarPantalla(RawImage pantallaActual, RawImage siguientePantalla, bool iniciarJuego = false)
    {
        yield return StartCoroutine(FadeOut(panelOscuro));

        if (pantallaActual != null)
        {
            pantallaActual.gameObject.SetActive(false);
        }

        if (siguientePantalla != null)
        {
            siguientePantalla.gameObject.SetActive(true);
            yield return StartCoroutine(FadeIn(panelOscuro));
        }

        if (iniciarJuego)
        {
            yield return StartCoroutine(FadeOut(panelOscuro));
            ActivarJuego();
        }
    }

    IEnumerator FadeIn(Image panel)
    {
        Color color = panel.color;
        float tiempo = duracionFade;

        while (tiempo > 0)
        {
            tiempo -= Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, (duracionFade - tiempo) / duracionFade);
            panel.color = color;
            yield return null;
        }

        panel.gameObject.SetActive(false);
    }

    IEnumerator FadeOut(Image panel)
    {
        panel.gameObject.SetActive(true);
        Color color = panel.color;
        float tiempo = 0;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, tiempo / duracionFade);
            panel.color = color;
            yield return null;
        }
    }

    // Función para activar el juego
    void ActivarJuego()
    {
        
        panelOscuro.gameObject.SetActive(false);
        juego.SetActive(true);

        // Cambiar música con Fade
        StartCoroutine(FadeOutAudio(musicaInicial));
        StartCoroutine(FadeInAudio(musicaJuego, 0.2f));
    }

    // **Funciones de Fade para Audio**
    IEnumerator FadeOutAudio(AudioSource audioSource)
    {
        float initialVolume = audioSource.volume;
        float tiempo = 1f;

        while (tiempo > 0)
        {
            tiempo -= Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, initialVolume, tiempo);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = initialVolume;
    }

    IEnumerator FadeInAudio(AudioSource audioSource, float maxVolume)
    {
        audioSource.volume = 0;
        audioSource.Play();

        float tiempo = 0;
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, maxVolume, tiempo);
            yield return null;
        }
    }
}
