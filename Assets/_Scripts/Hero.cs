﻿using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	    static public Hero      S; // Singleton

	public float            gameRestartDelay = 2f;

	    // These fields control the movement of the ship
	    public float            speed = 30;
	    public float            rollMult = -45;
	    public float            pitchMult = 30;

	    // Ship status information
	[SerializeField] // instructs Unity to show in inspector even though private var
	private float            _shieldLevel = 1;            // Add the underscore! 

	// Weapon fields
	    public Weapon[]            weapons; 

	    public bool ____________________________;

		public Bounds           bounds;

	// Declare a new delegate type WeaponFireDelegate
	    public delegate void WeaponFireDelegate();
	    // Create a WeaponFireDelegate field named fireDelegate.
	    public WeaponFireDelegate fireDelegate; 

	    void Awake() {
		    S = this;  // Set the Singleton
			bounds = Utils.CombineBoundsOfChildren(this.gameObject); 
		 }

		void Start() {
			// Reset the weapons to start _Hero with 1 blaster
			ClearWeapons();
			weapons[0].SetType(WeaponType.blaster);		
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
				
			// Use the fireDelegate to fire Weapons
		        // First, make sure the Axis("Jump") button is pressed 
		        // Then ensure that fireDelegate isn't null to avoid an error
		        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {           // 1
			            fireDelegate();
			     } 
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
			} else if (go.tag == "PowerUp") {
				                // If the shield was triggerd by a PowerUp
				                AbsorbPowerUp(go);
				            } 
			else {
				                print("Triggered: "+go.name);            // Move this line here!
				            }

		        } else {
			            // Otherwise announce the original other.gameObject
			            print("Triggered: "+other.gameObject.name); // Move this line here!
			        } 
	 } 

	public float shieldLevel {
		        get {
			            return( _shieldLevel );                                 // 1
			        }
		        set {
			            _shieldLevel = Mathf.Min( value, 4 );                   // 2
			            // If the shield is going to be set to less than zero
			            if (value < 0) {                                        // 3
				                Destroy(this.gameObject);
				// Tell Main.S to restart the game after a delay
				    Main.S.DelayedRestart( gameRestartDelay ); 
				            }
			        }
		    }

	public void AbsorbPowerUp( GameObject go ) {
		        PowerUp pu = go.GetComponent<PowerUp>();
		        switch (pu.type) {
		        case WeaponType.shield: // If it's the shield
			            shieldLevel++;
			            break;

		        default: // If it's any Weapon PowerUp
			            // Check the current weapon type
			            if (pu.type == weapons[0].type) {
				                // then increase the number of weapons of this type
				                Weapon w = GetEmptyWeaponSlot(); // Find an available weapon
				                if (w != null) {
					                    // Set it to pu.type
					                    w.SetType(pu.type);
					                }
			            } else {
				                // If this is a different weapon
				                ClearWeapons();
				                weapons[0].SetType(pu.type);
				            }
			            break;
		        }
		        pu.AbsorbedBy( this.gameObject );
		    }

	    Weapon GetEmptyWeaponSlot() {
		        for (int i=0; i<weapons.Length; i++) {
			            if ( weapons[i].type == WeaponType.none ) {
				                return( weapons[i] );
				            }
			        }
		        return( null );
		    }

	    void ClearWeapons() {
		        foreach (Weapon w in weapons) {
			            w.SetType(WeaponType.none);
			        }
		    } 
}