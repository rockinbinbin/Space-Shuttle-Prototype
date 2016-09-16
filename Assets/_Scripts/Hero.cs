using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	    static public Hero      S; // Singleton

	    // These fields control the movement of the ship
	    public float            speed = 30;
	    public float            rollMult = -45;
	    public float            pitchMult = 30;

	    // Ship status information
	    public float            shieldLevel = 1;

	    public bool ____________________________;

		public Bounds           bounds;

	    void Awake() {
		    S = this;  // Set the Singleton
			bounds = Utils.CombineBoundsOfChildren(this.gameObject);
		 }

	    void Update () {
		        // Pull in information from the Input class
		        float xAxis = Input.GetAxis("Horizontal");                          // 1
		        float yAxis = Input.GetAxis("Vertical");                            // 1

		        // Change transform.position based on the axes
		        Vector3 pos = transform.position;
		        pos.x += xAxis * speed * Time.deltaTime;
		        pos.y += yAxis * speed * Time.deltaTime;
		        transform.position = pos;

			bounds.center = transform.position;                                 // 1

		        // Keep the ship constrained to the screen bounds
		        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen); // 2
		        if ( off != Vector3.zero ) {                                        // 3
			            pos -= off;
			            transform.position = pos;
			        } 

		        // Rotate the ship to make it feel more dynamic                     // 2
		        transform.rotation = Quaternion.Euler(yAxis*pitchMult,xAxis*rollMult,0);
	 }

	// This variable holds a reference to the last triggering GameObject
	    public GameObject lastTriggerGo = null;                                // 1 

	void OnTriggerEnter(Collider other) {
		// Find the tag of other.gameObject or its parent GameObjects
		        GameObject go = Utils.FindTaggedParent(other.gameObject);
		        // If there is a parent with a tag
		        if (go != null) {
			            // Announce it
			            print("Triggered: "+go.name);
			// Make sure it's not the same triggering go as last time
			            if (go == lastTriggerGo) {                                      // 2
				                return;
				            }
			            lastTriggerGo = go;                                             // 3

			            if (go.tag == "Enemy") {
				                // If the shield was triggered by an enemy
				                // Decrease the level of the shield by 1
				                shieldLevel--;
				                // Destroy the enemy
				                Destroy(go);                                                // 4
			            } else {
				                print("Triggered: "+go.name);            // Move this line here!
				            }

		        } else {
			            // Otherwise announce the original other.gameObject
			            print("Triggered: "+other.gameObject.name); // Move this line here!
			        } 
	 } 
}