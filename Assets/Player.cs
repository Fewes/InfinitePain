using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public static Player local;

	public static int score;

	// Inspector
	public float		movementSpeed		= 10f;
	public float		acceleration		= 10f;
	public float		decceleration		= 10f;
	public float		mouseSensitivity	= 3f;
	public Light		headLight;
	public WeaponRig	weaponRig;
	public Transform	head;
	public int			shotgunAmmo			= 32;
	public Animator		foot;
	public LayerMask	kickLayers;
	public float		kickForce			= 5;
	public Feet			feet;

	[Header("UI")]
	public Image		crosshair;
	public Image		healthImage;
	public Text			healthText;
	public Text			ammoText;
	public Text			magText;

	public bool			isKicking { get; private set; } = false;

	Rigidbody _rigidbody;
	public new Rigidbody rigidbody
	{
		get
		{
			if (!_rigidbody)
				_rigidbody = GetComponent<Rigidbody>();
			return _rigidbody;
		}
	}

	CharacterController _controller;
	public CharacterController controller
	{
		get
		{
			if (!_controller)
				_controller = GetComponent<CharacterController>();
			return _controller;
		}
	}

	Camera _camera;
	public new Camera camera
	{
		get
		{
			if (!_camera)
				_camera = GetComponentInChildren<Camera>();
			return _camera;
		}
	}

	Killable _killable;
	public Killable killable
	{
		get
		{
			if (!_killable)
				_killable = GetComponent<Killable>();
			return _killable;
		}
	}

	// Private
	Vector3 input		= Vector3.zero;
	float	cameraPitch	= 0;
	float	cameraYaw	= 0;
	Quaternion headLightRotation;

    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		if (local && local != this)
			Debug.LogError("Multiple local players detected");
		else
			local = this;

		killable.OnDeath += Killable_OnDeath;
		killable.OnDamage += Killable_OnDamage;
		killable.OnHealthChanged += Killable_OnHealthChanged;

		feet = GetComponentInChildren<Feet>();

		headLightRotation = camera.transform.rotation;
		foot.gameObject.SetActive(false);
    }

	private void Killable_OnDamage (float damage)
	{
		PoolManager.GetPooledObject("Effects", "BloodSplat", transform.position + Vector3.up * 1.5f);
		KickCamera();
		if (killable.isAlive)
			AudioManager.PlaySoundEffect("PlayerHurt", transform.position);
	}

	private void Killable_OnDeath (object sender)
	{
		controller.height = 0;
		controller.center = Vector3.up * 1.3f;
		weaponRig.gameObject.SetActive(false);
		crosshair.enabled = false;
		AudioManager.PlaySoundEffect("PlayerDie", transform.position);
	}

	private void Killable_OnHealthChanged(object sender)
	{
		var color = Color.white;
		if (killable.healthNormalized < 0.5f)
			color = Color.yellow;
		if (killable.healthNormalized < 0.25f)
			color = Color.red;
		healthText.text = killable.health.ToString();
		healthText.color = color;
		healthImage.color = color;
	}

	void Update ()
    {
		UpdateCamera();
        UpdateInput();
		UpdateMovement();
		UpdateUI();

		if (Input.GetKeyDown(KeyCode.F))
			Kick();
    }

	void Kick ()
	{
		if (!isKicking && killable.isAlive)
			StartCoroutine(KickSequence());
	}

	IEnumerator KickSequence ()
	{
		isKicking = true;
		KickCamera(0.025f);
		foot.gameObject.SetActive(true);
		foot.SetTrigger("Kick");
		yield return new WaitForSeconds(0.1f);

		Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		// Move ray origin back one meter
		ray.origin -= ray.direction;
        var hits = Physics.SphereCastAll(ray, 0.5f, 3f, kickLayers);
		foreach (var hit in hits)
		{
            // Hit something
			var enemy = hit.transform.GetComponentInChildren<Enemy>();
			if (enemy)
				enemy.Flinch();
			var rb = hit.transform.GetComponentInChildren<Rigidbody>();
			if (rb)
				rb.velocity += ray.direction * kickForce;
        }
		if (hits.Length > 0)
			AudioManager.PlaySoundEffect("KickHit", ray.origin + ray.direction * 2);
		else
			AudioManager.PlaySoundEffect("KickMiss", ray.origin + ray.direction * 2);

		yield return new WaitForSeconds(0.3f);
		foot.gameObject.SetActive(false);
		isKicking = false;
	}

	void UpdateInput ()
	{
		// Get input
		var x = killable.isAlive ? Input.GetAxis("Horizontal") : 0;
		var z = killable.isAlive ? Input.GetAxis("Vertical") : 0;

		input.z = Mathf.Lerp(input.z, z, Time.deltaTime * (Mathf.Approximately(z, 0) ? decceleration : acceleration));
		input.x = Mathf.Lerp(input.x, x, Time.deltaTime * (Mathf.Approximately(x, 0) ? decceleration : acceleration));
	}

	void UpdateCamera ()
	{
		cameraPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		cameraPitch = Mathf.Clamp(cameraPitch, -89, 89);
		cameraYaw += Input.GetAxis("Mouse X") * mouseSensitivity;

		head.rotation = Quaternion.Euler(cameraPitch, cameraYaw, killable.isAlive ? 0 : -90);

		headLightRotation = Quaternion.Lerp(headLightRotation, Quaternion.LookRotation(head.forward, Vector3.up), Time.deltaTime * 8);
		headLight.transform.rotation = headLightRotation;

		camera.transform.localRotation = Quaternion.Lerp(camera.transform.localRotation, Quaternion.identity, Time.deltaTime * 8);
	}

	void UpdateUI ()
	{
		switch (weaponRig.current.type)
		{
			case Weapon.Type.Shotgun:
				ammoText.text = shotgunAmmo.ToString();
			break;
		}

		magText.text = weaponRig.current.ammo.ToString();
	}

	public void KickCamera (float amount = 0.1f)
	{
		camera.transform.localRotation = Quaternion.Lerp(Quaternion.identity, Random.rotation, amount);
	}

	Vector3 velocity;
	void UpdateMovement ()
	{
		// Flattened, camera relative coordinate system
		var fwd = camera.transform.forward;
		var right = camera.transform.right;
		fwd.y = 0;
		right.y = 0;
		fwd.Normalize();
		right.Normalize();

		var newVelocity = (fwd * input.z + right * input.x) * movementSpeed;
		velocity.x = newVelocity.x;
		velocity.z = newVelocity.z;
		velocity.y += -9.82f * Time.deltaTime;

		if (feet.grounded && Input.GetKeyDown(KeyCode.Space))
			Jump();

		controller.Move(velocity * Time.deltaTime);
	}

	void Jump ()
	{
		AudioManager.PlaySoundEffect("PlayerJump", transform.position);
		velocity.y = 5f;
	}

	/*
	private void FixedUpdate ()
	{
		bool grounded = true;

		// Flattened, camera relative coordinate system
		var fwd = camera.transform.forward;
		var right = camera.transform.right;
		fwd.y = 0;
		right.y = 0;
		fwd.Normalize();
		right.Normalize();

		var velocity = rigidbody.velocity;

		if (grounded)
		{
			var newVelocity = (fwd * input.z + right * input.x) * movementSpeed;
			velocity.x = newVelocity.x;
			velocity.z = newVelocity.z;
		}

		rigidbody.velocity = velocity;
	}
	*/
}
