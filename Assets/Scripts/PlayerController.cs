using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float runSpeed = 6f;

    [Header("Combat")]
    public int attack1Damage = 20;
    public int attack2Damage = 35;

    [Header("Grapple")]
    public float grappleDistance = 8f;
    public float grappleSpeed = 10f;
    public LayerMask grappleLayer;

    [Header("Health")]
    public int maxHealth = 100;

    private int currentHealth;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;
    private bool isGrappling = false;

    private Vector2 grapplePoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleAttack();
        HandleGrapple();
    }

    // ---------------- RUN ONLY ----------------
    void HandleMovement()
    {
        if (isGrappling) return;

        float move = 0f;

        if (Input.GetKey(KeyCode.A))
            move = -1f;
        else if (Input.GetKey(KeyCode.D))
            move = 1f;

        rb.linearVelocity = new Vector2(move * runSpeed, rb.linearVelocity.y);

        anim.SetFloat("Speed", Mathf.Abs(move));

        if (move > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (move < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // ---------------- ATTACK ----------------
    void HandleAttack()
    {
        if (isGrappling) return;

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack1");
        }

        if (Input.GetMouseButtonDown(1))
            {
                anim.SetTrigger("Attack2");
            }
    }

    // ---------------- DAMAGE (called via animation events) ----------------
    public void DealDamage(int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);

        foreach (Collider2D col in hits)
        {
            if (col.CompareTag("Enemy"))
            {
                col.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // Animation Event Functions
    public void Attack1Damage()
    {
        DealDamage(attack1Damage);
    }

    public void Attack2Damage()
    {
        DealDamage(attack2Damage);
    }

    // ---------------- GRAPPLE ----------------
    void HandleGrapple()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isGrappling)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, grappleDistance, grappleLayer);

            if (hit.collider != null)
            {
                grapplePoint = hit.point;
                isGrappling = true;
                anim.SetTrigger("Grapple");
            }
        }

        if (isGrappling)
        {
            Vector2 direction = (grapplePoint - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * grappleSpeed;

            if (Vector2.Distance(transform.position, grapplePoint) < 0.5f)
            {
                isGrappling = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    // ---------------- TAKE DAMAGE ----------------
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ---------------- DEATH ----------------
    void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
    }

    // ---------------- DEBUG ----------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}