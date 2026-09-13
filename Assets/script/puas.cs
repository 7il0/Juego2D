using UnityEngine;

public class puas : MonoBehaviour
{
    [SerializeField] private AudioClip sonidoGolpe;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (sonidoGolpe != null)
            {
                // PlayClipAtPoint no depende de Time.timeScale, asi que suena
                // aunque el juego se congele justo despues por el panel de Game Over
                AudioSource.PlayClipAtPoint(sonidoGolpe, transform.position);
            }

            GameManager.Instance.Morir();
        }
    }
}
