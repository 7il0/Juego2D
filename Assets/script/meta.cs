using UnityEngine;

/// <summary>
/// Meta del nivel. Solo se gana llegando aqui con todas las monedas recogidas.
/// </summary>
public class meta : MonoBehaviour
{
    private contadorMonedas contador;
    private bool nivelGanado;

    private void Start()
    {
        // Se busca una sola vez al inicio para no repetir la busqueda en cada colision
        contador = Object.FindObjectOfType<contadorMonedas>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // nivelGanado evita que el panel se cree dos veces si el trigger vuelve a dispararse
        if (nivelGanado || !collision.CompareTag("Player"))
        {
            return;
        }

        // Sin contador no hay forma de verificar las monedas, asi que no se permite ganar
        if (contador == null || !contador.TodasRecogidas())
        {
            Debug.Log("Aun faltan monedas por recoger.");
            return;
        }

        nivelGanado = true;
        panelVictoria.Mostrar();
    }
}
