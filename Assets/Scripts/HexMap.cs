using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

//画面内の描画を司る処理
public class HexMap : MonoBehaviour
{

    // デバッグテキスト
    public GameObject DebugText;
    public GameObject canvas;
    public GameObject[] MagicStone;
    public GameObject hexPrefabs;
    //配列の大きさを定義。
    private int width = 12;
    private int height = 8;
    public GameObject[,] hexArray = new GameObject[12, 8];

    //魔石の配列
    public GameObject[,] MSArray = new GameObject[12, 8];
    //魔石の大きさ
    private float MS_Width = 0.8f;

    //六角形のサイズMS
    private float Hex_Height;
    private float Hex_Adjust;
    private float Hex_Width = 1.0f;


    //持てたかどうか
    private bool GetFlag;
    //持ってるオブジェクト
    public GameObject GetObject ;

    //持ったときの場所
    public Vector3 GetPosition;

    private bool isStart;
    private List<GameObject> deleteList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //持っているゲームオブジェクトの初期化
        GetObject = new GameObject();
        //Getフラグを初期化
        GetFlag = false;
        CreateHex();
    }
   public float  dt;
    // Update is called once per frame
    void Update()
    {
        dt += Time.deltaTime;
        if (dt > 3)
        {
            dt = 0.0f;
            Debug.Log("" + MSArray[1, 1].transform.position);
        }
    }

   public void CreateHex()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                int r = Random.Range(0, 5);
                //基礎ポジション
                Vector2 vec2 = new Vector2(i * Hex_Width, j * Hex_Width);
                //魔石を生成
                GameObject MS = Instantiate(MagicStone[r]);
                MS.transform.position = Cal_HexPosToViewLocalPos(vec2);
                MS.name = i+":" + j;
                MSArray[i, j] = MS;

                //HEXを生成
                GameObject hex = Instantiate(hexPrefabs);
                hex.transform.position = Cal_HexPosToViewLocalPos(vec2);
                hex.name = i + ":" + j;

                //デバッグ用のテキストを生成
                /*
                GameObject DTex = Instantiate(DebugText);
                DTex.transform.SetParent(canvas.transform, false);
                DTex.GetComponent<Text>().text = i + ":" + j;
                DTex.transform.position = Cal_HexPosToViewLocalPos(vec2);
                */
            }
        }
    }

    //六角形のいちに調整
    public Vector3 Cal_HexPosToViewLocalPos(Vector2 hexPos)
    {
        // Y方向高さ
        Hex_Height = Hex_Width * Mathf.Sin(60.0f * Mathf.Deg2Rad);

        // X方向のずれ
        Hex_Adjust = Hex_Width * Mathf.Cos(60.0f * Mathf.Deg2Rad);

        float grid_X = Hex_Width * hexPos.x + Hex_Adjust * Mathf.Abs(hexPos.y % 2);
        float grid_Y = Hex_Height * hexPos.y;

        return new Vector3(grid_X, grid_Y, 0.0f);
    }



    //魔石を動かす
    public void MoveMagicStone()
    {
        //指の位置
        Vector2 TouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //掴んだものを一番手前にする
        Vector3 vec3 = new Vector3(TouchPosition.x, TouchPosition.y, -1);
        //持ってるゲームオブジェクト
        //押したかどうかの判定
        if (Input.GetMouseButtonDown(0) && GetFlag == false)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (PointCircleHitCheck(TouchPosition, MSArray[i, j], MS_Width))
                    {
                        GetPosition = MSArray[i, j].transform.position;
                        GetObject = MSArray[i, j];
                        Debug.Log(""+GetObject.name);
                        GetFlag = true;
                        break;
                    }
                }
            }
        }
        else
        if (Input.GetMouseButton(0) && GetFlag)
        {
            GetObject.transform.position = vec3;
        }
        else
        if (Input.GetMouseButtonUp(0))
        {
            if (GetFlag)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (PointCircleHitCheck(TouchPosition, MSArray[i, j], MS_Width))
                        {//入れ替え処理？
                            GetObject.transform.position = MSArray[i, j].transform.position;
                            MagicStoneChange(i, j, GetPosition);
                            break;
                        }
                    }
                }
            }
            GetFlag = false;
        }
    }
    //円と点の当たり判定
   public bool PointCircleHitCheck(Vector2 point,GameObject circle,float Width)
    {
        float a = point.x - circle.transform.position.x;
        float b = point.y - circle.transform.position.y;
        float c = a * a + b * b;
        float d = Width / 2;
        if (c <= d * d)
        {
            return true;
        }else
        {
            return false;
        }
    }

    //入れ替え処理
    public void MagicStoneChange(int i, int j, Vector2 vec2)
    {
        MSArray[i, j].transform.position = vec2;
    }

}
