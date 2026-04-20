using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public ZombieSpawner spawner;
    public float moveSpeed = 2f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            MoveToPlayer();
        }
        else
        {
            TryAttack();
        }
    }

    // ---------------- MOVEMENT ----------------
    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        anim.SetFloat("Speed", Mathf.Abs(direction.x));

        // Flip
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // ---------------- ATTACK ----------------
    void TryAttack()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetFloat("Speed", 0);

        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;

            anim.SetTrigger("Attack");

            // Damage player
            player.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }

    // ---------------- TAKE DAMAGE ----------------
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        anim.SetTrigger("Hurt");

        // You can add health here if needed
        // For now, 1 hit = death (change later)

        Die();
    }

    // ---------------- DEATH ----------------
    void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;

        spawner?.ZombieDied();

        Destroy(gameObject, 2f);
    }
}