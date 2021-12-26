using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//ゲームの基礎になる部分
public class MainGame : MonoBehaviour
{
   public enum SeenType
    {
        FirstTime,
        Move,
        TouchRelease,
        DeleteProcess,
        ReproductionProcess,
        SearchProcess
    }

    public  SeenType Seen = SeenType.FirstTime;
    private HexMap HexMapCS;

    // Start is called before the first frame update
    void Start()
    {
        HexMapCS = FindObjectOfType<HexMap>();
    }

    // Update is called once per frame
    void Update()
    {
       switch(Seen)
        {
            case SeenType.FirstTime:

                HexMapCS.MSCheck();
                //消すところがないか
                if (HexMapCS.Setflag())
                {
                    HexMapCS.ScoreInit();
                    Debug.Log("Moveにいく");
                    //タッチできるように
                    Seen = SeenType.Move;
                }
                else
                {
                    //消して再度再生
                    StartCoroutine("wait");
                }
                

                break;
            case SeenType.Move:
                HexMapCS.MoveMagicStone();
                //動かし終わったらTouchReleaseに移動
                if (HexMapCS.GetGoodRelease())
                {
                    HexMapCS.SetGoodRelease(false);
                    Debug.Log("TouchReleaseにいく");
                    //タッチできるように
                    Seen = SeenType.TouchRelease;
                }

                break;
            case SeenType.TouchRelease:

                //全マスをCheck
                HexMapCS.MSCheck();
                Seen = SeenType.DeleteProcess;
                Debug.Log("DeleteProcessにいく");

                break;
            case SeenType.DeleteProcess:
                if (HexMapCS.Setflag())
                {
                    Debug.Log("Moveにいく");
                    //タッチできるように
                    Seen = SeenType.Move;
                }
                else
                {
                    StartCoroutine("wait");

                    Debug.Log("TouchReleaseにいく");
                    Seen = SeenType.TouchRelease;
                }

                break;
            case SeenType.ReproductionProcess:
                break;
            case SeenType.SearchProcess:
                break;
            default:
                Debug.Log("デフォルトにいった");
                break;
        }


    }
    IEnumerator wait()
    {

        //消す処理
        HexMapCS.MSDelete();
        Debug.Log("消す処理");
        //3秒停止
        yield return new WaitForSeconds(0.1f);
        //消して再度再生
        HexMapCS.MSReincarnation();
        Debug.Log("再度再生");
        //3秒停止
        yield return new WaitForSeconds(0.3f);
        HexMapCS.MSReCheck();
        Debug.Log("再検索");
    }
}
