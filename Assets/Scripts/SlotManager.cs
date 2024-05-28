using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

public class SlotManager : MonoBehaviour
{
    // [SerializeField] private GameObject[] slotArrey; // 各スロットのゲームオブジェクトを入れる
    [SerializeField] private SpriteRenderer[] slotSprArrey; // 各スロットのスプライトレンダラーを入れる
    [SerializeField] private UnityEngine.Sprite[] sprArrey; // スロットの描画スプライトを入れる(1 ~ 9, ballなど)
    private int[] slotNumArrey = new int[] {0, 0, 0}; // 内部のスロット情報 0から順に左、右、真ん中
    private int[] lastSlotNumArrey = new int[] {0, 0, 0}; // 前回のスロットの数字を記憶する(スプライトの張り替えに使う)
    public static int slotStock; // スロットのストック数
    private bool isSlot = false; // スロットが動いているかどうか
    private int[] slotStopTimeArrey = new int[] {SLOTFIRSTSTOPTIME, SLOTSECONDSTOPTIME, SLOTLASTSTOPTIME}; // スロットの停止時間をfor文で使うために配列にしておく

    /* スロットのリールをいつ停止させるか */
    const int SLOTFIRSTSTOPTIME = 200;
    const int SLOTSECONDSTOPTIME = 300;
    const int SLOTLASTSTOPTIME = 400;
    const int SLOTSPANTIME = 10; // スロットを何msごとに動かすか
    const int SLOTKINDS = 10; // スロットの数値の種類(現在1 ~ 9, ball)
    // Start is called before the first frame update
    void Start()
    {
        /* 最初は0(スロットの1)で初期化 */
        for(int i = 0; i < 3; i++)
        {
            // slotSprArrey[i] = slotArrey[i].GetComponent<SpriteRenderer>(); // スプライトレンダラーを取得
            slotNumArrey[i] = 0;
            lastSlotNumArrey[i] = 0;
            slotSprArrey[i].sprite = sprArrey[0]; // スプライトを変更
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isSlot != true) // スロットが動いていない
        {
            if(slotStock >= 1) // ストックが1以上なら
            {
                slotStock -= 1;
                Debug.Log("現在のストック数:" + slotStock);
                isSlot = true;
                SlotStart(); // スロットスタート
            }
        }
    }

    public void SlotStart()
    {
        isSlot = true;
        /* スロットの中身をランダムに変更 完全ランダム*/
        slotNumArrey[0] = UnityEngine.Random.Range(0, 10); // 0 ~ 8は数字に対応、9はボール
        slotNumArrey[1] = UnityEngine.Random.Range(0, 10);
        slotNumArrey[2] = UnityEngine.Random.Range(0, 10);

        for(int i = 0; i < 3; i++)
        {
            /* スロットのコルーチン */
            StartCoroutine(SpinCoroutine(slotStopTimeArrey[i], i));
        }
        
        
        /* スロットの数値がすべて一致したら、あたり */
        if(slotNumArrey[0] == slotNumArrey[1] && slotNumArrey[1] == slotNumArrey[2])
        {
            Debug.Log(slotNumArrey[0] + "が揃いました");
        }
        //Debug.Log("外見" + lastSlotNumArrey[0]);
        //Debug.Log("中身" + slotNumArrey[0]);
        isSlot = false;
    }

    public void SlotStockPlus()
    {
        slotStock++;
        Debug.Log("現在のストック数:" + slotStock);
    }

    /* スロットを動かすコルーチン */
    private IEnumerator SpinCoroutine(int spinms, int spinPlace)
    {
        /* 指定した時間(ms)スロットを動かす */
        for(int i = 0; i < spinms; i++)
        {
            if(i % SLOTSPANTIME == 0) // スロットを回す間隔になったら
            {
                lastSlotNumArrey[spinPlace]++;
                lastSlotNumArrey[spinPlace] %= SLOTKINDS;
                slotSprArrey[spinPlace].sprite = sprArrey[lastSlotNumArrey[spinPlace]];
            }
            yield return null;
        }

        //Debug.Log("外見" + lastSlotNumArrey[spinPlace]);
        //Debug.Log("中身" + slotNumArrey[spinPlace]);
        /* スロットの内部の数値と外見の数値が一致するまで、スプライトを張り替える */
        StartCoroutine(FixCoroutine(spinPlace));
    }
    
    /* スロットの外見と中身を一致させる関数 */
    private void FixSlotNum(int spinPlace)
    {
        /* 100回スロットを動かす */
        for(int i = 0; i < 100; i++)
        {
            lastSlotNumArrey[spinPlace]++;
            lastSlotNumArrey[spinPlace] %= SLOTKINDS;
            slotSprArrey[spinPlace].sprite = sprArrey[lastSlotNumArrey[spinPlace]];
            //Debug.Log("外見" + lastSlotNumArrey[spinPlace]);
            //Debug.Log("中身" + slotNumArrey[spinPlace]);
            /* スロットの数値の同期が完了したら関数終了 */
            if(lastSlotNumArrey[spinPlace] == slotNumArrey[spinPlace])
            {
                break;
            }
        }
    }

    /* スロットの外見と中身を一致させるコルーチン */
    private IEnumerator FixCoroutine(int spinPlace)
    {
        for(int i = 0; i < SLOTKINDS * SLOTSPANTIME; i++)
        {
            if(i % SLOTSPANTIME == 0) // スロットを回す間隔になったら
            {
                lastSlotNumArrey[spinPlace]++;
                lastSlotNumArrey[spinPlace] %= SLOTKINDS;
                slotSprArrey[spinPlace].sprite = sprArrey[lastSlotNumArrey[spinPlace]];
            }
            /* スロットの数値の同期が完了したら関数終了 */
            if(lastSlotNumArrey[spinPlace] == slotNumArrey[spinPlace])
            {
                break;
            }
            yield return null;
        }
    }
}
