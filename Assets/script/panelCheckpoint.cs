using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Aviso flotante y no bloqueante que confirma que se activo un nuevo checkpoint.
/// A diferencia de los paneles de victoria/derrota, no congela el juego: aparece,
/// se mantiene un momento y se desvanece sola.
public class panelCheckpoint : MonoBehaviour
{
    private CanvasGroup grupo;
    private float duracionVisible;

    public static void Mostrar(int numeroBandera, float duracionVisible)
    {
        GameObject raiz = new GameObject("PanelCheckpoint");
        raiz.AddComponent<panelCheckpoint>().Construir(numeroBandera, duracionVisible);
    }

    private void Construir(int numeroBandera, float duracionVisible)
    {
        this.duracionVisible = duracionVisible;
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500; // sobre el HUD, pero por debajo de los paneles modales

        CanvasScaler escalador = gameObject.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        grupo = gameObject.AddComponent<CanvasGroup>();
        grupo.alpha = 0f;
        grupo.blocksRaycasts = false; // es solo informativo, no debe interceptar clics ni pausar nada

        CrearTarjeta(numeroBandera);

        StartCoroutine(MostrarYDesvanecer());
    }

    private void CrearTarjeta(int numeroBandera)
    {
        GameObject tarjeta = new GameObject("Tarjeta", typeof(RectTransform));
        tarjeta.transform.SetParent(transform, false);

        Image imagen = tarjeta.AddComponent<Image>();
        imagen.color = new Color(0.08f, 0.35f, 0.20f, 0.92f);

        RectTransform rect = tarjeta.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -60f);
        rect.sizeDelta = new Vector2(640f, 110f);

        GameObject textoObj = new GameObject("Texto", typeof(RectTransform));
        textoObj.transform.SetParent(tarjeta.transform, false);

        TextMeshProUGUI tmp = textoObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Punto de control alcanzado\nA partir de ahora reaparecerás aquí";
        tmp.fontSize = 30f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;

        RectTransform textRect = textoObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(20f, 10f);
        textRect.offsetMax = new Vector2(-20f, -10f);
    }

    private IEnumerator MostrarYDesvanecer()
    {
        const float duracionAparicion = 0.25f;
        const float duracionDesvanecido = 0.5f;

        float t = 0f;
        while (t < duracionAparicion)
        {
            t += Time.deltaTime;
            grupo.alpha = Mathf.Clamp01(t / duracionAparicion);
            yield return null;
        }
        grupo.alpha = 1f;

        yield return new WaitForSeconds(duracionVisible);

        t = 0f;
        while (t < duracionDesvanecido)
        {
            t += Time.deltaTime;
            grupo.alpha = 1f - Mathf.Clamp01(t / duracionDesvanecido);
            yield return null;
        }

        Destroy(gameObject);
    }
}
