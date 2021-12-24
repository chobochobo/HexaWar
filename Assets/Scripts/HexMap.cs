using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//画面内の描画を司る処理
public class HexMap : MonoBehaviour
{
    // デバッグテキスト
    public GameObject DebugText;
    public GameObject canvas;
    public GameObject MagicStone;
    public GameObject hexPrefabs;
    //配列の大きさを定義。
    private int width = 16;
    private int height = 12;

    //六角形の配列
    public GameObject[,] hexArray = new GameObject[16, 12];
    //魔石の配列
    public GameObject[,] MSArray = new GameObject[16, 12];

    //魔石の大きさ
    private float MS_Width = 0.8f;

    //六角形のサイズMS
    private float Hex_Height;
    private float Hex_Adjust;
    private float Hex_Width = 1.0f;

    //持てたかどうか
    private bool GetFlag;
    //持ってるオブジェクト
    public GameObject GetObject;
    //掴んだ色
    public int GetColor;
    //うまく離せたか
    public bool GoodRelease;

    //掴んだときの配列のID
    int[] GetID = new int[2];
    //掴んだときの場所
    public Vector3 GetPosition;
    //MSSprite呼び出し
    private MSSprite MSSpriteCS;
    //PUZZLE呼び出し
    private Puzzle PuzzleCS;


    //消せるのがあるかないか
    public bool SetCheck;

    //コンボカウンター
    public int ComboCounter;

    //Scorecount
    public int Scorecount;

