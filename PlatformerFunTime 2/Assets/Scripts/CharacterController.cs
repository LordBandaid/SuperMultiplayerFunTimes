using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class CharacterController : MonoBehaviour {
	
	public LayerMask collisionMask;
	public LayerMask weaponMask;

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	
	float horizontalRaySpacing;
	float verticalRaySpacing;
	
	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	void Start() {
		collider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
		weaponMask = LayerMask.NameToLayer("Weapon");
	}
	
	public void Move(Vector3 velocity) {
		UpdateRaycastOrigins ();

		collisions.Reset ();

		if (velocity.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(velocity.x);
		}

		HorizontalCollisions (ref velocity);

		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);
	}

	public WeaponController WeaponCollision(Vector3 pos){
		//Set a weapon in range to return to null. If no weapon is found in range, null will be sent to the Player
		WeaponController weaponInRange = null;

		//Set a Vector 2 to have an X at the center of the Player, and Y at the highest Y value
		Vector2 pointA = new Vector2(collider.bounds.center.x, collider.bounds.max.y);

		//Set a new Vector 2
		Vector2 pointB = new Vector2();

		//Set a float equal to the distance between the maximum X and minimum X of the Player
		float distanceX = collider.bounds.max.x - collider.bounds.min.x;

		//If the Player is facing left, set a point with an X equal to 
		if (collisions.faceDir == -1) {
			pointB = new Vector2 (collider.bounds.min.x - distanceX, collider.bounds.min.y);
		} else if (collisions.faceDir == 1) {
			pointB = new Vector2 (collider.bounds.max.x + distanceX, collider.bounds.min.y);
		}

		Collider2D[] test = Physics2D.OverlapAreaAll (pointA, pointB, -1);

		for (int i = 0; i < test.Length; i++) {
			//TODO -- handle getting the closest weapon to equip
			if (test[i].transform.gameObject.GetComponent<WeaponController>() != null)
				weaponInRange = test[i].transform.gameObject.GetComponent<WeaponController>();
		}

		//Testing
		print ("Point A: x " + pointA.x.ToString() + " y " + pointA.y.ToString() + " || Point B: x " + pointB.x.ToString() + " y " + pointB.y.ToString());
		Debug.DrawLine (new Vector3 (pointA.x, pointA.y, 0), new Vector3 (pointA.x, pointB.y, 0), Color.red, 100f);
		Debug.DrawLine (new Vector3 (pointA.x, pointA.y, 0), new Vector3 (pointB.x, pointA.y, 0), Color.red, 100f);
		Debug.DrawLine (new Vector3 (pointB.x, pointA.y, 0), new Vector3 (pointB.x, pointB.y, 0), Color.red, 100f);
		Debug.DrawLine (new Vector3 (pointA.x, pointB.y, 0), new Vector3 (pointB.x, pointB.y, 0), Color.red, 100f);


		return weaponInRange;


	}

	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		if (Mathf.Abs(velocity.x) < skinWidth) {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);
			
			if (hit) {
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}
	
	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;
		
		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);
			
			if (hit) {
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}
	
	void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);
		
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	
	void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);
		
		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);
		
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	
	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		public bool weapon;

		public int faceDir;

		public void Reset(){
			above = false;
			below = false;
			left = false;
			right = false;
			weapon = false;
		}
	}
	
}