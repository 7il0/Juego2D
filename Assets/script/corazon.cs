using UnityEngine;

public class corazon : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.GanarVida();
            Destroy(gameObject); // Destruye el corazon despues de ser recogido
        }
    }
}
