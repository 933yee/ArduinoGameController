using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class T : MonoBehaviour
{
    public Vector3 rotationPoint;
    public ParticleSystem deleteEffect;
    public static int height = 20, width = 10;
    public GameObject shadow;
    private GameObject[] shadowBlocks = new GameObject[4];
    public GameObject gameManager;


    void Update()
    {
        int h = calculateShadowPos(Mathf.RoundToInt(transform.position.y));
        // Debug.Log(calculateShadowPos(Mathf.RoundToInt(transform.position.y)));
        shadowDestroy();
        shadowGenerate(h);
    }

    public void moveToBottom()
    {
        int cnt = 0;
        foreach (Transform children in transform)
        {
            children.position = shadowBlocks[cnt++].transform.position;
        }
    }
    public void shadowDestroy()
    {
        for (int i = 0; i < 4; i++)
        {
            if (shadowBlocks[i] != null)
                Destroy(shadowBlocks[i]);
            // Debug.Log("destroy!");
        }
    }
    public void shadowGenerate(int h)
    {
        int cnt = 0;
        foreach (Transform children in transform)
        {
            int x = Mathf.RoundToInt(children.transform.position.x);
            int y = Mathf.RoundToInt(children.transform.position.y);
            bool flag = true;
            foreach (Transform children1 in transform)
            {
                int x1 = Mathf.RoundToInt(children1.transform.position.x);
                int y1 = Mathf.RoundToInt(children1.transform.position.y);
                if (y1 == y - h && x1 == x)
                {
                    flag = false;
                    break;
                }
            }
            shadowBlocks[cnt] = Instantiate(shadow, new Vector3(x, y - h, 0), Quaternion.identity);
            if (!flag) shadowBlocks[cnt].SetActive(false);
            cnt++;
        }
    }

    public int calculateShadowPos(int h)
    {
        for (int i = 0; i <= h + 1; i++)
        {
            foreach (Transform children in transform)
            {
                int x = Mathf.RoundToInt(children.transform.position.x);
                int y = Mathf.RoundToInt(children.transform.position.y);
                if (y - i < 0) return y;
                if (FindObjectOfType<Control>().grid[x, y - i] != null) return i - 1;
            }
        }
        return 0;
    }
    public bool validMove()
    {
        foreach (Transform children in transform)
        {
            int x = Mathf.RoundToInt(children.transform.position.x);
            int y = Mathf.RoundToInt(children.transform.position.y);
            if (x < 0 || x >= width || y < 0) return false;

            if (FindObjectOfType<Control>().grid[x, y] != null) return false;
        }
        return true;
    }
}
