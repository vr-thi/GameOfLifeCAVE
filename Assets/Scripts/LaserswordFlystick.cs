using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cave;

public class LaserswordFlystick : CollisionSynchronization {

	public GameObject flyStick;
	private GameObject lasersword;
	private Color[] colors;
	private int currColor;

	public LaserswordFlystick()
		: base(new[] { Cave.EventType.OnTriggerEnter })
	{

	}

	// Use this for initialization
	void Start () {
		colors = new Color[] { Color.red, Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.white, Color.yellow };
		flyStick = GameObject.Find ("Flystick").gameObject;
		lasersword = this.gameObject.transform.GetChild (0).gameObject;
		lasersword.GetComponent<Renderer> ().material.SetColor("_Color", colors[currColor]);
	}
	
	// Update is called once per frame
	void Update () {
		if (flyStick == null) {
			flyStick = GameObject.Find ("Flystick").gameObject;
		} else {
			this.gameObject.transform.position = flyStick.transform.position;
			this.gameObject.transform.rotation = flyStick.transform.rotation;
			if (InputSynchronizer.GetKeyUp ("flystick 1")) {
				currColor = (currColor + 1) % colors.Length;
				lasersword.GetComponent<Renderer> ().material.SetColor("_Color", colors[currColor]);
			}
			if (InputSynchronizer.GetKeyUp ("flystick 3")) {
				currColor = (currColor + colors.Length - 1) % colors.Length;
				lasersword.GetComponent<Renderer> ().material.SetColor("_Color", colors[currColor]);
			}
		}
	}

	public Color getColor() {
		return colors [currColor];
	}
}
