using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSMove : MonoBehaviour
{

    //タッチしたら獲得する
    public bool TouchOn;
    //Touchの位置を確認する
    //移動前の座標
    public Vector2 myPreviousPos;


    void Start()
    {
        TouchOn = false;
        //スタート位置を記録する。
        myPreviousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void onClickAct()
    {

        Debug.Log("" + this.gameObject.name);
    }


    //指をおいたとき
    private void OnMouseDown()
    {
        Debug.Log("aa" + this.gameObject.name);
        TouchOn = true;
    }
    //マウスを動かしている時
    private void OnMouseDrag()
    {
    }
    //指を離したとき
    private void OnMouseUp()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("あたった" + this.gameObject.name);
        other.gameObject.transform.position = myPreviousPos;
    }


}

