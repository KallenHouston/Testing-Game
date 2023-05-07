using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] bullets;
    [SerializeField] private LayerMask groundLayer;



    private Animator anim;
    private BoxCollider2D boxCollider;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (Mathf.Abs(playerMovement.body.velocity.x) < 0.05f)
        {
            anim.SetBool("IsAttacking", false);
        }
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Attack();
            anim.SetBool("IsAttacking", true);
            if (playerMovement.IsGrounded())
            {
                firePoint.position = new Vector3(firePoint.position.x, transform.position.y - 0.04f, firePoint.position.z);
                if (Mathf.Abs(playerMovement.body.velocity.x) < 0.05f)
                {
                    anim.SetTrigger("Attack");
                }
                else
                {
                    anim.SetBool("IsAttacking", true);
                }
            }
            if (!playerMovement.IsGrounded())
            {
                anim.SetTrigger("Jumpattack");
            } 
        }
        cooldownTimer += Time.deltaTime;


    }

    private void Attack()
    {


        bullets[FindBullets()].transform.position = firePoint.position;
        bullets[FindBullets()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));

        cooldownTimer = 0;
    }


private int FindBullets()
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            if (!bullets[i].activeInHierarchy)
                return i;
        }
        return 0;
    }


}