using UnityEngine;

/// <summary>
/// Checkpoint intermedio. Cada bandera tiene un numero de progreso (1, 2, 3...)
/// configurado en el Inspector; solo se convierte en el punto de reaparicion si
/// es mas avanzada que la ultima bandera pisada.
/// </summary>
public class bandera : MonoBehaviour
{
    [SerializeField] private int numero = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.ActualizarCheckpoint(transform.position, numero);
        }
    }
}
