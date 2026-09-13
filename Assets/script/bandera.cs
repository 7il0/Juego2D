using UnityEngine;

/// Checkpoint intermedio. Cada bandera tiene un numero de progreso (1, 2, 3...)
/// configurado en el Inspector; solo se convierte en el punto de reaparicion si
/// es mas avanzada que la ultima bandera pisada. El sonido y el aviso en pantalla
/// solo ocurren la primera vez que esta bandera en particular se activa.
public class bandera : MonoBehaviour
{
    [SerializeField] private int numero = 1;
    [SerializeField] private AudioClip sonidoCheckpoint;
    [SerializeField] [Range(0f, 1f)] private float volumen = 1f;

    // Segundos que el aviso permanece visible antes de desvanecerse
    [SerializeField] private float duracionMensaje = 1.6f;

    private bool yaActivada = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaActivada || !collision.CompareTag("Player"))
        {
            return;
        }

        yaActivada = true;
        GameManager.Instance.ActualizarCheckpoint(transform.position, numero);

        if (sonidoCheckpoint != null)
        {
            AudioSource.PlayClipAtPoint(sonidoCheckpoint, transform.position, volumen);
        }

        panelCheckpoint.Mostrar(numero, duracionMensaje);
    }
}
