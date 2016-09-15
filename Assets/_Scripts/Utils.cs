using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils : MonoBehaviour {

	//============================= Bounds Functions =============================\\

	    // Creates bounds that encapsulate the two Bounds passed in.
	    public static Bounds BoundsUnion( Bounds b0, Bounds b1 ) {
		        // If the size of one of the bounds is Vector3.zero, ignore that one
		        if ( b0.size == Vector3.zero && b1.size != Vector3.zero ) {         // 1
			            return( b1 );
		        } else if ( b0.size != Vector3.zero && b1.size == Vector3.zero ) {
			            return( b0 );
		        } else if ( b0.size == Vector3.zero && b1.size == Vector3.zero ) {
			            return( b0 );
			        }
		        // Stretch b0 to include the b1.min and b1.max
		        b0.Encapsulate(b1.min);                                             // 2
		        b0.Encapsulate(b1.max);
		        return( b0 );
		    }

	public static Bounds CombineBoundsOfChildren(GameObject go) {
		        // Create an empty Bounds b
		        Bounds b = new Bounds(Vector3.zero, Vector3.zero);
		        // If this GameObject has a Renderer Component...
		        if (go.GetComponent<Renderer>() != null) {
			            // Expand b to contain the Renderer's Bounds
			            b = BoundsUnion(b, go.GetComponent<Renderer>().bounds);
			        }
		        // If this GameObject has a Collider Component...
		        if (go.GetComponent<Collider>() != null) {
			            // Expand b to contain the Collider's Bounds
			            b = BoundsUnion(b, go.GetComponent<Collider>().bounds);
			        }
		        // Recursively iterate through each child of this gameObject.transform
		        foreach( Transform t in go.transform ) {                            // 1
			            // Expand b to contain their Bounds as well
			            b = BoundsUnion( b, CombineBoundsOfChildren( t.gameObject ) );  // 2
			        }

		        return( b );
		    }
}