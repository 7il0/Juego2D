using UnityEngine;

public class moneda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Aquí puedes agregar la lógica para cuando el jugador recoja la moneda
            // Por ejemplo, aumentar el puntaje del jugador
            Debug.Log("Moneda recogida!");
            Destroy(gameObject); // Destruye la moneda después de ser recogida
        }
    }
}
