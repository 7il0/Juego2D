using TMPro;
using UnityEngine;

/// Muestra en pantalla las vidas actuales, leyendo el valor desde GameManager.
public class vidasUI : MonoBehaviour
{
    [SerializeField] private TMP_Text vidasText;

    private void Start()
    {
        // Se pinta el valor actual de una vez, por si el objeto se crea despues del primer evento
        ActualizarTexto(GameManager.Instance.VidasActuales);
        GameManager.Instance.VidasCambiaron += ActualizarTexto;
    }

    private void OnDestroy()
    {
        // Evita que el GameManager (que persiste) intente notificar a una UI ya destruida
        if (GameManager.Instance != null)
        {
            GameManager.Instance.VidasCambiaron -= ActualizarTexto;
        }
    }

    private void ActualizarTexto(int vidas)
    {
        vidasText.text = "Vidas: " + vidas;
    }
}
