using UnityEngine;
using UnityEngine.SceneManagement;

public class meta : MonoBehaviour
{
    // Si esta activo, el jugador debe recoger todas las monedas antes de poder terminar el nivel
    [SerializeField] private bool requiereTodasLasMonedas = true;

    // Nombre de la escena a cargar al ganar. Si se deja vacio, se reinicia la escena actual
    [SerializeField] private string escenaSiguiente = "";

    private contadorMonedas contador;

    private void Start()
    {
        // Se busca una sola vez al inicio para no repetir la busqueda en cada colision
        contador = Object.FindObjectOfType<contadorMonedas>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        // La meta se bloquea mientras falten monedas por recoger
        if (requiereTodasLasMonedas && contador != null && !contador.TodasRecogidas())
        {
            Debug.Log("Aun faltan monedas por recoger.");
            return;
        }

        GanarNivel();
    }

    private void GanarNivel()
    {
        Debug.Log("Nivel completado!");

        if (string.IsNullOrEmpty(escenaSiguiente))
        {
            // Sin escena siguiente configurada, se reinicia el nivel actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(escenaSiguiente);
        }
    }
}
