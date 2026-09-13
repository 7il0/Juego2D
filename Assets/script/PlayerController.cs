using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rb2D;

    private float move;

    public float jumpForce = 4;
    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius=0.1f;
    public LayerMask groundLayer;

    public Animator animator;

    // Un sonido en loop por cada estado (igual que las 4 animaciones: respirar, caminar, saltar, caida)
    [SerializeField] private AudioClip sonidoEstatico;
    [SerializeField] private AudioClip sonidoAvanzar;
    [SerializeField] private AudioClip sonidoSaltar;
    [SerializeField] private AudioClip sonidoCaer;

    private AudioSource audioEstado;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        audioEstado = gameObject.AddComponent<AudioSource>();
        audioEstado.loop = true;
        audioEstado.playOnAwake = false;

        // Punto de reaparicion por defecto si el jugador aun no pisa ninguna bandera
        GameManager.Instance.RegistrarSpawnInicial(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
        }

        if(move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
        }

        animator.SetFloat("velocidad", Mathf.Abs(move));
        animator.SetFloat("velocidad_vertical", rb2D.linearVelocity.y);
        animator.SetBool("saltando", isGrounded);

        ActualizarSonidoDeEstado();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    /// <summary>
    /// Elige que clip debe sonar segun el mismo estado que ya usa el Animator,
    /// y solo reinicia el AudioSource cuando el clip realmente cambia.
    /// </summary>
    private void ActualizarSonidoDeEstado()
    {
        AudioClip clipDeseado;

        if (!isGrounded)
        {
            // En el aire: subiendo es "saltar", bajando es "caida"
            clipDeseado = rb2D.linearVelocity.y > 0f ? sonidoSaltar : sonidoCaer;
        }
        else
        {
            clipDeseado = move != 0f ? sonidoAvanzar : sonidoEstatico;
        }

        if (audioEstado.clip == clipDeseado)
        {
            return;
        }

        audioEstado.clip = clipDeseado;

        if (clipDeseado != null)
        {
            audioEstado.Play();
        }
        else
        {
            audioEstado.Stop();
        }
    }
}
