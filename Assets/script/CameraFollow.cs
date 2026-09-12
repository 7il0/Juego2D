using UnityEngine;

/// <summary>
/// Hace que la camara siga al jugador con un suavizado y, opcionalmente,
/// sin salir de los limites del nivel.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform objetivo;

    // Mayor a 0. Mas bajo = camara mas rapida y pegada al jugador. Mas alto = mas suave/perezosa
    [SerializeField] private float tiempoSuavizado = 0.15f;

    // Desplazamiento respecto al jugador (util para ver un poco mas adelante o arriba)
    [SerializeField] private Vector2 offset = Vector2.zero;

    // Si el nivel ya tiene un tamano definido, se puede evitar que la camara muestre fuera del escenario
    [SerializeField] private bool usarLimites = false;
    [SerializeField] private float limiteIzquierdo;
    [SerializeField] private float limiteDerecho;
    [SerializeField] private float limiteInferior;
    [SerializeField] private float limiteSuperior;

    private Vector3 velocidadActual;
    private Camera camara;

    private void Start()
    {
        camara = GetComponent<Camera>();

        // Si no se asigno manualmente, se busca por tag para no depender del Inspector
        if (objetivo == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                objetivo = jugador.transform;
            }
        }
    }

    // LateUpdate para mover la camara despues de que el jugador ya se movio en Update/FixedUpdate
    private void LateUpdate()
    {
        if (objetivo == null)
        {
            return;
        }

        Vector3 posicionDeseada = new Vector3(
            objetivo.position.x + offset.x,
            objetivo.position.y + offset.y,
            transform.position.z);

        if (usarLimites)
        {
            posicionDeseada = ClampAlNivel(posicionDeseada);
        }

        // SmoothDamp evita el efecto tembloroso de un Lerp simple y da una sensacion mas cinematografica
        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidadActual, tiempoSuavizado);
    }

    private Vector3 ClampAlNivel(Vector3 posicion)
    {
        float mitadAltoCamara = camara.orthographicSize;
        float mitadAnchoCamara = mitadAltoCamara * camara.aspect;

        // Si el nivel es mas angosto que la pantalla, se centra en X en vez de dejarla temblando entre limites invertidos
        if (limiteDerecho - limiteIzquierdo > mitadAnchoCamara * 2f)
        {
            posicion.x = Mathf.Clamp(posicion.x, limiteIzquierdo + mitadAnchoCamara, limiteDerecho - mitadAnchoCamara);
        }
        else
        {
            posicion.x = (limiteIzquierdo + limiteDerecho) / 2f;
        }

        if (limiteSuperior - limiteInferior > mitadAltoCamara * 2f)
        {
            posicion.y = Mathf.Clamp(posicion.y, limiteInferior + mitadAltoCamara, limiteSuperior - mitadAltoCamara);
        }
        else
        {
            posicion.y = (limiteInferior + limiteSuperior) / 2f;
        }

        return posicion;
    }
}
