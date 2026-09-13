using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla las vidas y el punto de reaparicion (checkpoint) del jugador.
/// Persiste entre recargas de escena (DontDestroyOnLoad) para que las vidas
/// sobrevivan a un Game Over y el checkpoint sobreviva mientras no se reinicia la partida.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int vidasIniciales = 10;

    // Evita que recoger corazones de mas infle las vidas sin limite
    [SerializeField] private int vidasMaximas = 10;

    [SerializeField] private AudioClip musicaFondo;
    [SerializeField] [Range(0f, 1f)] private float volumenMusica = 0.4f;

    public int VidasActuales { get; private set; }

    // La UI se suscribe a este evento en vez de que este script conozca a la UI directamente
    public event Action<int> VidasCambiaron;

    // Numero de la ultima bandera pisada (0 = ninguna todavia). Solo sube, nunca baja al pisar una anterior
    private int checkpointNumero = 0;
    private Vector3? puntoReaparicion = null;

    private void Awake()
    {
        // Patron singleton: al recargar la escena (Game Over), la version persistida ya existe,
        // asi que la copia que trae la escena nueva sobra y se descarta
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        VidasActuales = vidasIniciales;

        IniciarMusicaDeFondo();
    }

    private void IniciarMusicaDeFondo()
    {
        if (musicaFondo == null)
        {
            return;
        }

        AudioSource audioMusica = gameObject.AddComponent<AudioSource>();
        audioMusica.clip = musicaFondo;
        audioMusica.volume = volumenMusica;
        audioMusica.loop = true;
        audioMusica.playOnAwake = false;
        audioMusica.Play();
    }

    /// <summary>
    /// Llamado por el jugador al iniciar, para tener un punto de reaparicion
    /// aunque todavia no haya pisado ninguna bandera.
    /// </summary>
    public void RegistrarSpawnInicial(Vector3 posicion)
    {
        if (puntoReaparicion == null)
        {
            puntoReaparicion = posicion;
        }
    }

    /// <summary>
    /// Llamado por una bandera al ser pisada. Solo actualiza el checkpoint si es
    /// una bandera mas avanzada que la ultima registrada.
    /// </summary>
    public void ActualizarCheckpoint(Vector3 posicion, int numero)
    {
        if (numero <= checkpointNumero)
        {
            return;
        }

        checkpointNumero = numero;
        puntoReaparicion = posicion;
    }

    /// <summary>
    /// Descuenta una vida. Si quedan vidas, el jugador reaparece en el ultimo
    /// checkpoint sin recargar la escena (las monedas ya recogidas no se pierden).
    /// Si las vidas llegan a cero, se muestra el panel de Game Over.
    /// </summary>
    public void Morir()
    {
        VidasActuales--;
        VidasCambiaron?.Invoke(VidasActuales);

        if (VidasActuales <= 0)
        {
            panelGameOver.Mostrar();
            return;
        }

        ReaparecerEnCheckpoint();
    }

    private void ReaparecerEnCheckpoint()
    {
        if (puntoReaparicion == null)
        {
            return;
        }

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null)
        {
            return;
        }

        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Se limpia la velocidad para que no conserve el impulso que traia al morir
            rb.linearVelocity = Vector2.zero;
            rb.position = puntoReaparicion.Value;
        }
        else
        {
            jugador.transform.position = puntoReaparicion.Value;
        }
    }

    /// <summary>
    /// Suma una vida al recoger un corazon, sin pasar del maximo permitido.
    /// </summary>
    public void GanarVida()
    {
        VidasActuales = Mathf.Min(VidasActuales + 1, vidasMaximas);
        VidasCambiaron?.Invoke(VidasActuales);
    }

    /// <summary>
    /// Reinicio completo de la partida: vidas, checkpoint y escena (monedas incluidas)
    /// vuelven a su estado inicial. Se usa desde el boton del panel de Game Over.
    /// </summary>
    public void ReiniciarPartida()
    {
        VidasActuales = vidasIniciales;
        checkpointNumero = 0;
        puntoReaparicion = null;
        VidasCambiaron?.Invoke(VidasActuales);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
