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
    const int SLOTFIRSTSTOPTIME = 300;
    const int SLOTSECONDSTOPTIME = 400;
    const int SLOTLASTSTOPTIME = 500;
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
            StartCoroutine(SpinCoroutine(slotStopTimeArrey[i], i, () => 
            {
                /* スロットの内部の数値と外見の数値が一致するまで、スプライトを張り替える */
                StartCoroutine(FixCoroutine(i));
            }));
        }
        
        
        /* スロットの数値がすべて一致したら、あたり */
        if(slotNumArrey[0] == slotNumArrey[1] && slotNumArrey[1] == slotNumArrey[2])
        {
            Debug.Log(slotNumArrey[0] + "が揃いました");
        }
        isSlot = false;
    }

    public void SlotStockPlus()
    {
        slotStock++;
        Debug.Log("現在のストック数:" + slotStock);
    }

    /* スロットを動かすコルーチン */
    private IEnumerator SpinCoroutine(int spinms, int spinPlace, System.Action action)
    {
        /* 指定した時間(ms)スロットを動かす */
        for(int i = 0; i < spinms; i++)
        {
            lastSlotNumArrey[spinPlace]++;
            lastSlotNumArrey[spinPlace] %= SLOTKINDS;
            slotSprArrey[spinPlace].sprite = sprArrey[lastSlotNumArrey[spinPlace]];
            yield return null;
        }

        action?.Invoke(); // ?.はnull条件演算子 中身がnullでなければ実行
    }
    
    /*スロットの外見と中身を一致させるコルーチン */
    private IEnumerator FixCoroutine(int spinPlace)
    {
        /* 100回スロットを動かす */
        for(int i = 0; i < 100; i++)
        {
            lastSlotNumArrey[spinPlace]++;
            lastSlotNumArrey[spinPlace] %= SLOTKINDS;
            slotSprArrey[spinPlace].sprite = sprArrey[lastSlotNumArrey[spinPlace]];
            /* スロットの数値の同期が完了したらコルーチン終了 */
            if(lastSlotNumArrey[spinPlace] == slotNumArrey[spinPlace])
            {
                yield break;
            }
            yield return null;
        }
    }
}
