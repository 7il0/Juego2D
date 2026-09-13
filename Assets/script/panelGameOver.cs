using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Construye por codigo el dialogo flotante de derrota, cuando las vidas llegan a cero.
/// Mismo enfoque que panelVictoria: se genera en tiempo de ejecucion sin depender del Inspector.
public class panelGameOver : MonoBehaviour
{
    private RectTransform tarjeta;
    private CanvasGroup grupo;

    /// Punto de entrada: crea el panel y congela el juego.
    public static void Mostrar()
    {
        GameObject raiz = new GameObject("PanelGameOver");
        raiz.AddComponent<panelGameOver>().Construir();
    }

    private void Construir()
    {
        CrearCanvas();
        CrearFondo();
        CrearTarjeta();
        AsegurarEventSystem();

        // Se congela el juego para que no se siga interactuando detras del dialogo
        Time.timeScale = 0f;

        StartCoroutine(AnimarEntrada());
    }

    private void CrearCanvas()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // por encima del HUD de monedas/vidas

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
        imagen.color = new Color(0f, 0f, 0f, 0.75f);

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
        imagen.type = Image.Type.Simple;
        imagen.color = new Color(0.20f, 0.07f, 0.09f, 1f);

        tarjeta = panel.GetComponent<RectTransform>();
        tarjeta.anchorMin = new Vector2(0.5f, 0.5f);
        tarjeta.anchorMax = new Vector2(0.5f, 0.5f);
        tarjeta.pivot = new Vector2(0.5f, 0.5f);
        tarjeta.anchoredPosition = Vector2.zero;
        tarjeta.sizeDelta = new Vector2(820f, 460f);

        CrearFranjaSuperior(tarjeta);
        CrearTexto("Titulo", tarjeta, "GAME OVER", 96f,
            new Color(0.90f, 0.20f, 0.25f), new Vector2(0f, 105f), new Vector2(760f, 130f));
        CrearTexto("Mensaje", tarjeta, "Perdiste", 52f,
            Color.white, new Vector2(0f, 10f), new Vector2(760f, 80f));
        CrearTexto("Detalle", tarjeta, "Te quedaste sin vidas. Intenta de nuevo.", 28f,
            new Color(0.85f, 0.70f, 0.72f), new Vector2(0f, -55f), new Vector2(760f, 60f));
        CrearBoton(tarjeta);
    }

    private void CrearFranjaSuperior(RectTransform padre)
    {
        GameObject franja = NuevoElemento("Franja", padre);
        Image imagen = franja.AddComponent<Image>();
        imagen.type = Image.Type.Simple;
        imagen.color = new Color(0.90f, 0.20f, 0.25f);

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
        GameObject boton = NuevoElemento("BotonReintentar", padre);

        Image imagen = boton.AddComponent<Image>();
        imagen.type = Image.Type.Simple;
        imagen.color = new Color(0.62f, 0.20f, 0.22f);

        RectTransform rect = boton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, -150f);
        rect.sizeDelta = new Vector2(340f, 84f);

        Button control = boton.AddComponent<Button>();
        control.targetGraphic = imagen;
        control.onClick.AddListener(Reintentar);

        CrearTexto("Etiqueta", rect, "Reintentar", 34f,
            Color.white, Vector2.zero, new Vector2(340f, 84f));
    }

    private void Reintentar()
    {
        // Se restaura el tiempo antes de recargar, si no la escena nueva queda congelada
        Time.timeScale = 1f;
        GameManager.Instance.ReiniciarPartida();
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

    /// Crea un GameObject de UI ya emparentado y con RectTransform.
    private GameObject NuevoElemento(string nombre, Transform padre)
    {
        GameObject elemento = new GameObject(nombre, typeof(RectTransform));
        elemento.transform.SetParent(padre, false);
        return elemento;
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