    GameObject ScoreTex;
    // Start is called before the first frame update
    void Start()
    {
        //持っているゲームオブジェクトの初期化
        GetObject = new GameObject();
        //Getフラグを初期化
        GetFlag = false;
        //魔石の獲得
        MSSpriteCS = FindObjectOfType<MSSprite>();
        PuzzleCS = FindObjectOfType<Puzzle>();

        //セットフラグ
        SetCheck = false;

        GoodRelease = false;

        CreateHex();

        Scorecount = 0;

        //スコア表示
        ScoreTex = Instantiate(DebugText);
        ScoreTex.transform.SetParent(canvas.transform, false);
        ScoreTex.GetComponent<Text>().text = "Score:" + Scorecount;
        ScoreTex.transform.position = Cal_HexPosToViewLocalPos(new Vector2(2.0f, 9.0f));
        ScoreTex.transform.localScale= new Vector3(2.5f, 2.5f, 2.5f);

    }
    public float dt;
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
                //範囲確認
                if (PuzzleCS.GetOutFlag(i, j) == 0)
                {

                    //魔石をランダム生成
                    int r = Random.Range(0, 5);
                    //基礎ポジション
                    Vector2 vec2 = new Vector2(i * Hex_Width, j * Hex_Width);
                    //魔石を生成
                    GameObject MS = Instantiate(MagicStone);
                    MS.GetComponent<MSSprite>().ChangeSprite(r);
                    // MS.GetComponent<MSSprite>().ChengeColor(0f / 255f);
                    MS.transform.position = Cal_HexPosToViewLocalPos(vec2);
                    MS.name = i + ":" + j;
                    MSArray[i, j] = MS;
                    PuzzleCS.SetMapDate(i, j, r);
                    //HEXを生成
                    GameObject hex = Instantiate(hexPrefabs);
                    hex.transform.position = Cal_HexPosToViewLocalPos(vec2);
                    hex.name = i + ":" + j;
                    hexArray[i, j] = hex;

                    //DeleteDataを初期化
                    PuzzleCS.SetDeleteDate(i, j, 0);

                    //デバッグ用のテキストを生成

                    GameObject DTex = Instantiate(DebugText);
                    DTex.transform.SetParent(canvas.transform, false);
                    DTex.GetComponent<Text>().text = i + ":" + j;
                    DTex.transform.position = Cal_HexPosToViewLocalPos(vec2);

                }

            }
        }
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
                    //範囲確認
                    if (PuzzleCS.GetOutFlag(i, j) == 0)
                    {
                        if (PointCircleHitCheck(TouchPosition, MSArray[i, j], MS_Width))
                        {
                            //撮った場所の獲得
                            GetPosition = MSArray[i, j].transform.position;
                            //取ったところの画像を非表示に
                            MSArray[i, j].SetActive(false);
                            //持ってるやつの魔石を生成
                            GetObject = Instantiate(MagicStone);
                            GetObject.GetComponent<MSSprite>().ChangeSprite(PuzzleCS.GetMapDate(i, j));
                            //取ったところの色Dataを獲得
                            GetColor = PuzzleCS.GetMapDate(i, j);
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
                        //範囲確認
                        if (PuzzleCS.GetOutFlag(i, j) == 0)
                        {
                            //当たり判定
                            if (PointCircleHitCheck(TouchPosition, MSArray[i, j], MS_Width))
                            {
                                //色のDataを入れ替える
                                PuzzleCS.SetMapDate(GetID[0], GetID[1], PuzzleCS.GetMapDate(i, j));
                                PuzzleCS.SetMapDate(i, j, GetColor);
                                //画像を入れ替える
                                MSArray[i, j].GetComponent<MSSprite>().ChangeSprite(PuzzleCS.GetMapDate(i, j));
                                MSArray[GetID[0], GetID[1]].GetComponent<MSSprite>().ChangeSprite(PuzzleCS.GetMapDate(GetID[0], GetID[1]));
                                //消していたのを再度表示
                                MSArray[GetID[0], GetID[1]].SetActive(true);
                                //持っているのを消す
                                Destroy(GetObject);
                                Debug.Log(GetID[0] + ":" + GetID[1]);
                                //おいたよフラグ
                                GoodRelease = true;
                                //マスを消して再度再生
                                PuzzleCS.MatchMS(i, j);
                                PuzzleCS.MatchMS(GetID[0], GetID[1]);
                                //消して再度再生
                                StartCoroutine("wait");
                            }
                        }
                    }
                }
                //もとに戻す
                MSArray[GetID[0], GetID[1]].SetActive(true);
                //持っているのを消す
                Destroy(GetObject);
            }
            //離したよ
            GetFlag = false;
        }
    }

    //全部のマスで検索
    public void MSCheck()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //範囲確認
                if (PuzzleCS.GetOutFlag(i, j) == 0)
                {//範囲確認
                    PuzzleCS.MatchMS(i, j);
                }
            }
        }
    }


    //魔石を消して再構築
    public void MSReincarnation()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //範囲確認
                if (PuzzleCS.GetOutFlag(i, j) == 0)
                {
                    if (PuzzleCS.GetDeleteDate(i, j) == 1)
                    {
                        int r = Random.Range(0, 5);
                        MSArray[i, j].GetComponent<MSSprite>().ChangeSprite(r);
                        MSArray[i, j].GetComponent<MSSprite>().ScaleAnime();
                        // MSArray[i, j].SetActive(false);
                        PuzzleCS.SetMapDate(i, j, r);
                        PuzzleCS.SetDeleteDate(i, j, 0);

                        Scorecount++;
                        //Debug.Log(""+ Scorecount);
                        ScoreTex.GetComponent<Text>().text = "Score:" + Scorecount;
                    }
                }

            }
        }
    }

    //落とす処理
    public void DawnDraw()
    {

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (PuzzleCS.GetDeleteDate(i, j) == 1)
                {
                    //上のマスをひとマス落とす
                    for (int y = j; y < height; y++)
                    {
                        //一個下に落とす処理処理
                        PuzzleCS.SetMapDate(i, y - 1, PuzzleCS.GetMapDate(i, y));
                        MSArray[i, y - 1].GetComponent<MSSprite>().ChangeSprite(PuzzleCS.GetMapDate(i, y - 1));

                    }
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
                //範囲確認
                if (PuzzleCS.GetOutFlag(i, j) == 0)
                {
                    if (PuzzleCS.GetDeleteDate(i, j) == 1)
                    {
                        return SetCheck = false;
                    }
                }
            }
        }
        return SetCheck = true;
    }

    //ちゃんと離したか確認
    public bool GetGoodRelease()
    {
        return GoodRelease;
    }
    //グッドフラグをもとに戻す
    public void SetGoodRelease(bool set)
    {
        GoodRelease = set;
    }


    //六角形の位置を設定
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


    //円と点の当たり判定
    public bool PointCircleHitCheck(Vector2 point, GameObject circle, float Width)
    {
        float a = point.x - circle.transform.position.x;
        float b = point.y - circle.transform.position.y;
        float c = a * a + b * b;
        float d = Width / 2;
        if (c <= d * d)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //この中に送らせたい処理を描く
    IEnumerator wait()
    {
        MSReincarnation();
        //3秒停止
        yield return new WaitForSeconds(3);

    }
}
