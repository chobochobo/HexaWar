using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MSSprite : MonoBehaviour
{

    public enum StoneColor
    {
        Red = 0,
        Yellow = 1,
        Blew = 2,
        Green = 3,
        Purple = 4
    }
    //元画像への参照
    public Sprite[] MSSperite;
    //現在の色
    public StoneColor nowStoneColor;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    //色を変更する関数
    public void ChangeSprite(int index)
    {
        nowStoneColor = (StoneColor)index;
        GetComponent<SpriteRenderer>().sprite = MSSperite[index];
    }



    public void ScaleAnime()
    {
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(0.8f, 1.0f).SetEase(Ease.OutBounce);
    }

    public void ChengeColor(float a)
    {
        GetComponent<SpriteRenderer>().color = new Color(a, a, a);
    }
}
