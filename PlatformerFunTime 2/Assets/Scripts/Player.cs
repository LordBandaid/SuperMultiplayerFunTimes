using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Player : MonoBehaviour {

	public float jumpHeight = 3.5f;
	public float timeToJumpApex = .4f;
	public float wallSlideSpeedMax = 3;

	public WeaponController weapon;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallStickTime = .25f;
	float timeToWallUnstick;

	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;

	float gravity;
	float jumpSpeed;
	Vector3 velocity;
	float velocityXSmoothing;

	CharacterController controller;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		weapon = null;

		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpSpeed = Mathf.Abs(gravity) * timeToJumpApex;
	
		print ("Gravity: " + gravity + " | Jump Speed: " + jumpSpeed);
	}

	void Update(){
		//Testing
		//WeaponController t = controller.WeaponCollision (this.transform.position);



		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		int wallDirX = (controller.collisions.left) ? -1 : 1;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

		bool wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax){
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0){
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (input.x != wallDirX && input.x != 0)
					timeToWallUnstick -= Time.deltaTime;
				else
					timeToWallUnstick = wallStickTime;
			}
			else
				timeToWallUnstick = wallStickTime;
		}

		if (controller.collisions.below || controller.collisions.above) {
			velocity.y = 0;
		}

		if (Input.GetKeyDown ("joystick button 3") || Input.GetKeyDown (KeyCode.E)) {
			if (weapon != null){
				weapon.Thrown();
				weapon = null;
			}else if (weapon == null){
				weapon = controller.WeaponCollision(this.transform.position);
				if (weapon != null){
					weapon.Equipped(this);
				}
			}

		}

		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown("joystick button 0")) {
			if (wallSliding){
				if (wallDirX == input.x){
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				}
				else if (input.x == 0){
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				}
				else if (wallDirX != input.x){
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}

			}

			if (controller.collisions.below){
				velocity.y = jumpSpeed;
			}
		}



		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

}
