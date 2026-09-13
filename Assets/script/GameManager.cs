using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla las vidas del jugador. Persiste entre recargas de escena (DontDestroyOnLoad)
/// para que el conteo de vidas sobreviva a un reinicio por muerte.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int vidasIniciales = 10;

    // Evita que recoger corazones de mas infle las vidas sin limite
    [SerializeField] private int vidasMaximas = 10;

    public int VidasActuales { get; private set; }

    // La UI se suscribe a este evento en vez de que este script conozca a la UI directamente
    public event Action<int> VidasCambiaron;

    private void Awake()
    {
        // Patron singleton: al recargar la escena, la version persistida ya existe,
        // asi que la copia que trae la escena nueva sobra y se descarta
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        VidasActuales = vidasIniciales;
    }

    /// <summary>
    /// Descuenta una vida y recarga el nivel. Si las vidas se agotan, la partida
    /// se reinicia desde el conteo inicial.
    /// </summary>
    public void PerderVidaYReiniciar()
    {
        VidasActuales--;
        VidasCambiaron?.Invoke(VidasActuales);

        if (VidasActuales <= 0)
        {
            // Sin vidas se muestra el panel de Game Over en vez de recargar de inmediato;
            // el reinicio real ocurre cuando el jugador presiona el boton del panel
            panelGameOver.Mostrar();
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
    /// Restaura las vidas iniciales y recarga el nivel. Se usa desde el boton del panel de Game Over.
    /// </summary>
    public void ReiniciarPartida()
    {
        VidasActuales = vidasIniciales;
        VidasCambiaron?.Invoke(VidasActuales);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
