﻿using UnityEngine;
using System.Collections;
using ParticlePlayground;

// Example of scripted preset emitting in a circle on instantiation.
// To instantiate this during runtime as a preset use: Playground.InstantiatePreset("Playground Circle Shot (Script)");

[ExecuteInEditMode()]
[RequireComponent(typeof(PlaygroundParticlesC))]
public class PlaygroundPresetCircleShotC : MonoBehaviour {

	public int numberOfParticles = 30;                   	// The number of particles to emit each cycle
	public float force = 10f;                           	// The force to emit in forward direction
	public int cycles = 1;									// The number of cycles to emit
	public Vector3 rotationNormal  = new Vector3(0,0,1);	// The axis you want to rotate around
	public float minimumLifetime = 3f;						// The minimum lifetime of a particle
	public float maximumLifetime = 3f;						// The maximum lifetime of a particle
	public Color color = Color.white;                    	// The color of particle
	public float yieldBeforeEmission = 0f;					// The seconds to wait before starting emission
	public float yieldBetweenShots = 0f;                 	// The seconds between shots (if any)
	public float yieldBetweenCycles = 0f;					// The seconds between cycles (if any)
	public WhenDoneCircleShot whenDone;						// Should this GameObject inactivate or destroy when emission is done?

	private Transform thisTransform;
	private PlaygroundParticlesC particles;

	bool isSameLifetime = false;

	void Start () {
		particles = GetComponent<PlaygroundParticlesC>();
		thisTransform = transform;
		isSameLifetime = minimumLifetime==maximumLifetime;
		StartCoroutine(Shoot());
	}

	IEnumerator Shoot () {

		// We need to wait before the particle system is fully ready before we start to call for our emission
		while (!particles.Initialized ())
			yield return null;

		// Set variables
		float rotationSpeed = 360f/numberOfParticles;
		float timeDone;
		
		// Set particle count to match the amount needed
		particles.particleCount = numberOfParticles*cycles;

		// Wait before emission starts (if applicable)
		if (yieldBeforeEmission>0) {
			timeDone = PlaygroundC.globalTime+yieldBeforeEmission;
			while (PlaygroundC.globalTime<timeDone)
				yield return null;
		}
		
		// Loop through every cycle (c) and particle (p)
		for (int c = 0; c<cycles; c++) {
		    for (int p = 0; p<numberOfParticles; p++) {
		    
		    	// Emit a particle in rotated direction
				particles.Emit(thisTransform.position, thisTransform.right*force, isSameLifetime? maximumLifetime : Random.Range(minimumLifetime, maximumLifetime), color);
		        
		        // Rotate towards direction
		        thisTransform.Rotate(rotationNormal*rotationSpeed);
		        
		        // Wait for next emission (if applicable)
		        if (yieldBetweenShots>0) {
		        	timeDone = PlaygroundC.globalTime+yieldBetweenShots;
		            while (PlaygroundC.globalTime<timeDone)
						yield return null;
		        }     
			}
			
			// Wait for next cycle (if applicable)
			if (yieldBetweenCycles>0) {
				timeDone = PlaygroundC.globalTime+yieldBetweenCycles;
		    	while (PlaygroundC.globalTime<timeDone)
					yield return null;
		    }
		}
		
		// Return if not in Play Mode in Editor
		#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
				yield break;
		#endif
		
		// Wait for action when last particle's lifetime is over
		switch (whenDone) {
			case WhenDoneCircleShot.Inactivate:
			yield return new WaitForSeconds(maximumLifetime);
				gameObject.SetActive(false);
			break;
			case WhenDoneCircleShot.Destroy:
			yield return new WaitForSeconds(maximumLifetime);
				Destroy(gameObject);
			break;
		}
	}

	public enum WhenDoneCircleShot {
		Nothing,
		Inactivate,
		Destroy,
	}

}