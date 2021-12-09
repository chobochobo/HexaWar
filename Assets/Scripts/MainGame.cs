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
                //全マスをCheck
                HexMapCS.MSCheck();
                //消すところがないか
                if (HexMapCS.Setflag())
                {
                    Debug.Log("Moveにいく");
                    //タッチできるように
                    Seen = SeenType.Move;
                }
                else
                {
                    //消して再度再生
                    HexMapCS.MSReincarnation();
                }
                

                break;
            case SeenType.Move:
                HexMapCS.MoveMagicStone();
                //動かし終わったらTouchReleaseに移動
                if (HexMapCS.GetGoodRelease())
                {
                    HexMapCS.SetGoodRelease(false);
                    Debug.Log("Moveにいく");
                    //タッチできるように
                    Seen = SeenType.TouchRelease;
                }

                break;
            case SeenType.TouchRelease:
                //全マスをCheck
                HexMapCS.MSCheck();
                //消すところがないか
                if (HexMapCS.Setflag())
                {
                    Debug.Log("Moveにいく");
                    //タッチできるように
                    Seen = SeenType.Move;
                }
                else
                {
                    //消して再度再生
                    HexMapCS.MSReincarnation();
                }

                break;
            case SeenType.DeleteProcess:
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
}
