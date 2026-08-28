using UnityEngine;

public class moneda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            contadorMonedas uiManager = Object.FindObjectOfType<contadorMonedas>();
            uiManager.CoinColleted();
            Destroy(gameObject); // Destruye la moneda despu�s de ser recogida
        }
    }
}
