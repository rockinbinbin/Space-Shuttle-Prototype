﻿using UnityEngine;
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
	// Make a static read-only public property camBounds
	    static public Bounds camBounds {                                        // 1
		        get {
			            // if _camBounds hasn't been set yet
			            if (_camBounds.size == Vector3.zero) {
				                // SetCameraBounds using the default Camera
				                SetCameraBounds();
				            }
			            return( _camBounds );
			        }
		    }
	    // This is the private static field that camBounds uses
	    static private Bounds _camBounds;                                       // 2”

	// This function is used by camBounds to set _camBounds and can also be
	    //  called directly.
	    public static void SetCameraBounds(Camera cam=null) {                   // 3
		        // If no Camera was passed in, use the main Camera
		        if (cam == null) cam = Camera.main;
		        // This makes a couple of important assumptions about the camera!:
		        //   1. The camera is Orthographic
		        //   2. The camera is at a rotation of R:[0,0,0]

		        // Make Vector3s at the topLeft and bottomRight of the Screen coords
		        Vector3 topLeft = new Vector3( 0, 0, 0 );
		        Vector3 bottomRight = new Vector3( Screen.width, Screen.height, 0 );

		        // Convert these to world coordinates
		        Vector3 boundTLN = cam.ScreenToWorldPoint( topLeft );
		        Vector3 boundBRF = cam.ScreenToWorldPoint( bottomRight );

		        // Adjust their zs to be at the near and far Camera clipping planes
		        boundTLN.z += cam.nearClipPlane;
		        boundBRF.z += cam.farClipPlane;

		        // Find the center of the Bounds
		        Vector3 center = (boundTLN + boundBRF)/2f;
		        _camBounds = new Bounds( center, Vector3.zero );
		        // Expand _camBounds to encapsulate the extents.
		        _camBounds.Encapsulate( boundTLN );
		        _camBounds.Encapsulate( boundBRF );
		    }

}