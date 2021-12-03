using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSMove : MonoBehaviour
{
    //GameControllerスクリプトを使うので、指定する。
    private HexMap HexMapCS;
    //自身の入っている配列の座標
    public int column;//列
    public int row;//行
    //スワイプしたときの座標を確認するための変数
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private Vector2 distance;
    //移動中はキャンディを操作できないように
    public bool isMoving;
    //隣のキャンディ
    private GameObject neighborCandy;
    //３つ並んでいるとき知らせる
    public bool isMatching;
    //移動前の座標
    public Vector2 myPreviousPos;
    void Start()
    {
        HexMapCS = FindObjectOfType<HexMap>();
        //自分の位置を座標配列の番号（Index)にあてておく。
        //スタート位置を記録する。
        myPreviousPos = transform.position;
    }
    //指をおいたとき
    private void OnMouseDown()
    {
        fingerDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //２点のベクトルの差を計算
        distance = fingerUp - fingerDown;
    }

    //指を離したとき
    private void OnMouseUp()
    {
        if (isMoving)
        {
            return;
        }
        fingerUp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        moveCandies();

    }
    void moveCandies()
    {

        //すべてのキャンディを操作できないようにする
        HexMapCS.StopCandies();
        //右にスワイプしていたなら。（Mathf.Absとは絶対値を示す）
        if (distance.x >= 0 && Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        {
            //自身が一番右にいない場合、となりのキャンディと位置を交換する
            if (column < 4)
            {
                //右隣りのキャンディ情報をneighborCandyに代入
                neighborCandy = HexMapCS.hexArray[column + 1, row];
                //隣のキャンディを１列左へ。
                neighborCandy.GetComponent<MSMove>().column -= 1;
                //自身は１列右へ。
                column += 1;
            }
        }

        //左にスワイプしていたなら。
        if (distance.x < 0 && Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        {
            //自身が一番左にいない場合、となりのキャンディと位置を交換する
            if (column > 0)
            {
                //左隣りのキャンディ情報を取得
                neighborCandy = HexMapCS.hexArray[column - 1, row];
                //隣のキャンディを１列右へ。
                neighborCandy.GetComponent<MSMove>().column += 1;
                //自身は１列左へ。
                column -= 1;
            }
        }

        //上にスワイプしていたなら。
        if (distance.y >= 0 && Mathf.Abs(distance.x) < Mathf.Abs(distance.y))
        {
            //自身が一番上にいない場合、となりのキャンディと位置を交換する
            if (row < 6)
            {
                //上のキャンディ情報を取得
                neighborCandy = HexMapCS.hexArray[column, row + 1];

                //隣のキャンディを１行下へ。
                neighborCandy.GetComponent<MSMove>().row -= 1;
                //自身は１行上へ。
                row += 1;
            }
        }

        //下にスワイプしていたなら。
        if (distance.y < 0 && Mathf.Abs(distance.x) < Mathf.Abs(distance.y))
        {
            //自身が一番下にいない場合、となりのキャンディと位置を交換する
            if (row > 0)
            {
                //下のキャンディ情報を取得
                neighborCandy = HexMapCS.hexArray[column, row - 1];

                //隣のキャンディを１行上へ。
                neighborCandy.GetComponent<MSMove>().row += 1;
                //自身は１行下へ。
                row -= 1;
            }
        }
        //0.5秒後にマッチしてるかチェック
        Invoke("DoCheckMatching", 0.5f);
    }
    /*
    //CandyArray配列に、自身を格納する。
    public void SetCandyToArray()
    {
        HexMapCS.hexArray[column, row] = gameObject;
    }

    void Update()
    {
        //現在の座標と、column、rowの値が異なるとき。
        if (transform.position.x != column || transform.position.y != row)
        {
            //column,rowの位置に徐々に移動する。
            transform.position = Vector2.Lerp(transform.position, new Vector2(column, row), 0.3f);
            //現在の位置と、目的地(column,row)との距離を測る。
            Vector2 dif = (Vector2)transform.position - new Vector2(column, row);
            //目的地との距離が0.1fより小さくなったら。
            if (Mathf.Abs(dif.magnitude) < 0.1f)
            {
                transform.position = HexMapCS.Cal_HexPosToViewLocalPos(new Vector2(column, row));
                //自身をCandyArray配列に格納する。
                SetCandyToArray();

            }
        } //自分が０行目（一番下）ではなく、かつ、下にキャンディがない場合、落下させる
        else if (row > 0 && HexMapCS.hexArray[column, row - 1] == null)
        {
            FallCandy();
        }
    }
    void FallCandy()
    {
        //自分のいた配列を空にする
        HexMapCS.hexArray[column, row] = null;
        //自分を下に移動させる
        row -= 1;
    }
    void DoCheckMatching()
    {
        HexMapCS.CheckMatching();

    }
    public void BackToPreviousPos()
    {
        column = (int)myPreviousPos.x;
        row = (int)myPreviousPos.y;
    }
    */
}

