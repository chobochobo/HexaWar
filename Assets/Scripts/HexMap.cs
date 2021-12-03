using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

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

    //六角形のサイズ
    private float Hex_Height;
    private float Hex_Adjust;
    private float Hex_Width = 1.0f;


    private bool isStart;
    private List<GameObject> deleteList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        CreateHex();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateHex()
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
                hexArray[i, j] = MS;

                //HEXを生成
                GameObject hex = Instantiate(hexPrefabs);
                hex.transform.position = Cal_HexPosToViewLocalPos(vec2);

                //デバッグ用のテキストを生成
                GameObject DTex = Instantiate(DebugText);
                DTex.transform.SetParent(canvas.transform, false);
                DTex.GetComponent<Text>().text = i + ":" + j;
                DTex.transform.position = Cal_HexPosToViewLocalPos(vec2);
            }
        }
       // CheckStartset();
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
    //キャンディを操作できないようにする。
    public void StopCandies()
    {
        foreach (var item in hexArray)
        {
            item.GetComponent<MSMove>().isMoving = true;
        }
    }
    /*
    void CheckStartset()
    {
        //下の行からヨコのつながりを確認
        for (int i = 0; i < height; i++)
        {
            //右から２つ目以降は確認不要（width-2）
            for (int j = 0; j < width - 2; j++)
            {
                //同じタグのキャンディが３つ並んでいたら。Ｘ座標がｊなので注意。
                //念のため、ふたつの式それぞれをカッコで囲んでいる。
                if ((hexArray[j, i].tag == hexArray[j + 1, i].tag) && (hexArray[j, i].tag == hexArray[j + 2, i].tag))
                {
                    //CandyのisMatchingをtrueに//CandyのisMatchingをtrueに

                    hexArray[j, i].GetComponent<MSMove>().isMatching = true;
                    hexArray[j + 1, i].GetComponent<MSMove>().isMatching = true;
                    hexArray[j + 2, i].GetComponent<MSMove>().isMatching = true;
                }

            }

        }//
        //左の列からタテのつながりを確認
        for (int i = 0; i < width; i++)
        {
            //上から２つ目以降は確認不要。height-2
            for (int j = 0; j < height - 2; j++)
            {
                //Ｙ座標がｊ。
                if ((hexArray[i, j].tag == hexArray[i, j + 1].tag) && (hexArray[i, j].tag == hexArray[i, j + 2].tag))
                {
                    hexArray[i, j].GetComponent<MSMove>().isMatching = true;
                    hexArray[i, j + 1].GetComponent<MSMove>().isMatching = true;
                    hexArray[i, j + 2].GetComponent<MSMove>().isMatching = true;

                }
            }
        }
        //isMatching=trueのものをＬｉｓｔに入れる
        foreach (var item in hexArray)
        {
            if (item.GetComponent<MSMove>().isMatching)
            {
                deleteList.Add(item);
            }
        }
        //List内にキャンディがある場合
        if (deleteList.Count > 0)
        {
            //該当する配列をnullにして（内部管理）、キャンディを消去する（見た目）。
            foreach (var item in deleteList)
            {
                hexArray[(int)item.transform.position.x, (int)item.transform.position.y] = null;
                Destroy(item);
            }

            //Listを空っぽに。
            deleteList.Clear();
            //空欄に新しいキャンディを入れる。
            SpawnNewMS();
        }
        else//Listにキャンディがない場合。
        {
            //ゲーム開始。
            isStart = true;
        }
    }

    void SpawnNewMS()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (hexArray[i, j] == null)
                {
                    int r = Random.Range(0, 5);
                    var candy = Instantiate(MagicStone[r]);
                    //見た目の処理
                    candy.transform.position = Cal_HexPosToViewLocalPos(new Vector2(i, j + 0.3f));
                    //内部管理の処理
                    hexArray[i, j] = candy;
                }
            }
        }
        //まだゲーム開始してないときは３つ揃ってないか確認。
        if (isStart == false)
        {
            CheckStartset();
        }
        else　//isStart==trueのとき。
        {
            //新しい位置をmyPreviousPosに設定
            foreach (var item in hexArray)
            {
                item.GetComponent<MSMove>().myPreviousPos = Cal_HexPosToViewLocalPos(item.transform.position);
            }
            //続けざまに３つそろっているかどうか判定。
            Invoke("CheckMatching", 0.2f);

        }

    }

    public void CheckMatching()
    {
        //下の行からヨコのつながりを確認
        for (int i = 0; i < height; i++)
        {
            //右から２つ目以降は確認不要
            for (int j = 0; j < width - 2; j++)
            {
                //同じタグのキャンディが３つ並んでいたら。Ｘ座標がｊ。
                if ((hexArray[j, i].tag == hexArray[j + 1, i].tag) && (hexArray[j, i].tag == hexArray[j + 2, i].tag))
                {
                    //CandyのisMatchingをtrueに
                    hexArray[j, i].GetComponent<MSMove>().isMatching = true;
                    hexArray[j + 1, i].GetComponent<MSMove>().isMatching = true;
                    hexArray[j + 2, i].GetComponent<MSMove>().isMatching = true;
                }
            }
        }
        //左の列からタテのつながりを確認
        for (int i = 0; i < width; i++)
        {
            //上から２つ目以降は確認不要。
            for (int j = 0; j < height - 2; j++)
            {
                //Ｙ座標がｊ。
                if ((hexArray[i, j].tag == hexArray[i, j + 1].tag) && (hexArray[i, j].tag == hexArray[i, j + 2].tag))
                {
                    hexArray[i, j].GetComponent<MSMove>().isMatching = true;
                    hexArray[i, j + 1].GetComponent<MSMove>().isMatching = true;
                    hexArray[i, j + 2].GetComponent<MSMove>().isMatching = true;

                }

            }

        }
        //isMatching=trueのものをＬｉｓｔに入れる
        foreach (var item in hexArray)
        {
            if (item.GetComponent<MSMove>().isMatching)
            {
                //３つ以上そろったとき、キャンディを半透明にする。
                item.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                deleteList.Add(item);
            }
        }
        //List内にキャンディがある場合
        if (deleteList.Count > 0)
        {
            //キャンディを消去するとき、一瞬の間を持たせるためIvoke関数にする。
            Invoke("DeleteCandies", 0.2f);
        }
        //List内にキャンディがある場合
        if (deleteList.Count > 0)
        {
            Invoke("DeleteCandies", 0.2f);
        }
        else//Listにキャンディがない場合。
        {
            //いま位置交換したキャンディを元の位置に。
            foreach (var item in hexArray)
            {
                item.GetComponent<MSMove>().BackToPreviousPos();
            }
            //再びキャンディを操作できるようにする。
            Invoke("CanMoveCandies", 0.4f);
        }
    }
    void DeleteCandies()
    {
        //List内のキャンディを消去。かつ、その配列をnullに。
        foreach (var item in deleteList)
        {
            Destroy(item);
            hexArray[(int)item.transform.position.x, (int)item.transform.position.y] = null;
        }
        //Listを空っぽに。
        deleteList.Clear();
        //キャンディの落下を待って、空欄に新しいキャンディを入れる。
        Invoke("SpawnNewCandy", 1.2f);
    }

    
    void CanMoveCandies()
    {
        foreach (var item in hexArray)
        {
            item.GetComponent<MSMove>().isMoving = false;
        }
    }*/
}
