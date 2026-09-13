using UnityEngine;

public class moneda : MonoBehaviour
{
    [SerializeField] private AudioClip sonidoRecoger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            contadorMonedas uiManager = Object.FindObjectOfType<contadorMonedas>();
            uiManager.CoinColleted();

            if (sonidoRecoger != null)
            {
                // PlayClipAtPoint crea su propio AudioSource temporal, asi el sonido
                // no se corta al destruir la moneda en este mismo frame
                AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
            }

            Destroy(gameObject); // Destruye la moneda despues de ser recogida
        }
    }
}
