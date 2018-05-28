using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cave;

public class ActivateCubes : CollisionSynchronization {
	private GameObject flyStick;
	private LaserswordFlystick laserswordFlystick;
	private GameOfLife gameOfLife;

	public ActivateCubes() : base(new[] {Cave.EventType.OnTriggerStay})
	{

	}

	// Use this for initialization
	void Start () {
		flyStick = GameObject.Find ("Flystick").gameObject;
		laserswordFlystick = (LaserswordFlystick) GameObject.Find ("Handle").gameObject.GetComponent ("LaserswordFlystick");
		GameObject cubeManager = GameObject.Find ("CubeManager");
		if (cubeManager != null) {
			gameOfLife = (GameOfLife)cubeManager.gameObject.GetComponent ("GameOfLife");
		}
	}

	void Update() {
	}

	private void Reset(){
	}

	public override void OnSynchronizedTriggerStay(GameObject other){
		if (other.name == "Lasersword") {
			if (flyStick == null) {
				flyStick = GameObject.Find ("Flystick").gameObject;
			} else {
				var forward = flyStick.transform.TransformDirection (Vector3.forward);
				// Debug.Log ("forward:" + forward);
				if (System.Math.Abs (forward.y) > 1e-4f) {
					float lambda = -flyStick.transform.position.y / forward.y;
					float x = flyStick.transform.position.x + lambda * forward.x;
					float z = flyStick.transform.position.z + lambda * forward.z;
					// Debug.Log ("x = " + x + ", z = " + z);
					if (gameOfLife != null && gameOfLife.enabled == true) {
						gameOfLife.activate (x, z, laserswordFlystick.getColor ());
					}
				}
			}
		}
	}
}
