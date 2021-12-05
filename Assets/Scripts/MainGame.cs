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
    private MSMove MSMoveCS;

    // Start is called before the first frame update
    void Start()
    {
        HexMapCS = FindObjectOfType<HexMap>();
        MSMoveCS = FindObjectOfType<MSMove>();
    }

    // Update is called once per frame
    void Update()
    {
       switch(Seen)
        {
            case SeenType.FirstTime:
                Seen = SeenType.Move;
                Debug.Log("NoTouchにいく");
                break;
            case SeenType.Move:
                HexMapCS.MoveMagicStone();
                break;
            case SeenType.TouchRelease:
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
