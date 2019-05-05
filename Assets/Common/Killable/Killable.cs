using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void HitEventHandler(Killable target);

public class Killable : MonoBehaviour
{
	[System.Serializable]
	public class KillableEvent : UnityEvent {}

	public static List<Killable>	killables			{ get; private set; }
	public float					health				{ get { return currentHealth; } }
	public float					healthNormalized	{ get { return currentHealth / maxHealth; } }
	public bool						isAlive				{ get { return currentHealth > 0; } }
	public bool						isDestroyed			{ get; private set; }
	public bool						isDead				{ get { return !isAlive; } }
	[Header("Killable")]
	public float					maxHealth			= 100f;
	public float					damageMultiplier	= 1f;

	public bool						resetOnEnable		= true;
	public bool						deactivateOnDeath	= false;

	float							currentHealth;

	public delegate void KillableEventHandler(object sender);
	public delegate void KillableDamagedEventHandler(float damage);
	public event KillableDamagedEventHandler OnDamage;
	public event KillableEventHandler OnDeath;
	public event KillableEventHandler OnResurrect;
	public event KillableEventHandler OnHealthChanged;
	public event KillableEventHandler OnDestroyed;

	[SerializeField]
	KillableEvent	m_OnDeath;
	[SerializeField]
	KillableEvent	m_OnDestroyed;
	[SerializeField]
	KillableEvent	m_OnResurrected;

	void Awake ()
	{
		currentHealth = maxHealth;
		isDestroyed = false;
	}

	void OnEnable ()
	{
		if (killables == null)
			killables = new List<Killable>();
		killables.Add(this);

		if (resetOnEnable)
			SetHealth(maxHealth);
	}

	void OnDisable ()
	{
		killables.Remove(this);
	}

	public float GetHealth ()
	{
		return currentHealth;
	}

	public void Resurrect ()
	{
		SetHealth(maxHealth);
	}

	public void SetHealth (float h)
	{
		currentHealth = h;

		if (isAlive)
		{
			if (h < Mathf.Epsilon)
			{
				Kill();
				return;
			}
		}
		else
		{
			if (h > Mathf.Epsilon)
			{
				isDestroyed = false;
				if (OnResurrect != null)
					OnResurrect(this);
				m_OnResurrected.Invoke();
			}
		}
		if (OnHealthChanged != null)
			OnHealthChanged(this);
	}

	public bool Damage (float damage)
	{
		if (damage <= 0)
			return false;
		bool wasAlive = isAlive;
		currentHealth = Mathf.Max(currentHealth - damage * damageMultiplier, 0);
		if (OnDamage != null)
			OnDamage(damage);
		if (OnHealthChanged != null)
			OnHealthChanged(this);
		return DamageCheck(wasAlive);
	}

	bool DamageCheck (bool wasAlive)
	{
		if (currentHealth < Mathf.Epsilon && wasAlive)
		{
			Die();
			return true;
		}
		return false;
	}

	public void Kill ()
	{
		if (!isAlive)
			return;

		bool wasAlive = isAlive;
		currentHealth = 0;
		if (OnHealthChanged != null)
			OnHealthChanged(this);
		DamageCheck(wasAlive);
	}

	public void Destroy ()
	{
		isDestroyed = true;
		Kill();
		if (OnDestroyed != null)
			OnDestroyed(this);
		m_OnDestroyed.Invoke();
	}

	void Die ()
	{
		if (OnDeath != null)
			OnDeath(this);
		m_OnDeath.Invoke();
		if (deactivateOnDeath)
			gameObject.SetActive(false);
	}
}
