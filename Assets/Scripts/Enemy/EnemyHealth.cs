using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	[Header("Health")]
	public int maxHealth = 100;
	public int currentHealth { get; private set; }
	private Animator anim;
	private bool dead;

	[Header("iFrames")]
	[SerializeField] private float iFramesDuration;
	[SerializeField] private float numberOfFlashes;
	private SpriteRenderer spriteRend;

	private void Awake()
	{
		currentHealth = maxHealth;
		anim = GetComponent<Animator>();
		spriteRend = GetComponent<SpriteRenderer>();
	}
		
	public void TakeDamage(int damage)
	{
		currentHealth -= damage;

		if (currentHealth > 0)
		{
			anim.SetTrigger("Hurt");
			StartCoroutine(Invunerability());
		}
		else
		{
			if (!dead)
			{
				anim.SetTrigger("Die");
				if (GetComponent<EnemyPatrol>() != null)
				GetComponent<EnemyPatrol>().enabled = false;

				if (GetComponent<MeleeEnemy>() != null)
				{
					GetComponent<MeleeEnemy>().enabled = false;
				}
				dead = true;
			}
		}


	}
	public void AddHealth(int _value)
	{
		currentHealth = Mathf.Clamp(currentHealth + _value, 0, currentHealth);
	}

	private IEnumerator Invunerability()
	{
		Physics2D.IgnoreLayerCollision(10, 11, true);
		for (int i = 0; i < numberOfFlashes; i++)
		{
			spriteRend.color = new Color(1, 0, 0, 0.5f);
			yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
			spriteRend.color = Color.white;
			yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
		}
		Physics2D.IgnoreLayerCollision(10, 11, false);
	}
}