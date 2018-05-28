using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowText : MonoBehaviour {
	public Text myText;
	private GameObject flyStick;


	// Use this for initialization
	void Start () {
		flyStick = GameObject.Find ("Flystick").gameObject;
	}

	void Update () {
		var forward = flyStick.transform.TransformDirection (Vector3.forward);
		float lambda = -flyStick.transform.position.y / forward.y;
		float x = flyStick.transform.position.x + lambda * forward.x;
		float z = flyStick.transform.position.z + lambda * forward.z;

		int N = 25;
		float cubeSize = 0.2f;
		float centerTrans  = cubeSize * (-0.5f * N + 0.5f);
		int i = (int) ((x - centerTrans) / cubeSize);
		int j = (int) ((z - centerTrans) / cubeSize);

		myText.text = "Position Flystick: " + flyStick.transform.position.ToString()
			+ " Orientation Flystick: " + flyStick.transform.rotation.ToString() 
			+ " forward: " + forward.ToString()
			+ " Hits ground: (" + x + ", " + z + ")"
			+ " Calculates coords: (" + i + ", " + j + ")";
	}
}