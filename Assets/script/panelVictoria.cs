using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Construye por codigo el dialogo flotante de victoria.
/// Se genera en tiempo de ejecucion para no depender de referencias del Inspector.
/// </summary>
public class panelVictoria : MonoBehaviour
{
    private RectTransform tarjeta;
    private CanvasGroup grupo;

    /// <summary>
    /// Punto de entrada: crea el panel y congela el juego.
    /// </summary>
    public static void Mostrar()
    {
        GameObject raiz = new GameObject("PanelVictoria");
        raiz.AddComponent<panelVictoria>().Construir();
    }

    private void Construir()
    {
        CrearCanvas();
        CrearFondo();
        CrearTarjeta();
        AsegurarEventSystem();

        // Se congela el juego para que el jugador no siga moviendose detras del dialogo
        Time.timeScale = 0f;

        StartCoroutine(AnimarEntrada());
    }

    private void CrearCanvas()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // por encima del HUD de monedas

        CanvasScaler escalador = gameObject.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        grupo = gameObject.AddComponent<CanvasGroup>();
        grupo.alpha = 0f;
    }

    private void CrearFondo()
    {
        GameObject fondo = NuevoElemento("Fondo", transform);
        Image imagen = fondo.AddComponent<Image>();
        imagen.color = new Color(0f, 0f, 0f, 0.7f);

        RectTransform rect = fondo.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void CrearTarjeta()
    {
        GameObject panel = NuevoElemento("Tarjeta", transform);

        Image imagen = panel.AddComponent<Image>();
        imagen.sprite = SpriteRedondeado();
        imagen.type = Image.Type.Sliced;
        imagen.color = new Color(0.10f, 0.15f, 0.27f, 1f);

        tarjeta = panel.GetComponent<RectTransform>();
        tarjeta.anchorMin = new Vector2(0.5f, 0.5f);
        tarjeta.anchorMax = new Vector2(0.5f, 0.5f);
        tarjeta.pivot = new Vector2(0.5f, 0.5f);
        tarjeta.anchoredPosition = Vector2.zero;
        tarjeta.sizeDelta = new Vector2(820f, 460f);

        CrearFranjaSuperior(tarjeta);
        CrearTexto("Titulo", tarjeta, "¡FELICIDADES!", 96f,
            new Color(1f, 0.80f, 0.25f), new Vector2(0f, 105f), new Vector2(760f, 130f));
        CrearTexto("Mensaje", tarjeta, "¡Ganaste!", 52f,
            Color.white, new Vector2(0f, 10f), new Vector2(760f, 80f));
        CrearTexto("Detalle", tarjeta, "Recogiste todas las monedas y llegaste a la meta.", 28f,
            new Color(0.72f, 0.78f, 0.90f), new Vector2(0f, -55f), new Vector2(760f, 60f));
        CrearBoton(tarjeta);
    }

    private void CrearFranjaSuperior(RectTransform padre)
    {
        GameObject franja = NuevoElemento("Franja", padre);
        Image imagen = franja.AddComponent<Image>();
        imagen.sprite = SpriteRedondeado();
        imagen.type = Image.Type.Sliced;
        imagen.color = new Color(1f, 0.80f, 0.25f);

        RectTransform rect = franja.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(0f, -10f);
        rect.offsetMax = new Vector2(0f, 0f);
    }

    private void CrearTexto(string nombre, RectTransform padre, string contenido, float tamano,
        Color color, Vector2 posicion, Vector2 medidas)
    {
        GameObject texto = NuevoElemento(nombre, padre);

        TextMeshProUGUI tmp = texto.AddComponent<TextMeshProUGUI>();
        tmp.text = contenido;
        tmp.fontSize = tamano;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;

        RectTransform rect = texto.GetComponent<RectTransform>();
        rect.anchoredPosition = posicion;
        rect.sizeDelta = medidas;
    }

    private void CrearBoton(RectTransform padre)
    {
        GameObject boton = NuevoElemento("BotonReiniciar", padre);

        Image imagen = boton.AddComponent<Image>();
        imagen.sprite = SpriteRedondeado();
        imagen.type = Image.Type.Sliced;
        imagen.color = new Color(0.20f, 0.62f, 0.42f);

        RectTransform rect = boton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, -150f);
        rect.sizeDelta = new Vector2(340f, 84f);

        Button control = boton.AddComponent<Button>();
        control.targetGraphic = imagen;
        control.onClick.AddListener(Reiniciar);

        CrearTexto("Etiqueta", rect, "Jugar de nuevo", 34f,
            Color.white, Vector2.zero, new Vector2(340f, 84f));
    }

    private void Reiniciar()
    {
        // Se restaura el tiempo antes de recargar, si no la escena nueva queda congelada
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator AnimarEntrada()
    {
        const float duracion = 0.35f;
        float transcurrido = 0f;

        while (transcurrido < duracion)
        {
            // unscaledDeltaTime porque Time.timeScale esta en 0
            transcurrido += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(transcurrido / duracion);

            grupo.alpha = t;
            // Escala con un pequeno rebote para que el dialogo entre con presencia
            tarjeta.localScale = Vector3.one * Mathf.LerpUnclamped(0.75f, 1f, 1f - Mathf.Pow(1f - t, 3f));

            yield return null;
        }

        grupo.alpha = 1f;
        tarjeta.localScale = Vector3.one;
    }

    /// <summary>
    /// Crea un GameObject de UI ya emparentado y con RectTransform.
    /// </summary>
    private GameObject NuevoElemento(string nombre, Transform padre)
    {
        GameObject elemento = new GameObject(nombre, typeof(RectTransform));
        elemento.transform.SetParent(padre, false);
        return elemento;
    }

    /// <summary>
    /// Sprite de esquinas redondeadas que Unity trae incorporado.
    /// </summary>
    private Sprite SpriteRedondeado()
    {
        return Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
    }

    private void AsegurarEventSystem()
    {
        // Sin EventSystem el boton no responde a los clics
        if (EventSystem.current != null)
        {
            return;
        }

        GameObject sistema = new GameObject("EventSystem");
        sistema.AddComponent<EventSystem>();
        sistema.AddComponent<StandaloneInputModule>();
    }
}
