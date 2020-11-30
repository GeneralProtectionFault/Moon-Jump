using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce;                          // Amount of force added when the player jumps.

	// MODIFIED CODE
	[SerializeField] private float m_JumpForceOnMoon;
	[SerializeField] private float moonJumpVelocity;

	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private LayerMask m_WhatIsGoo;								// A mask to determine when stepping in goo, which will prevent regular jumping, etc...

	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	public const float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
	public static bool m_Grounded { get; private set; }            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	public static Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;


	// MODIFIED CODE ****************
	
	public static float beenJumpingTime { get; private set; } = 0f;
	private Vector2 zeroedJumpVector;
	public static Vector2 movement { get; private set; }
	// As opposed to "jumping," this variable is just used on the fixed update frame that will tell the character to jump, then get set back to false
	private static bool jump = false;
	public static bool crouch = false;

	public static bool moonJumpTriggered { get; private set; } = false;

	[SerializeField] private float walkSpeed;
	[SerializeField] private float moonWalkSpeedMultiplier;

	[SerializeField] private int airMoonJumpMax;
	public static bool moonJumping { get; private set; } = false;
	public static bool jumping { get; private set; } = false;
    public static int airMoonJumpCount { get; private set; } = 0;  // This will keep track for the purpose of limiting "air" moon jumps
	public static int jumpCount { get; private set; } = 0; // This will keep track if the player has jumped, since knowing the jump is "started" may not be enough
	public static bool passingThroughPlatformCollider { get; private set; } = false;

	[SerializeField] private Animator animator;

	public static GameObject spawnEmptyObject;
	private static GameObject player;

	[SerializeField] private float gooSlowFactor;
	[SerializeField] private float gooTriggerVelocity;


	// ******************************

	private void Awake()
	{
		player = gameObject;
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();


		// Prevent an air moon jump when "falling" into the scene - will be reset when the character lands
		// The reason this needs to be in the Awake method is because spamming the moon jump button upon scene switching will work and
		// give the player a split-second glimpse of the level
		airMoonJumpCount = airMoonJumpMax;

		// **** Set this ONLY at the start ***** so they can't control where they land on entering the scene
		// Set it back to true as soon as they land
		m_AirControl = false;
	}




	private void Start()
	{
		// ********* MODIFIED CODE **********************************

		zeroedJumpVector = new Vector2(0f, m_Rigidbody2D.velocity.y);

		// If you die on a mud platform, this needs to be set back!
		GlobalSettings.isOnGoo = false;

		// Change gravity depending on location
		if (GlobalSettings.onMoon || GlobalSettings.onSun)
		{
			// Flip player upside-down
			Vector3 theScale = transform.localScale;
			theScale.y *= -1;
			transform.localScale = theScale;

			// Flip gravity
			m_Rigidbody2D.gravityScale = GlobalSettings.moonGravity;

			// Modify speed (sludge controls a bit)
			walkSpeed *= (moonWalkSpeedMultiplier);
		}
		else
			m_Rigidbody2D.gravityScale = GlobalSettings.earthGravity;

		// **********************************************************    
	}




	private void OnEnable()
    {
		HazardScript.DeathEvent += OnDeath;
		TimerScript.TimedDeathEvent += OnDeath;
    }

    private void OnDisable()
    {
		HazardScript.DeathEvent -= OnDeath;
		TimerScript.TimedDeathEvent -= OnDeath;
    }


	/// <summary>
	/// Method used to safely override directional controller movement if an outside influence on the player requires it
	/// </summary>
	/// <param name="xVector"></param>
	/// <param name="yVector"></param>
	public static void OverrideMovement(float xVector, float yVector)
    {
		movement = new Vector2(xVector, yVector);
    }



	// Triggers switch between moon & earth
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "PortalCollider")
		{
			// Log the x position on the earth/moon so it can be used for spawning the character in the same x position on the opposing scene
			GlobalSettings.xCoordinate = this.transform.position.x;
			// UnityEngine.Debug.Log("Player left scene at X coordinate:  " + GlobalSettings.xCoordinate);


			// Load the moon scene for the current level
			var currentScene = SceneManager.GetActiveScene().name;
			var currentLevel = currentScene.Substring(5, 3);
			var currentArea = currentScene.Substring(9);

			var targetArea = "";

			// FOR SUN LEVEL(S) ONLY!
			if (GlobalSettings.sunIsOut)
			{
				targetArea = "sun";
				GlobalSettings.onSun = true;
			}
			else
			{
				if (currentArea == "earth")
					targetArea = "moon";
				else if (currentArea == "moon" || currentArea == "sun")
				{
					GlobalSettings.onSun = false;
					targetArea = "earth";
				}
			}

			SceneManager.LoadScene("Level" + currentLevel + '_' + targetArea);
		}
	}


	public static void RestartLevel()
    {
		Destroy(player);
		GlobalSettings.levelStarting = true;

		var currentScene = SceneManager.GetActiveScene().name;
		var currentLevel = currentScene.Substring(5, 3);


		GlobalSettings.xCoordinate = 0f;


		// Just go back to the earth start if dying on the moon or sun
		if (GlobalSettings.onMoon || GlobalSettings.onSun)
		{
			GlobalSettings.onMoon = false;
			GlobalSettings.onSun = false;
			SceneManager.LoadScene("Level" + currentLevel + "_earth");
			return;
		}


		// Spawn at the appropriate location if dying on earth
		Vector3 spawnLocation = new Vector3();

		if (spawnEmptyObject != null)
		{
			spawnLocation = new Vector3(spawnEmptyObject.transform.position.x, spawnEmptyObject.transform.position.y);
		}
		else
		{
			if (GameObject.Find("SpawnStart") != null)
				spawnEmptyObject = GameObject.Find("SpawnStart");
			else
				spawnEmptyObject = GameObject.Find("SpawnDefault");

			spawnLocation = new Vector3(spawnEmptyObject.transform.position.x, spawnEmptyObject.transform.position.y);
		}


		// Respawn the player
		if (spawnLocation == Vector3.zero)
			PlayerSpawn.SpawnPlayer((GameObject)Resources.Load("Player"), true);
		else
			PlayerSpawn.SpawnPlayer((GameObject)Resources.Load("Player"), true, spawnLocation);
		

		GlobalSettings.isDead = false;
		PauseScreenScript.pauseScreen.SetActive(false);
		Time.timeScale = 1f;
	}


	// Will trigger the death animation
	private void OnDeath(object sender, GameObject spawnObject)
    {
		// UnityEngine.Debug.Log("Death event triggered");

		animator.SetTrigger("death");

		GlobalSettings.isDead = true;

		spawnEmptyObject = spawnObject;


		// Disable player's colliders so he can fall off the screen, through whatever objects...
		foreach (var collider in gameObject.GetComponents<Collider2D>())
        {
			collider.enabled = false;
        }

		// Also need to disable colliders in CHILDREN
		Transform[] allChildren = GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren)
        {
			foreach (var collider in child.GetComponents<Collider2D>())
			{
				collider.enabled = false;
			}
		}


		// Toss him off the screen, so to speak...
		// Bump him up a bit and let gravity do the rest :)
		if (GlobalSettings.onMoon || GlobalSettings.onSun)
			m_Rigidbody2D.AddForce(new Vector2(0f, -500f)); 
		else
			m_Rigidbody2D.AddForce(new Vector2(0f, 500f));
	}


	// Call this from an animation event after the death animation to actually destroy the game object :)
	private void OnDeathComplete()
    {
		RestartLevel();		
	}


	/// <summary>
	/// If going through a platform collider, we do not want to "ground" the player if we're passing through that collider.
	/// If that is the case, this method returns true
	/// </summary>
	/// <param name="collider">The collider to check if passing through</param>
	/// <returns></returns>
	private bool ShouldPassThroughPlatformCollider(Collider2D collider)
    {
		var tagOfCollidedObject = collider.gameObject.tag;

		bool playerIsAbovePlatform = false; //= transform.position.y - collider.transform.position.y > 0;


		if (tagOfCollidedObject == "OneWayUpPlatform")
		{
			if (GlobalSettings.onMoon || GlobalSettings.onSun)
				playerIsAbovePlatform = m_CeilingCheck.transform.position.y - collider.transform.position.y > 0;
			else // on earth
				playerIsAbovePlatform = m_GroundCheck.transform.position.y - collider.transform.position.y > .2f;
		}
		else if (tagOfCollidedObject == "OneWayDownPlatform")
        {
			if (GlobalSettings.onMoon || GlobalSettings.onSun)
				playerIsAbovePlatform = m_GroundCheck.transform.position.y - collider.transform.position.y > 0;
			else // on earth
				playerIsAbovePlatform = m_CeilingCheck.transform.position.y - collider.transform.position.y > 0;
		}

		if (tagOfCollidedObject == "OneWayUpPlatform" && !playerIsAbovePlatform ||
			tagOfCollidedObject == "OneWayDownPlatform" && playerIsAbovePlatform)
		{
			return true;
		}
		else
		{
			return false;
		}
	}





    private void FixedUpdate()
	{
		//UnityEngine.Debug.Log(m_Rigidbody2D.velocity.x);

		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		GlobalSettings.isOnGoo = false;


		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround | m_WhatIsGoo); // Goo is also ground!..but we'll prioritize the goo, we'll check for both layers so we don't loop through all of this twice...
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject && !GlobalSettings.isDead)
			{

				// Ignore if we're going "through" a collider, which happens if we're going the "one way" through a one-way collider...
				if (ShouldPassThroughPlatformCollider(colliders[i]))
					continue;


				// If we're stuck in the goo....
				if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Goo") && !moonJumping)
				{
					// UnityEngine.Debug.Log("We're on goo!");
					GlobalSettings.isOnGoo = true;

					var xVelocity = m_Rigidbody2D.velocity.x;
					var yVelocity = m_Rigidbody2D.velocity.y;


					// Note that yVelocity is just here for the Vector2 - triggering the isOnGoo is there to be used to prevent jumping (controlled in CharacterController2D)
					if (xVelocity > gooTriggerVelocity)
					{
						m_Rigidbody2D.velocity = new Vector2(xVelocity - gooSlowFactor, yVelocity);
					}
					else if (xVelocity < -1 * gooTriggerVelocity)
					{
						m_Rigidbody2D.velocity = new Vector2(xVelocity + gooSlowFactor, yVelocity);
					}

					m_Grounded = true;
                    if (!wasGrounded && beenJumpingTime > .04)  // MODIFIED - extra check because was getting triggered at jump start
                    {
                        OnLandEvent.Invoke();
                    }
                    // Sometimes we're on BOTH ground & goo layers.  If so, we want goo to take precedence, so break the loop on the goo layer here!
                    break;
				}


				// Code runs if we're NOT on a goo layer only...
				GlobalSettings.isOnGoo = false;
				m_Grounded = true;

				if (!wasGrounded && beenJumpingTime > .04)  // MODIFIED - extra check because was getting triggered at jump start
				{
					OnLandEvent.Invoke();
				}
				
			}
		}


		// ******** MODIFIED CODE ***************************************





		// Character movement - from input]
		Move(movement.x * walkSpeed, crouch, jump);

		// Set this back to false if we jumped so we cannot double jump
		jump = false;

		//UnityEngine.Debug.Log("X: " + m_Rigidbody2D.velocity.x);
		//UnityEngine.Debug.Log("Y: " + m_Rigidbody2D.velocity.y);


		// Animation stuff...
		if (Mathf.Abs(m_Rigidbody2D.velocity.x) > .001 || Mathf.Abs(m_Rigidbody2D.velocity.y) > .001)
		{
			animator.SetBool("moving", true);
		}
		else
		{
			animator.SetBool("moving", false);
		}

		animator.SetBool("grounded", m_Grounded);



		// Get which y direction for jumping vs. falling animation
		var fallingAnimationMovementThreshold = .2f;

		if (!m_Grounded)
		{
			// If going off a ledge, increase downforce
			if (!jumping)
			{
				var additionalDownForce = 30f;

				if (!GlobalSettings.onMoon && !GlobalSettings.onSun)
					m_Rigidbody2D.AddForce(new Vector2(0f, -1 * additionalDownForce));
				else
					m_Rigidbody2D.AddForce(new Vector2(0f, additionalDownForce));
			}

			// Set the falling animation when moving downward/off ledges/etc...
			if (m_Rigidbody2D.velocity.y > fallingAnimationMovementThreshold && GlobalSettings.onMoon)
			{
				animator.SetBool("falling", true);
				CharacterSoundManager.audioIsMoonJumping = false;
			}
			else if (m_Rigidbody2D.velocity.y < -1 * fallingAnimationMovementThreshold && !GlobalSettings.onMoon)
			{
				animator.SetBool("falling", true);
				CharacterSoundManager.audioIsMoonJumping = false;
			}
		}
		else
			airMoonJumpCount = 0;
		

		// **************************************************************
	}



	// ******** MODIFIED CODE *********************************************************

	// This method triggered when the d-pad/control stick/movement keys are pressed
	public void OnDPadMovement(InputAction.CallbackContext context)
    {
		movement = context.ReadValue<Vector2>();
    }

	// When jump button pressed...
	public void OnJump(InputAction.CallbackContext context)
    {
		if (context.phase == InputActionPhase.Started)
		{
			// Old way - this is only if using the method of holding another button down first
			// Currently unused
			if (moonJumpTriggered)
			{
				PerformMoonJump();
			}
			else if (!GlobalSettings.isOnGoo) // ADDED - only if not on goo!
			{
				// ##### THIS is what makes the player perform a normal jump on the next available frame proided the logic is met!  See the Move method! ######
				jump = true;
			}
			else if(!moonJumping) // IS on mud platform
            {
				// Spawn the "goo graphic" of player trying (and failing) to jump
				Instantiate((GameObject)Resources.Load("GooStuck"), new Vector3(this.transform.position.x, this.transform.position.y -.45f, this.transform.position.z), Quaternion.identity);
			}
		}
    }

	/// <summary>
	/// This method is just if using the moon jump "trigger," in which that button is held down in order for the regular jump button to trigger the moon jump
	/// </summary>
	/// <param name="context"></param>
	public void OnMoonJump(InputAction.CallbackContext context)
    {
		m_AirControl = false;

		if (context.phase == InputActionPhase.Canceled)
			moonJumpTriggered = false;
		else
			moonJumpTriggered = true;
    }

	public void OnMoonJumpButton(InputAction.CallbackContext context)
    {
		if (context.phase == InputActionPhase.Started)
        {
			// Increment if moon jumping in the air
			if (!m_Grounded)
				airMoonJumpCount += 1;

			// Disallow the moon jump if too many air moon jumps
			if (airMoonJumpCount > airMoonJumpMax)
				return;

			PerformMoonJump();
        }
    }


	private void PerformMoonJump()
    {
		moonJumping = true;

		// Avoid tripping this animation if he's in the death animation....this allows hilarious moon jumping while dead w/o breaking the game
		if (!GlobalSettings.isDead)
			animator.SetTrigger("moonJumping");


		// TO THE MOON!
		m_AirControl = false;  // ** This will be set back to true AFTER player lands!
		//m_Rigidbody2D.velocity = zeroedJumpVector; // Disable any horizontal momentum for the moon jump!
		
		// Variable used to notify the sound script because I'm too lazy to do it with events
		CharacterSoundManager.audioIsMoonJumping = true;
		Instantiate((GameObject)Resources.Load("LaunchSmoke"), new Vector3(this.transform.position.x, this.transform.position.y - .4f, this.transform.position.z), Quaternion.identity);


		// Using velocity here, we do not want to use force because this will be impervious to any momentum the player has (falling, etc...)
		// Disable any horizontal momentum for the moon jump!
		if (GlobalSettings.onMoon || GlobalSettings.onSun)
			m_Rigidbody2D.velocity = new Vector2(0f, -1 * moonJumpVelocity);
		else
			m_Rigidbody2D.velocity = new Vector2(0f, moonJumpVelocity);
	}


	public void OnLand()
    {
		// UnityEngine.Debug.Log("Landed");

		moonJumping = false;
		jumping = false;
		animator.SetBool("jumping", false);
		animator.SetBool("falling", false);
		//animator.SetBool("moonJumping", false);
		CharacterSoundManager.audioIsMoonJumping = false;

		jumpCount = 0;
		airMoonJumpCount = 0;

		// This just forces time to resume to normal if the player touches the
		Time.timeScale = 1f;

		if (PauseScreenScript.pauseScreen.activeSelf)
			Time.timeScale = 0f;
		else
			Time.timeScale = 1f;


	}







    // ******************************************************************************

	// Logic to control player depending on crouching/jumping/walking/etc...
    public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius,m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);

            // *** Seems to be a RANDOM STUPID HOP, this is hacking that out...
            //if (!jumping && m_Grounded && !GooStuck.jumpingInGoo)
            //    targetVelocity.y = 0f;


            // And then smoothing it out and applying it to the character
            if (!moonJumping)
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

				

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}



		// If the player should jump...
		// *** ADDED CONDITION *** --only fire if NOT on a goo!
		if (m_Grounded && jump && !GlobalSettings.isOnGoo)
		{
			// UnityEngine.Debug.Log("Jumping!");

			if (!moonJumping)
				m_AirControl = true;

			// Add a vertical force to the player.
			m_Grounded = false;
			jumping = true;
			jumpCount += 1;
			animator.SetBool("jumping", true);

			// MODIFIED CODE *************
			if (GlobalSettings.onMoon)
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForceOnMoon));
			else
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}

        // *************** MODIFIED CODE **************


        // Add downforce to character after peak of jump for more authentic feel
        if (!m_Grounded)
        {
            beenJumpingTime += Time.deltaTime;
            // UnityEngine.Debug.Log("Been jumping for " + beenJumpingTime.ToString());

            if (beenJumpingTime > .35) // This is just prior to the jump's peak
            {
				if (GlobalSettings.onMoon || GlobalSettings.onSun)
				{
					m_Rigidbody2D.AddForce(new Vector2(0f, -1 * m_JumpForceOnMoon / 15));
				}
				else
				{
					m_Rigidbody2D.AddForce(new Vector2(0f, -1 * m_JumpForce / 15));
				}
            }
        }
        else
        {
            beenJumpingTime = 0f;
        }


        // *************** MODIFIED CODE **************
    }



	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}





}