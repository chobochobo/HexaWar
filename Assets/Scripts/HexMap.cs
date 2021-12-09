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
    public GameObject MagicStone;
    public GameObject hexPrefabs;
    //配列の大きさを定義。
    private int width = 12;
    private int height = 8;

    //六角形の配列
    public GameObject[,] hexArray = new GameObject[12, 8];
    //魔石の配列
    public GameObject[,] MSArray = new GameObject[12, 8];
    //魔石の色の配置
    public int[,] MAPData = new int[12, 8];
    //消えるID
    public int[,] DeleteDeta = new int[12, 8];
    public int ComboCounter;

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
    //掴んだ色
    public int GetColor;
    //うまく離せたか
    public bool GoodRelease;

    //掴んだときの配列のID
    int[] GetID = new int[2];
    //掴んだときの場所
    public Vector3 GetPosition;
    //消せるのがあるかないか
    public bool SetCheck;

    private MSSprite MSSpriteCS;


    // Start is called before the first frame update
    void Start()
    {
        //持っているゲームオブジェクトの初期化
        GetObject = new GameObject();
        //Getフラグを初期化
        GetFlag = false;
        //魔石の獲得
        MSSpriteCS = FindObjectOfType<MSSprite>();

        //コンボカウンターを初期化
        ComboCounter = 0;
        //セットフラグ
        SetCheck = false;

        GoodRelease = false;

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
           // Debug.Log("" + MSArray[1, 1].transform.position);
        }
    }
    //初期マップを生成
   public void CreateHex()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //魔石をランダム生成
                int r = Random.Range(0, 5);
                //基礎ポジション
                Vector2 vec2 = new Vector2(i * Hex_Width, j * Hex_Width);
                //魔石を生成
                GameObject MS = Instantiate(MagicStone);
                MS.GetComponent<MSSprite>().ChangeSprite(r);
                MS.transform.position = Cal_HexPosToViewLocalPos(vec2);
                MS.name = i + ":" + j;
                MSArray[i, j] = MS;
                MAPData[i, j] = r;
                //HEXを生成
                GameObject hex = Instantiate(hexPrefabs);
                hex.transform.position = Cal_HexPosToViewLocalPos(vec2);
                hex.name = i + ":" + j;
                hexArray[i, j] = hex;

                //DeleteDataを初期化
                DeleteDeta[i, j] = 0;

                //デバッグ用のテキストを生成
                
                GameObject DTex = Instantiate(DebugText);
                DTex.transform.SetParent(canvas.transform, false);
                DTex.GetComponent<Text>().text = i + ":" + j;
                DTex.transform.position = Cal_HexPosToViewLocalPos(vec2);
                
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
                        //撮った場所の獲得
                        GetPosition = MSArray[i, j].transform.position;
                        //取ったところの画像を非表示に
                        MSArray[i, j].SetActive(false);
                        //持ってるやつの魔石を生成
                        GetObject = Instantiate(MagicStone);
                        GetObject.GetComponent<MSSprite>().ChangeSprite( MAPData[i, j]);
                        //取ったところの色Dataを獲得
                        GetColor = MAPData[i, j];
                        //取った夜フラグ
                        GetFlag = true;
                        //取ったところのIDを格納
                        GetID[0] = i;
                        GetID[1] = j;
                        break;
                    }
                }
            }
        }
        else
        if (Input.GetMouseButton(0) && GetFlag)
        {
            //生成したオブジェクトをマウスのPositionに
            GetObject.transform.position = vec3;
        }
        else
        if (Input.GetMouseButtonUp(0))
        {
            //もってるいか
            if (GetFlag)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        //当たり判定
                        if (PointCircleHitCheck(TouchPosition, MSArray[i, j], MS_Width))
                        {
                            
                            //色のDataを入れ替える
                            MAPData[GetID[0], GetID[1]] = MAPData[i, j];
                            MAPData[i, j] = GetColor;
                            //画像を入れ替える
                            MSArray[i, j].GetComponent<MSSprite>().ChangeSprite(MAPData[i, j]);
                            MSArray[GetID[0], GetID[1]].GetComponent<MSSprite>().ChangeSprite(MAPData[GetID[0], GetID[1]]);
                            MSArray[GetID[0], GetID[1]].SetActive(true);
                            //持っているのを消す
                            Destroy(GetObject);
                            Debug.Log(GetID[0] + ":" + GetID[1]);
                            //おいたよフラグ
                            GoodRelease = true;
                            //マスを消して再度再生
                            MatchMS(i, j, MAPData[i, j]);
                            MatchMS(GetID[0], GetID[1], MAPData[GetID[0], GetID[1]]);
                            MSReincarnation();
                        }
                    }
                }
                //もとに戻す
                MSArray[GetID[0], GetID[1]].SetActive(true);
                //持っているのを消す
                Destroy(GetObject);
            }
            GetFlag = false;
        }
    }

    //同じ色判定
    public void MatchMS(int IDx,int IDy,int mapdeta)
    {
        int[,] DD = new int[,]
        {
        {1,1,1,2,0,1,1,2 },//右上奇数,偶数
        {1,0,2,0,1,0,2,0 },//右奇数,偶数
        {1,-1,1,-2,0,-1,1,-2 },//右下奇数,偶数
        {0,-1,-1,-2,-1,-1,-1,-2 },//左下奇数,偶数
        {-1,0,-2,0,0,-1,0,-2 },//左奇数,偶数
        {0,1,-1,2,-1,1,-1,2 }//左上奇数,奇数
        };

        for (int i = 0; i < 6; i++)
        {
            int a = (IDx + DD[i, 0]);
            int aa = (IDy + DD[i, 1]);
            int b = (IDx + DD[i, 2]);
            int bb = (IDy + DD[i, 3]);
            int c = (IDx + DD[i, 4]);
            int cc = (IDy + DD[i, 5]);
            int d = (IDx + DD[i, 6]);
            int dd = (IDy + DD[i, 7]);


            // Debug.Log(a + ":" + aa);
            if (IDy % 2 == 1)
            {
                //壁際判定
                if (a < width && aa < height &&
                   a > 0 && aa > 0)
                {
                    //同じ色かどうか判定
                    if (mapdeta == MAPData[a, aa])
                    {
                        //壁際判定
                        if (b < width && bb < height &&
                            b > 0 && bb > 0)
                        {
                            //同じ色かどうか判定
                            if (mapdeta == MAPData[b, bb])
                            {
                                //元データ
                                DeleteDeta[IDx, IDy] = 1;
                                //一個前削除設定
                                DeleteDeta[a, aa] = 1;
                                //削除設定
                                DeleteDeta[b, bb] = 1;
                            }
                        }
                    }
                }
            }
            else
            {
                //壁際判定
                if (c < width && cc < height &&
                    c > 0 && cc > 0)
                {
                    //同じ色かどうか判定
                    if (mapdeta == MAPData[c, cc])
                    {
                        //壁際判定
                        if (d < width && dd < height &&
                            d > 0 && dd > 0)
                        {
                            //同じ色かどうか判定
                            if (mapdeta == MAPData[d, dd])
                            {

                                //元データ
                                DeleteDeta[IDx, IDy] = 1;
                                //一個前削除設定
                                DeleteDeta[c, cc] = 1;
                                //削除設定
                                DeleteDeta[d, dd] = 1;
                            }
                        }
                    }

                }
            }
        }
    }


    public void MSReincarnation()
    {

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (DeleteDeta[i, j] == 1)
                {
                    int r = Random.Range(0, 5);
                    MSArray[i, j].GetComponent<MSSprite>().ChangeSprite(r);
                    MAPData[i, j] = r;
                    DeleteDeta[i, j] = 0;
                }

            }
        }
    }
    //消すところがあるかCheck
    public bool Setflag()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (DeleteDeta[i, j] == 1)
                {
                   return SetCheck = false;
                }
            }
        }
        return SetCheck = true;
    }

    //全部のマスで検索
    public void MSCheck()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                MatchMS(i, j, MAPData[i, j]);
            }
        }
    }



    public bool GetGoodRelease()
    {
        return GoodRelease;
    }

    public void SetGoodRelease(bool set)
    {
        GoodRelease = set;
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

}
