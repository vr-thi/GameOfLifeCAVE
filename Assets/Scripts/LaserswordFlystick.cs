using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cave;

public class LaserswordFlystick : CollisionSynchronization
{

    private GameObject flyStick;
    private GameObject lasersword;
    private Color[] colors;
    private int currColor;
    private bool initialized = false;

    public LaserswordFlystick()
        : base(new[] { Cave.EventType.OnTriggerEnter })
    {

    }

    // Use this for initialization
    void Start()
    {
        colors = new Color[] { Color.red, Color.blue, Color.cyan, Color.green, Color.magenta, Color.yellow, Color.white };
        lasersword = gameObject.transform.GetChild(0).gameObject;
        lasersword.GetComponent<Renderer>().material.SetColor("_Color", colors[currColor]);
        StartCoroutine(WaitForFlystick());
    }

    private IEnumerator WaitForFlystick()
    {
        while (flyStick == null)
        {
            flyStick = GameObject.Find("Flystick");
            yield return null;
        }
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            gameObject.transform.position = flyStick.transform.position;
            gameObject.transform.rotation = flyStick.transform.rotation;
            if (InputSynchronizer.GetKeyUp("flystick 1"))
            {
                currColor = (currColor + 1) % colors.Length;
                lasersword.GetComponent<Renderer>().material.SetColor("_Color", colors[currColor]);
            }
            if (InputSynchronizer.GetKeyUp("flystick 3"))
            {
                currColor = (currColor + colors.Length - 1) % colors.Length;
                lasersword.GetComponent<Renderer>().material.SetColor("_Color", colors[currColor]);
            }
        }
    }

    public Color GetColor()
    {
        return colors[currColor];
    }
}
