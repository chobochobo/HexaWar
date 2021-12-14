using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{

    //配列の大きさを定義。
    private int width = 12;
    private int height = 8;

    //魔石の色の配置
    public int[,] MAPData = new int[12, 8];
    //消えるID
    public int[,] DeleteData = new int[12, 8];

    //コンボカウンター
    public int ComboCounter;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetMapDate(int i,int j,int data)
    {
        MAPData[i, j] = data;
    }

    public int GetMapDate(int i,int j)
    {
        return MAPData[i, j];
    }

    public void SetDeleteDate(int i, int j, int data)
    {
        DeleteData[i, j] = data;
    }
    public int GetDeleteDate(int i, int j)
    {
        return DeleteData[i, j];
    }

    //同じ色判定
    public void MatchMS(int IDx, int IDy)
    {
        //各方角に検索、２マスだけ
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
            //同じ計算が多いので短略
            int a = (IDx + DD[i, 0]);
            int aa = (IDy + DD[i, 1]);
            int b = (IDx + DD[i, 2]);
            int bb = (IDy + DD[i, 3]);
            int c = (IDx + DD[i, 4]);
            int cc = (IDy + DD[i, 5]);
            int d = (IDx + DD[i, 6]);
            int dd = (IDy + DD[i, 7]);

            //奇数偶数判定
            if (IDy % 2 == 1)
            {
                //壁際判定
                if (a < width && aa < height &&
                   a > 0 && aa > 0)
                {
                    //同じ色かどうか判定
                    if (MAPData[IDx,IDy] == MAPData[a, aa])
                    {
                        //壁際判定
                        if (b < width && bb < height &&
                            b > 0 && bb > 0)
                        {
                            //同じ色かどうか判定
                            if (MAPData[IDx, IDy] == MAPData[b, bb])
                            {
                                //元データ
                                DeleteData[IDx, IDy] = 1;
                                //一個前削除設定
                                DeleteData[a, aa] = 1;
                                //削除設定
                                DeleteData[b, bb] = 1;
                            }
                        }
                    }
                }
            }
            else if(IDy % 2 == 0)
            {

                //壁際判定
                if (c < width && cc < height &&
                    c > 0 && cc > 0)
                {
                    //同じ色かどうか判定
                    if (MAPData[IDx, IDy] == MAPData[c, cc])
                    {
                        //壁際判定
                        if (d < width && dd < height &&
                            d > 0 && dd > 0)
                        {
                            //同じ色かどうか判定
                            if (MAPData[IDx,IDy] == MAPData[d, dd])
                            {
                                //元データ
                                DeleteData[IDx, IDy] = 1;
                                //一個前削除設定
                                DeleteData[c, cc] = 1;
                                //削除設定
                                DeleteData[d, dd] = 1;
                            }
                        }
                    }

                }
            }
        }
    }

}
