using UnityEngine;

public class puas : MonoBehaviour
{
    [SerializeField] private AudioClip sonidoGolpe;
    [SerializeField] [Range(0f, 1f)] private float volumen = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (sonidoGolpe != null)
            {
                // PlayClipAtPoint no depende de Time.timeScale, asi que suena
                // aunque el juego se congele justo despues por el panel de Game Over
                AudioSource.PlayClipAtPoint(sonidoGolpe, transform.position, volumen);
            }

            GameManager.Instance.Morir();
        }
    }
}
