using System.Collections;   
using UnityEngine;
using Cave;

public class ActivateCubes : CollisionSynchronization
{
    [SerializeField]
    private GameOfLife gameOfLife;
    [SerializeField]
    private LaserswordFlystick laserswordFlystick;

    private GameObject flyStick;
    private bool initialized = false;

    public ActivateCubes() : base(new[] { Cave.EventType.OnTriggerStay })
    {

    }

    // Use this for initialization
    void Start()
    {
        if (gameOfLife == null || laserswordFlystick == null)
        {
            Debug.LogError("Assign variables in inspector before resuming");
        }
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

    public override void OnSynchronizedTriggerStay(GameObject other)
    {
        if (initialized && other.name == "Lasersword")
        {
            var forward = flyStick.transform.TransformDirection(Vector3.forward);
            if (System.Math.Abs(forward.y) > 1e-4f)
            {
                float lambda = -flyStick.transform.position.y / forward.y;
                float x = flyStick.transform.position.x + lambda * forward.x;
                float z = flyStick.transform.position.z + lambda * forward.z;
                if (gameOfLife != null && gameOfLife.enabled == true)
                {
                    gameOfLife.activate(x, z, laserswordFlystick.GetColor());
                }
            }
        }
    }
}
