using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] slotSprArrey; // 各スロットのスプライトレンダラーを入れる
    [SerializeField] private UnityEngine.Sprite[] sprArrey; // スロットの描画スプライトを入れる(1 ~ 9, ballなど)
    [SerializeField] private SCManager SCManagerScript;
    private int[] slotNumArrey = new int[] {0, 0, 0}; // 内部のスロット情報 0から順に左、右、真ん中
    private int[] lastSlotNumArrey = new int[] {0, 0, 0}; // 前回のスロットの数字を記憶する(スプライトの張り替えに使う)
    public static int slotStock; // スロットのストック数
    public bool isSlot = false; // スロットが動いているかどうか
    private float[] slotStopTimeArrey = new float[] {SLOTFIRSTSTOPTIME, SLOTSECONDSTOPTIME, SLOTLASTSTOPTIME}; // スロットの停止時間をfor文で使うために配列にしておく

    const int SLOTSTOCKMAX = 50; // スロットストックの上限

    /* スロットのリールをいつ停止させるか */
    const float SLOTFIRSTSTOPTIME = 1.5f; // 1.5秒後に停止
    const float SLOTSECONDSTOPTIME = 2.0f;
    const float SLOTLASTSTOPTIME = 2.5f;
    const int SLOTSPANTIME = 50; // スロットを何msごとに動かすか
    const int SLOTSPANLATETIME = 100; // スロットの終わり際(内部と外見の出目の調整フェーズ)で何msごとに動かすか 緩急をつけることができる
    const int SLOTKINDS = 10; // スロットの数値の種類(現在1 ~ 9, ball)
    const int WAIT = 1000; // 演出のためにわざと遅延させる
    /* 何回転か回したら、乱数抽選をするようにする。当選確率はだんだん上がっていく。乱数で当選した場合、7を除いた何かが必ず当たり、確率をリセット */
    const int SLOTADGETWHEEL = 2; // 当選確率が上がるために何回転必要か
    const float SLOTADPERCENT = 10; // 当選確率を1回で何パーセント上げるか
    // Start is called before the first frame update
    void Start()
    {
        /* 最初は0(スロットの1)で初期化 */
        for(int i = 0; i < 3; i++)
        {
            slotNumArrey[i] = 0;
            lastSlotNumArrey[i] = 0;
            slotSprArrey[i].sprite = sprArrey[0]; // スプライトを変更
        }
    }

    // Update is called once per frame
    void Update()
    {
        /* ストックがあって、スロット中、SC中でないならスロット開始 */
        if(slotStock >= 1)
        {
            if(isSlot != true && SCManagerScript.isSC != true)
            {
                slotStock -= 1;
                Debug.Log("現在のストック数:" + slotStock);
                SlotStart(); // スロットスタート
            }
            else if(SCManagerScript.isSC)
            {
                //Debug.Log("SC中なので、スロット拒否");
            }
        }
    }

    public async void SlotStart()
    {
        isSlot = true;
        /* スロットの中身をランダムに変更 完全ランダム*/
        slotNumArrey[0] = UnityEngine.Random.Range(0, 10); // 0 ~ 8は数字に対応、9はボール
        slotNumArrey[1] = UnityEngine.Random.Range(0, 10);
        slotNumArrey[2] = UnityEngine.Random.Range(0, 10);

        /* スロットの非同期メソッド */
        Task TaskA = SpinAsync(slotStopTimeArrey[0], 0);
        Task TaskB = SpinAsync(slotStopTimeArrey[1], 1);
        Task TaskC = SpinAsync(slotStopTimeArrey[2], 2);

        await Task.WhenAll(TaskA, TaskB, TaskC); // 全タスクが終わるまで待機
        
        /* スロットの数値がすべて一致したら、あたり */
        if(slotNumArrey[0] == slotNumArrey[1] && slotNumArrey[1] == slotNumArrey[2])
        {
            Debug.Log(slotNumArrey[0] + "が揃いました");
        }

        await WaitTaskAsync(WAIT); // 連続しないように少し待機
        isSlot = false;
        await WaitTaskAsync(WAIT); // 他の処理(SCなど)との衝突を避けるために、フラグ処理後も遅延させる
    }

    /* スロットを動かす非同期メソッド */
    private async Task SpinAsync(float spinSecond, int spinPlace)
    {
        /* 指定した時間スロットを動かす */
        float currentTime = 0; // 現実世界でどのくらい時間が経ったかを入れる　端末スペックに関係なく決めた時間スロットを回すことが可能
        while(currentTime <= spinSecond)
        {
            lastSlotNumArrey[spinPlace]++;
            lastSlotNumArrey[spinPlace] %= SLOTKINDS; // 配列オーバーしないように種類数であまりをとる
            slotSprArrey[spinPlace].sprite = sprArrey[lastSlotNumArrey[spinPlace]];
            currentTime += Time.deltaTime + (SLOTSPANTIME / 1000f); // 現実世界での経過時間とスロット外見の切り替え時のディレイ(msなので1000fで割る 1000だとint / int = intになる)を経過時間に加算
            Debug.Log(spinPlace + ": " + currentTime);
            await Task.Delay(SLOTSPANTIME); // SLOTSPANTIME分待機            
        }

        /* スロットの外見と中身を一致させる */
        while(lastSlotNumArrey[spinPlace] != slotNumArrey[spinPlace]) // 外見と中身が一致していない間
        {
            lastSlotNumArrey[spinPlace]++;
            lastSlotNumArrey[spinPlace] %= SLOTKINDS;
            slotSprArrey[spinPlace].sprite = sprArrey[lastSlotNumArrey[spinPlace]];
            //Debug.Log("外見" + lastSlotNumArrey[spinPlace]);
            //Debug.Log("中身" + slotNumArrey[spinPlace]);
            /* スロットの数値の同期が完了したら関数終了 */
            await Task.Delay(SLOTSPANLATETIME); // 待機
        }
    }

    /* スロットストックを増やすメソッド 上限に達していたら増えない */
    public void SlotStockPlus()
    {
        if(slotStock < SLOTSTOCKMAX) // 上限未満なら増やす
        {
            slotStock++;
        }
        Debug.Log("現在のストック数:" + slotStock);
    }

    /* 指定した時間待機するタスク */
    private async Task WaitTaskAsync(int delayms)
    {
        await Task.Delay(delayms);
    }
}
