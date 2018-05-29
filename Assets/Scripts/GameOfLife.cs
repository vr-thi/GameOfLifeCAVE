using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cave;

public class GameOfLife : MonoBehaviour
{
    [SerializeField]
    private GameObject cube;
    [SerializeField]
    private float speed = 0.1f;
    [SerializeField]
    private Transform gamePlane;

    private const int N = 25;
private const int MAX_TIMESTEPS = 25;
    private GameObject[,,] cubearr = new GameObject[MAX_TIMESTEPS, N, N];
    private bool[,,] alive = new bool[MAX_TIMESTEPS, N, N];
    private float dynSpeed;
    private float cubeSize = 0.2f;
    private float centerTrans;
    private bool init = true;
    // the layer for which calculations will take place next
    private int active = 0;


    // Use this for initialization
    void Start()
    {
        Vector3 gameBounds = gamePlane.gameObject.GetComponent<MeshCollider>().bounds.size;
        if (gameBounds.x != gameBounds.z) {
            Debug.LogError("Plane for Game of Life needs to be quadratic");
        }
        cubeSize = (gameBounds.x) / N;

        centerTrans = cubeSize * (-0.5f * N + 0.5f);
        dynSpeed = speed;
        for (int timestep = 0; timestep < MAX_TIMESTEPS; timestep++)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    cubearr[timestep, i, j] = (GameObject)Instantiate(cube, new Vector3(discreteToContinousX(i), 0f, discreteToContinousZ(j)), Quaternion.identity);
                    cubearr[timestep, i, j].transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
                    cubearr[timestep, i, j].GetComponent<Renderer>().enabled = false;
                    alive[timestep, i, j] = false;
                }
            }
        }
    }

    private int getNeighbors(int level, int i, int j)
    {
        int neigh = 0;
        for (int k = i - 1; k <= i + 1; k++)
        {
            for (int l = j - 1; l <= j + 1; l++)
            {
                if ((k != i || l != j) && alive[level, (k + N) % N, (l + N) % N])
                {
                    neigh++;
                }
            }
        }
        return neigh;
    }

    private Color getAverageColor(int level, int i, int j)
    {
        float r = 0f;
        float g = 0f;
        float b = 0f;
        for (int k = i - 1; k <= i + 1; k++)
        {
            for (int l = j - 1; l <= j + 1; l++)
            {
                if ((k != i || l != j) && alive[level, (k + N) % N, (l + N) % N])
                {
                    Color cellColor = cubearr[level, (k + N) % N, (l + N) % N].GetComponent<Renderer>().material.GetColor("_Color");
                    r += cellColor.r;
                    g += cellColor.g;
                    b += cellColor.b;
                }
            }
        }
        return new Color(r / 3, g / 3, b / 3);
    }

    private float discreteToContinousX(int i)
    {
        return i * cubeSize + centerTrans + gamePlane.position.x; ;
    }

    private float discreteToContinousZ(int j)
    {
        return j * cubeSize + centerTrans + gamePlane.position.z;
    }

    private int continousToDiscreteX(float x)
    {
        return (int)((x - centerTrans - gamePlane.position.x) / cubeSize);
    }

    private int continousToDiscreteZ(float z)
    {
        return (int)((z - centerTrans - gamePlane.position.z) / cubeSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (init)
        {
            init = false;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (Random.value > 0.33333f)
                    {
                        cubearr[0, i, j].transform.position = new Vector3(discreteToContinousX(i), -0.5f * cubeSize, discreteToContinousZ(j));
                        cubearr[0, i, j].GetComponent<Renderer>().enabled = true;
                        alive[0, i, j] = true;
                    }
                }
            }
            return;
        }

        // First, lift all living GameObjects
        for (int t = 0; t < MAX_TIMESTEPS; t++)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (alive[t, i, j])
                    {
                        cubearr[t, i, j].transform.Translate(0, dynSpeed * TimeSynchronizer.deltaTime, 0);
                    }
                }
            }
        }

        // find distance to floor
        float distToFloor = 0f;
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (alive[active, i, j])
                {
                    distToFloor = cubearr[active, i, j].transform.position.y;
                    goto foundDist;
                }
            }
        }
    foundDist:

        // calculations for "new" plane
        if (distToFloor >= 0.5f * cubeSize)
        {
            dynSpeed = 0.5f * cubeSize / distToFloor * speed;

            int prev = active;
            active = (active + 1) % MAX_TIMESTEPS;
            float basey = distToFloor - cubeSize;

            // calculate active plane
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    cubearr[active, i, j].GetComponent<Renderer>().enabled = false;
                    int neigh = getNeighbors(prev, i, j);
                    if (neigh == 3 || neigh == 2 && alive[prev, i, j])
                    {
                        alive[active, i, j] = true;
                        cubearr[active, i, j].transform.position = new Vector3(discreteToContinousX(i), basey, discreteToContinousZ(j));
                        cubearr[active, i, j].GetComponent<Renderer>().enabled = true;
                        // set colors
                        if (alive[prev, i, j])
                        {
                            // set to color in cell of previous plane
                            cubearr[active, i, j].GetComponent<Renderer>().material.SetColor("_Color", cubearr[prev, i, j].GetComponent<Renderer>().material.GetColor("_Color"));
                        }
                        else
                        {
                            // take average color of those who have awoken cell to life
                            cubearr[active, i, j].GetComponent<Renderer>().material.SetColor("_Color", getAverageColor(prev, i, j));
                        }

                    }
                    else
                    {
                        alive[active, i, j] = false;
                    }
                }
            }
        }
    }

    public void activate(float x, float z, Color color)
    {
        int i = continousToDiscreteX(x);
        int j = continousToDiscreteZ(z);
        if (i >= 0 && i < N && j >= 0 && j < N)
        {
            if (!alive[active, i, j])
            {
                alive[active, i, j] = true;
                cubearr[active, i, j].GetComponent<Renderer>().enabled = true;
                cubearr[active, i, j].name = x + "," + z + "," + i + "," + j;
            }
            cubearr[active, i, j].GetComponent<Renderer>().material.SetColor("_Color", color);
        }
        
//        Debug.Log(x + " " + z + " " + i + " " + j);
    }
}
