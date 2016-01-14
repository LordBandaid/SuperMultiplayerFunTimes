using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	public Player heldBy;

	// Use this for initialization
	void Start () {
		heldBy = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (heldBy != null) {
			this.transform.position = heldBy.transform.position;
	
		}
	}

	public void Equipped(Player p)
	{
		this.heldBy = p;
		this.transform.position = p.transform.position;
	}

	public void Thrown()
	{
		heldBy = null;
		print ("Weapon X: " + this.transform.position.x + " || Weapon Y: " + this.transform.position.y);
	}
}
