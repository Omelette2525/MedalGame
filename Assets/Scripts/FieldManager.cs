using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    private long inMedal = 0; // プレイヤーがフィールドに投入したメダル数
    private long outMedal = 0; // プレイヤーがメダルを落として得たメダル数
    private long winMedal = 0; // ゲーム側がスロットなどで払い出したメダル数
    private long fieldPayout = 0; // inとwinの合計 pay outではなく、payout(支払い)のこと。
    private float outPerFieldPayout = 0; // フィールドに払い出された1メダルあたりプレイヤーが得たメダルの割合(ex. 2メダル払い出して1メダル得たら、1 / 2 = 0.5, 50%)
    private float outPerIn = 0; // プレイヤーが投入したメダル1枚あたりプレイヤーが得たメダルの割合(ex. 4メダル投入して1メダル得たら、1 / 4 = 0.25, 25%)
    private int fieldBall = 1; // フィールド上のボール数
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* 外部からアクセスするためのプロパティ ペイアウト率はgetのみ、ペイアウト率を計算するための情報(inMedalなど)はsetするために現在の値が必要なので、getもできるようにする */
    public long InMedalProperty
    {
        get
        {
            return inMedal;
        }
        set
        {
            if(value >= 0) // 正または0の値なら代入許可
            {
                inMedal = value;
                /* inMedalを使うので更新 */
                CalcFieldPayout();
                CalcOutPerIn();
            }
        }
    }
    public long OutMedalProperty
    {
        get
        {
            return outMedal;
        }
        set
        {
            if(value >= 0) // 正または0の値なら代入許可
            {
                outMedal = value;
                /* outMedalを使うので更新 */
                CalcOutPerFieldPayout();
                CalcOutPerIn();
            }
        }
    }
    public long WinMedalProperty
    {
        get
        {
            return winMedal;
        }
        set
        {
            if(value >= 0) // 正または0の値なら代入許可
            {
                winMedal = value;
                CalcFieldPayout(); // winMedalを使うので更新
            }
        }
    }

    public float FieldPayoutProperty
    {
        get
        {
            return fieldPayout;
        }
    }

    public float OutPerFieldPayoutProperty
    {
        get
        {
            return outPerFieldPayout;
        }
    }

    public float OutPerInProperty
    {
        get
        {
            return outPerIn;
        }
    }

    public int FieldBallProperty
    {
        get
        {
            return fieldBall;
        }
        set
        {
            if(value >= 0) // 正または0の値なら代入許可
            {
                fieldBall = value;
            }
        }
    }

    /* 情報が更新されたらその情報を使うペイアウト率なども更新する */
    private void CalcFieldPayout()
    {
        fieldPayout = inMedal + winMedal;
        CalcOutPerFieldPayout(); // fieldPayoutを使うので更新
    }
    private void CalcOutPerFieldPayout()
    {
        outPerFieldPayout = 100.0f * outMedal / fieldPayout; // %なので *100, 除算は最後
    }
    private void CalcOutPerIn()
    {
        outPerIn = 100.0f * outMedal / inMedal; // %なので *100, 除算は最後
    }
}
