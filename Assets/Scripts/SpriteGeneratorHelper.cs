using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteGeneratorHelper : MonoBehaviour
{
    // Start is called before the first frame update

    public float LeftConnectorHeight;
    public float RightConnectorHeight;
    public float RightWidth,LeftWidth;

    public List<GameObject> Theme;

    void Awake()
    {
        SpriteShapeController a = gameObject.GetComponent<SpriteShapeController>();
        RightWidth = a.spline.GetPosition(a.spline.GetPointCount() - 1).x;
        LeftWidth = - a.spline.GetPosition(0).x;
        LeftConnectorHeight = a.spline.GetPosition(1).y;
        RightConnectorHeight = a.spline.GetPosition(a.spline.GetPointCount() - 2).y;

        for (int i = 0; i < Theme.Capacity;i++)
        {
            if (i == FindObjectOfType<SpiriteGenerator2D>().Theme)
            {
                Theme[i].SetActive(true);
            }
            else
            {
                Destroy(Theme[i]);
            }
        }

    }

}
