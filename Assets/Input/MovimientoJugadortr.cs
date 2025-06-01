using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MovimientoJugadortr : MonoBehaviour
{
    [Header("Movimiento y Salto")]
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float fuerzaSalto;
    [SerializeField] private Rigidbody2D rb2D;
    private Animator animator;

    [Header("Dash")]
    [SerializeField] private float fuerzaDash = 10f;
    [SerializeField] private float tiempoDash = 0.2f;
    private bool puedeHacerDash = true;
    private bool estaHaciendoDash = false;
    [SerializeField] private float cooldownDash = 4f; // 4 segundo de cooldown
    [SerializeField] private Image cooldownDashImage;



    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 boxCastSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private float boxCastMaxDistance = 0.1f;

    private Movimiento input;
    private float movimientoHorizontal = 0f;
    private bool estaSaltando = false;
    private Vector3 velocidad = Vector3.zero;

    private void Awake()
    {
        input = new Movimiento();

        input.MovimientoJugador.Salto.performed += OnJumpPerformed;
        input.MovimientoJugador.InteraccionDash.performed += OnDashPerformed;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("dashing", false);
    }

    private void OnEnable()
    {
        input.MovimientoJugador.Enable();
    }

    private void OnDisable()
    {
        input.MovimientoJugador.Salto.performed -= OnJumpPerformed;
        input.MovimientoJugador.InteraccionDash.performed -= OnDashPerformed;
        input.MovimientoJugador.Disable();
    }

    private void Update()
    {
        if (estaHaciendoDash) return;

        movimientoHorizontal = input.MovimientoJugador.Horizontal.ReadValue<float>() * velocidadMovimiento;

        if (animator != null)
        {
            animator.SetBool("running", movimientoHorizontal != 0.0f);
            if (movimientoHorizontal < 0.0f) transform.localScale = new Vector3(-1.2f, 1.2f, 1.2f);
            else if (movimientoHorizontal > 0.0f) transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        animator.SetBool("jumping", !IsGrounded());
        if (estaSaltando && IsGrounded())
        {
            rb2D.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            estaSaltando = false;
        }
        else
        {
            estaSaltando = false;
        }
        
    }

    private void FixedUpdate()
    {
        if (!estaHaciendoDash)
        {
            Mover(movimientoHorizontal * Time.fixedDeltaTime);
        }
    }

    private void Mover(float mover)
    {
        Vector3 velocidadObjetivo = new Vector2(mover, rb2D.linearVelocity.y);
        rb2D.linearVelocity = Vector3.SmoothDamp(rb2D.linearVelocity, velocidadObjetivo, ref velocidad, 0f);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        estaSaltando = true;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (puedeHacerDash)
        {
            StartCoroutine(HacerDash());
        }
    }

    private System.Collections.IEnumerator HacerDash()
    {
        puedeHacerDash = false;
        estaHaciendoDash = true;

        if (animator != null)
        {
            animator.SetBool("dashing", true);
        }

        rb2D.linearVelocity = new Vector2(transform.localScale.x * fuerzaDash, rb2D.linearVelocity.y);


        yield return new WaitForSeconds(tiempoDash);

        estaHaciendoDash = false;
        animator.SetBool("dashing", false);
        StartCoroutine(DashCooldownVisual(cooldownDash));
        yield return new WaitForSeconds(cooldownDash);
        puedeHacerDash = true;


        if (animator != null)
        {
            animator.SetBool("dashing", false);
        }

    }

    private System.Collections.IEnumerator DashCooldownVisual(float cooldown)
    {
        float tiempo = 0f;
        cooldownDashImage.fillAmount = 1f; // Lleno al iniciar cooldown

        while (tiempo < cooldown)
        {
            tiempo += Time.deltaTime;
            cooldownDashImage.fillAmount = 1f - (tiempo / cooldown);
            yield return null;
        }

        cooldownDashImage.fillAmount = 0f; // Vacío al terminar
    }


    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheck.position,
            boxCastSize,
            0f,
            Vector2.down,
            boxCastMaxDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.blue;
        Vector2 origin = groundCheck.position;
        Vector2 target = origin + Vector2.down * boxCastMaxDistance;

        Gizmos.DrawWireCube(origin, boxCastSize);
        Gizmos.DrawWireCube(target, boxCastSize);

        Vector2 halfSize = boxCastSize / 2f;
        Gizmos.DrawLine(origin + new Vector2(-halfSize.x, halfSize.y), target + new Vector2(-halfSize.x, halfSize.y));
        Gizmos.DrawLine(origin + new Vector2(halfSize.x, halfSize.y), target + new Vector2(halfSize.x, halfSize.y));
        Gizmos.DrawLine(origin + new Vector2(-halfSize.x, -halfSize.y), target + new Vector2(-halfSize.x, -halfSize.y));
        Gizmos.DrawLine(origin + new Vector2(halfSize.x, -halfSize.y), target + new Vector2(halfSize.x, -halfSize.y));
    }
    
    
}