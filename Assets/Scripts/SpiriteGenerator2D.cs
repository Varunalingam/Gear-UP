using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpiriteGenerator2D : MonoBehaviour
{
    public SpriteShapeController StartObject;
    public SpriteShapeController EndObject;

    public SpriteShapeController EndOBjLE;

    public int Level;

    public List<Level> LevelData;

    List<int> RequireData;

    public List<MapProflie> MapProfile;


    List<GameObject> Generators;

    SpriteShapeController OverallController;

    public int Theme;

    public List<SpriteShape> Themes;

    public ParticleSystem Snow;

    public List<AILevel> AILevelData;

    public void Start()
    {
        Vector3 scale = Snow.shape.scale;
        float wSH = GetComponent<Camera>().orthographicSize * 2f;
        float wSW = wSH / Screen.height * Screen.width;
        scale.x = (Screen.width > Screen.height ? wSW / scale.x : wSH / scale.x);
        ParticleSystem.ShapeModule Shape = Snow.shape;
        Shape.scale = scale;
    }

    public void LevelGenerate()
    {
        int totalscenes = 0;
        

        if (GetComponent<GameController2D>().PlayMode == 1)
        {
            RequireData = AILevelData[Level].RequiredData;
            Theme = AILevelData[Level].Theme;
        }
        else
        {
            RequireData = LevelData[Level].RequiredData;
            Theme = LevelData[Level].Theme;
        }

        StartObject.gameObject.SetActive(true);
        StartObject.spriteShape = Themes[Theme];

        GameObject Abc = Instantiate(StartObject.gameObject);
        Abc.GetComponent<SpriteShapeController>().enabled = false;

        Generators = new List<GameObject>();

        if (Theme == 2)
        {
            Snow.Play();
        }

        for (int i = 0; i < RequireData.Count;i++)
        {
            totalscenes += RequireData[i];
        }

        Spline A;
        Vector3 Defalut;
        int sx;

        for (int i = 0; i < totalscenes;)
        {
            int j = Random.Range(0, RequireData.Count);
            if (RequireData[j] != 0)
            {
                int gen = Random.Range(0, MapProfile[j].Maps.Count);

                Generators.Add(Instantiate(MapProfile[j].Maps[gen]));
                RequireData[j] -= 1;
                Vector3 pos;
                if (i == 0)
                {
                    pos = StartObject.transform.position;
                    pos.x += StartObject.GetComponent<SpriteGeneratorHelper>().RightWidth + Generators[i].GetComponent<SpriteGeneratorHelper>().LeftWidth;
                    pos.y = StartObject.GetComponent<SpriteGeneratorHelper>().RightConnectorHeight - Generators[i].GetComponent<SpriteGeneratorHelper>().LeftConnectorHeight + StartObject.transform.position.y;
                    OverallController = StartObject;
                }
                else
                {
                    pos = Generators[i - 1].transform.position;
                    pos.x += Generators[i - 1].GetComponent<SpriteGeneratorHelper>().RightWidth + Generators[i].GetComponent<SpriteGeneratorHelper>().LeftWidth;
                    pos.y = Generators[i - 1].GetComponent<SpriteGeneratorHelper>().RightConnectorHeight - Generators[i].GetComponent<SpriteGeneratorHelper>().LeftConnectorHeight + Generators[i-1].transform.position.y;
                }

                Generators[i].transform.position = pos;

                Generators[i].GetComponent<Collider2D>().enabled = true;
                A = Generators[i].GetComponent<SpriteShapeController>().spline;
                Defalut = Generators[i].transform.position - OverallController.GetComponent<Transform>().position;

                OverallController.spline.SetPosition(OverallController.spline.GetPointCount() - 1, new Vector3((A.GetPosition(A.GetPointCount() - 1) + Defalut).x, (A.GetPosition(A.GetPointCount() - 1) + Defalut).y > OverallController.spline.GetPosition(0).y ? OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y : (A.GetPosition(A.GetPointCount() - 1) + Defalut).y));

                OverallController.spline.SetPosition(OverallController.spline.GetPointCount() - 2, A.GetPosition(A.GetPointCount() - 2) + Defalut);

                sx = OverallController.spline.GetPointCount() - 2;

                for (int s = 2; s < A.GetPointCount() - 2; s++)
                {
                    OverallController.spline.InsertPointAt(sx + s - 2, A.GetPosition(s) + Defalut);
                    OverallController.spline.SetTangentMode(sx + s - 2, A.GetTangentMode(s));

                    OverallController.spline.SetRightTangent(sx + s - 2, A.GetRightTangent(s));
                    OverallController.spline.SetSpriteIndex(sx + s - 2, A.GetSpriteIndex(s));
                    OverallController.spline.SetLeftTangent(sx + s - 2, A.GetLeftTangent(s));
                    OverallController.spline.SetCorner(sx + s - 2, A.GetCorner(s));
                    OverallController.spline.SetHeight(sx + s - 2, A.GetHeight(s));
                }
                Generators[i].GetComponent<SpriteShapeController>().enabled = false;

                i++;

                OverallController.spline.SetPosition(0, new Vector3(OverallController.spline.GetPosition(0).x, OverallController.spline.GetPosition(0).y > OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y ? OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y : OverallController.spline.GetPosition(0).y));
                OverallController.spline.SetPosition(OverallController.spline.GetPointCount() - 1, new Vector3(OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).x, OverallController.spline.GetPosition(0).y > OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y ? OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y : OverallController.spline.GetPosition(0).y));

            }
        }

        if (Level == LevelData.Count - 1 && GetComponent<GameController2D>().PlayMode != 1)
            EndObject = EndOBjLE;

        EndObject.gameObject.SetActive(true);
        Vector3 Epos = Generators[Generators.Count - 1].transform.position;
        Epos = Generators[Generators.Count - 1].transform.position;
        Epos.x += Generators[Generators.Count - 1].GetComponent<SpriteGeneratorHelper>().RightWidth + EndObject.GetComponent<SpriteGeneratorHelper>().LeftWidth;
        Epos.y = Generators[Generators.Count - 1].GetComponent<SpriteGeneratorHelper>().RightConnectorHeight - EndObject.GetComponent<SpriteGeneratorHelper>().LeftConnectorHeight + Generators[Generators.Count - 1].transform.position.y;
        EndObject.gameObject.transform.position = Epos;

        EndObject.GetComponent<Collider2D>().enabled = true;
        A = EndObject.GetComponent<SpriteShapeController>().spline;
        Defalut = EndObject.transform.position - OverallController.GetComponent<Transform>().position;

        OverallController.spline.SetPosition(OverallController.spline.GetPointCount() - 1, new Vector3((A.GetPosition(A.GetPointCount() - 1) + Defalut).x, (A.GetPosition(A.GetPointCount() - 1) + Defalut).y > OverallController.spline.GetPosition(0).y ? OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y : (A.GetPosition(A.GetPointCount() - 1) + Defalut).y));

        OverallController.spline.SetPosition(OverallController.spline.GetPointCount() - 2, A.GetPosition(A.GetPointCount() - 2) + Defalut);

        sx = OverallController.spline.GetPointCount() - 2;

        for (int s = 2; s < A.GetPointCount() - 2; s++)
        {
            OverallController.spline.InsertPointAt(sx + s - 2, A.GetPosition(s) + Defalut);
            OverallController.spline.SetTangentMode(sx + s - 2, A.GetTangentMode(s));

            OverallController.spline.SetRightTangent(sx + s - 2, A.GetRightTangent(s));
            OverallController.spline.SetSpriteIndex(sx + s - 2, A.GetSpriteIndex(s));
            OverallController.spline.SetLeftTangent(sx + s - 2, A.GetLeftTangent(s));
            OverallController.spline.SetCorner(sx + s - 2, A.GetCorner(s));
            OverallController.spline.SetHeight(sx + s - 2, A.GetHeight(s));
        }
        EndObject.GetComponent<SpriteShapeController>().enabled = false;

        OverallController.spline.SetPosition(0, new Vector3(OverallController.spline.GetPosition(0).x, OverallController.spline.GetPosition(0).y > OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y ? OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y : OverallController.spline.GetPosition(0).y));
        OverallController.spline.SetPosition(OverallController.spline.GetPointCount() - 1, new Vector3(OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).x, OverallController.spline.GetPosition(0).y > OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y ? OverallController.spline.GetPosition(OverallController.spline.GetPointCount() - 1).y : OverallController.spline.GetPosition(0).y));

        OverallController.GetComponent<Collider2D>().enabled = false;

    }
}

[System.Serializable]
public class MapProflie
{
    public List<GameObject> Maps;
}

[System.Serializable]
public class Level
{
    public string Name;
    public int Theme;
    public List<int> RequiredData;
}

[System.Serializable]
public class AILevel
{
    public string Name;
    public int Theme;
    public int AICar;
    public List<int> RequiredData;
}
