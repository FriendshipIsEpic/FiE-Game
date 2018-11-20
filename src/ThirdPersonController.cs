using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
	public AnimationClip idleAnimation;

	public AnimationClip walkAnimation;

	public AnimationClip runAnimation;

	public AnimationClip jumpPoseAnimation;

	public float walkMaxAnimationSpeed;

	public float trotMaxAnimationSpeed;

	public float runMaxAnimationSpeed;

	public float jumpAnimationSpeed;

	public float landAnimationSpeed;

	private Animation _animation;

	private CharacterState _characterState;

	public float walkSpeed;

	public float trotSpeed;

	public float runSpeed;

	public float inAirControlAcceleration;

	public float jumpHeight;

	public float gravity;

	public float speedSmoothing;

	public float rotateSpeed;

	public float trotAfterSeconds;

	public bool canJump;

	private float jumpRepeatTime;

	private float jumpTimeout;

	private float groundedTimeout;

	private float lockCameraTimer;

	private Vector3 moveDirection;

	private float verticalSpeed;

	private float moveSpeed;

	private CollisionFlags collisionFlags;

	private bool jumping;

	private bool jumpingReachedApex;

	private bool movingBack;

	private bool isMoving;

	private float walkTimeStart;

	private float lastJumpButtonTime;

	private float lastJumpTime;

	private float lastJumpStartHeight;

	private Vector3 inAirVelocity;

	private float lastGroundedTime;

	private bool isControllable;

	public ThirdPersonController()
	{
		walkMaxAnimationSpeed = 0.75f;
		trotMaxAnimationSpeed = 1f;
		runMaxAnimationSpeed = 1f;
		jumpAnimationSpeed = 1.15f;
		landAnimationSpeed = 1f;
		walkSpeed = 2f;
		trotSpeed = 4f;
		runSpeed = 6f;
		inAirControlAcceleration = 3f;
		jumpHeight = 0.5f;
		gravity = 20f;
		speedSmoothing = 10f;
		rotateSpeed = 500f;
		trotAfterSeconds = 3f;
		canJump = true;
		jumpRepeatTime = 0.05f;
		jumpTimeout = 0.15f;
		groundedTimeout = 0.25f;
		moveDirection = Vector3.zero;
		lastJumpButtonTime = -10f;
		lastJumpTime = -1f;
		inAirVelocity = Vector3.zero;
		isControllable = true;
	}

	public override void Awake()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		_animation = (Animation)GetComponent(typeof(Animation));
		if (!(bool)_animation)
		{
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		}
		if (!(bool)idleAnimation)
		{
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if (!(bool)walkAnimation)
		{
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if (!(bool)runAnimation)
		{
			_animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if (!(bool)jumpPoseAnimation && canJump)
		{
			_animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
	}

	public override void UpdateSmoothedMovementDirection()
	{
		Transform transform = Camera.main.transform;
		bool flag = IsGrounded();
		Vector3 a = transform.TransformDirection(Vector3.forward);
		a.y = 0f;
		a = a.normalized;
		Vector3 a2 = new Vector3(a.z, 0f, 0f - a.x);
		float axisRaw = Input.GetAxisRaw("Vertical");
		float axisRaw2 = Input.GetAxisRaw("Horizontal");
		if (!(axisRaw >= -0.2f))
		{
			movingBack = true;
		}
		else
		{
			movingBack = false;
		}
		bool flag2 = isMoving;
		int num = (Mathf.Abs(axisRaw2) > 0.1f) ? 1 : 0;
		if (num == 0)
		{
			num = ((Mathf.Abs(axisRaw) > 0.1f) ? 1 : 0);
		}
		isMoving = ((byte)num != 0);
		Vector3 vector = axisRaw2 * a2 + axisRaw * a;
		if (flag)
		{
			lockCameraTimer += Time.deltaTime;
			if (isMoving != flag2)
			{
				lockCameraTimer = 0f;
			}
			if (vector != Vector3.zero)
			{
				if (!(moveSpeed >= walkSpeed * 0.9f) && flag)
				{
					moveDirection = vector.normalized;
				}
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, vector, rotateSpeed * 0.0174532924f * Time.deltaTime, 1000f);
					moveDirection = moveDirection.normalized;
				}
			}
			float t = speedSmoothing * Time.deltaTime;
			float num2 = Mathf.Min(vector.magnitude, 1f);
			_characterState = CharacterState.Idle;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				num2 *= runSpeed;
				_characterState = CharacterState.Running;
			}
			else if (!(Time.time - trotAfterSeconds <= walkTimeStart))
			{
				num2 *= trotSpeed;
				_characterState = CharacterState.Trotting;
			}
			else
			{
				num2 *= walkSpeed;
				_characterState = CharacterState.Walking;
			}
			moveSpeed = Mathf.Lerp(moveSpeed, num2, t);
			if (!(moveSpeed >= walkSpeed * 0.3f))
			{
				walkTimeStart = Time.time;
			}
		}
		else
		{
			if (jumping)
			{
				lockCameraTimer = 0f;
			}
			if (isMoving)
			{
				inAirVelocity += vector.normalized * Time.deltaTime * inAirControlAcceleration;
			}
		}
	}

	public override void ApplyJumping()
	{
		if (lastJumpTime + jumpRepeatTime <= Time.time && IsGrounded() && canJump && !(Time.time >= lastJumpButtonTime + jumpTimeout))
		{
			verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
			SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void ApplyGravity()
	{
		if (isControllable)
		{
			bool button = Input.GetButton("Jump");
			if (jumping && !jumpingReachedApex && !(verticalSpeed > 0f))
			{
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			if (IsGrounded())
			{
				verticalSpeed = 0f;
			}
			else
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
		}
	}

	public override float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * gravity);
	}

	public override void DidJump()
	{
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		Vector3 position = transform.position;
		lastJumpStartHeight = position.y;
		lastJumpButtonTime = -10f;
		_characterState = CharacterState.Jumping;
	}

	public override void Update()
	{
		if (!isControllable)
		{
			Input.ResetInputAxes();
		}
		if (Input.GetButtonDown("Jump"))
		{
			lastJumpButtonTime = Time.time;
		}
		UpdateSmoothedMovementDirection();
		ApplyGravity();
		ApplyJumping();
		Vector3 a = moveDirection * moveSpeed + new Vector3(0f, verticalSpeed, 0f) + inAirVelocity;
		a *= Time.deltaTime;
		CharacterController characterController = (CharacterController)GetComponent(typeof(CharacterController));
		collisionFlags = characterController.Move(a);
		if ((bool)_animation)
		{
			if (_characterState == CharacterState.Jumping)
			{
				if (!jumpingReachedApex)
				{
					_animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);
				}
				else
				{
					_animation[jumpPoseAnimation.name].speed = 0f - landAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);
				}
			}
			else if (!(characterController.velocity.sqrMagnitude >= 0.1f))
			{
				_animation.CrossFade(idleAnimation.name);
			}
			else if (_characterState == CharacterState.Running)
			{
				_animation[runAnimation.name].speed = Mathf.Clamp(characterController.velocity.magnitude, 0f, runMaxAnimationSpeed);
				_animation.CrossFade(runAnimation.name);
			}
			else if (_characterState == CharacterState.Trotting)
			{
				_animation[walkAnimation.name].speed = Mathf.Clamp(characterController.velocity.magnitude, 0f, trotMaxAnimationSpeed);
				_animation.CrossFade(walkAnimation.name);
			}
			else if (_characterState == CharacterState.Walking)
			{
				_animation[walkAnimation.name].speed = Mathf.Clamp(characterController.velocity.magnitude, 0f, walkMaxAnimationSpeed);
				_animation.CrossFade(walkAnimation.name);
			}
		}
		if (IsGrounded())
		{
			transform.rotation = Quaternion.LookRotation(moveDirection);
		}
		else
		{
			Vector3 forward = a;
			forward.y = 0f;
			if (!(forward.sqrMagnitude <= 0.001f))
			{
				transform.rotation = Quaternion.LookRotation(forward);
			}
		}
		if (IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping)
			{
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public override void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Vector3 vector = hit.moveDirection;
		if (vector.y <= 0.01f)
		{
		}
	}

	public override float GetSpeed()
	{
		return moveSpeed;
	}

	public override bool IsJumping()
	{
		return jumping;
	}

	public override bool IsGrounded()
	{
		return (collisionFlags & CollisionFlags.Below) != CollisionFlags.None;
	}

	public override Vector3 GetDirection()
	{
		return moveDirection;
	}

	public override bool IsMovingBackwards()
	{
		return movingBack;
	}

	public override float GetLockCameraTimer()
	{
		return lockCameraTimer;
	}

	public override bool IsMoving()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
	}

	public override bool HasJumpReachedApex()
	{
		return jumpingReachedApex;
	}

	public override bool IsGroundedWithTimeout()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	public override void Reset()
	{
		gameObject.tag = "Player";
	}

	public override void Main()
	{
	}
}
