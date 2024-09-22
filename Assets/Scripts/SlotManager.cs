using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonConst;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

public class SlotManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] slotArrey; // 各スロットのスプライトレンダラーを入れる
    [SerializeField] UnityEngine.Sprite[] slotSprArrey; // スロットの描画スプライトを入れる(1 ~ 9, ballなど)
    [SerializeField] SpriteRenderer[] stockArrey; // スロットストックのスプライトレンダラーを入れる
    [SerializeField] UnityEngine.Sprite[] stockSprArrey; // ストックの描画スプライトを入れる(Lv1, Lv2, Lv3)
    [SerializeField] EventOrderManager eventOrderScript; // スロットが開始可能かのチェックにイベントフラグの状態を見る
    [SerializeField] FieldManager fieldScript; // ボールの個数を取得する
    [SerializeField] BallGenerate ballGenerateScript; // ボールを出現させる関数の呼び出しに使う
    [SerializeField] MedalGenerate medalGenerateScript; // メダルの払い出し枚数を増やすために使う
    [SerializeField] SoundController soundScript; // 音を鳴らすために使う
    [SerializeField] UIManager UIScript; // 得たものの表示に使う
    private int[] slotNumArrey = new int[] {0, 0, 0}; // 内部のスロット情報 0から順に左、右、真ん中
    private int[] lastSlotNumArrey = new int[] {0, 0, 0}; // 前回のスロットの数字を記憶する(スプライトの張り替えに使う)
    private static int slotStock; // スロットのストック数 外部からこの値が増やされたら、Update内で消費してストック配列のレベルをアップさせる

    private static int[] slotStockInsideArrey; // スロットのストック配列 上限個数は9, 上限レベルは3(1~3)
    private bool isSlot = false; // スロットが動いているかどうか
    private float[] slotStopTimeArrey = new float[] {SLOTFIRSTSTOPTIME, SLOTSECONDSTOPTIME, SLOTLASTSTOPTIME}; // スロットの停止時間をfor文で使うために配列にしておく

    const int SLOTSTOCKMAXLEVEL = 3; // スロットストックレベルの上限
    const int SLOTSTOCKCOUNTS = 9; // スロットストックの数

    /* スロットのリールをいつ停止させるか */
    const float SLOTFIRSTSTOPTIME = 1.5f; // 1.5秒後に停止
    const float SLOTSECONDSTOPTIME = 2.0f;
    const float SLOTLASTSTOPTIME = 2.5f;
    const int SLOTSPANTIME = 50; // スロットを何msごとに動かすか
    const int SLOTSPANLATETIME = 100; // スロットの終わり際(内部と外見の出目の調整フェーズ)で何msごとに動かすか 緩急をつけることができる
    const int SLOTKINDS = 10; // スロットの数値の種類(現在1 ~ 9, ball)
    const int WAIT = 1000; // 演出のためにわざと遅延させる

    /* 乱数抽選の確率 */
    const float SLOTADPERCENT = 5; // スロットの当選確率 これで当選しても7は揃わない
    const float BALLADPERCENT = 35; // フィールド上にボールが0個のとき、毎回この確率で抽選し、当選したらボールを排出する

    /* 払い出し枚数 */
    const int PAYOUTODD = 40; // 奇数が揃ったとき
    const int PAYOUTEVEN = 20; // 偶数が揃ったとき
    const int PAYOUTSEVEN = 100; // 7が揃ったとき 後々メダル排出ではなく、別の特典をつける

    /* スロットの出目を宣言 */
    /* ballは0, 1 ~ 9 は 1 ~ 9 */
    const int SLOT7 = 7; // 7だけは他の数字と処理が違うので、宣言しておく
    const int SLOTBALL = 0;
    // Start is called before the first frame update
    void Start()
    {
        /* stock配列を初期化 */
        slotStockInsideArrey = new int[SLOTSTOCKCOUNTS];
        for(int i = 0; i < SLOTSTOCKCOUNTS; i++)
        {
            slotStockInsideArrey[i] = 0;
        }
        SlotStockLookChange(); // スロットストックの描画を更新
        /* スロットの出目は1で初期化 */
        for(int i = 0; i < 3; i++)
        {
            slotNumArrey[i] = 1;
            lastSlotNumArrey[i] = 1;
            slotArrey[i].sprite = slotSprArrey[1]; // スプライトを変更
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool startJudge = CanStartSlot(); // スロットを回すかメソッドで判定
        if(startJudge == true)
        {
            int stockLevel = slotStockInsideArrey[0]; // 先頭のストックのレベルを取得
            for(int i = 0; i < SLOTSTOCKCOUNTS - 1; i++)
            {
                slotStockInsideArrey[i] = slotStockInsideArrey[i + 1]; // ストックを前詰めする
            }
            slotStockInsideArrey[SLOTSTOCKCOUNTS - 1] = 0; // 末尾のストックを0にする
            SlotStockLookChange(); // スロットストックの描画を更新
            // Debug.Log("現在のストック数:" + slotStock);
            SlotStart(stockLevel); // レベルを渡してスロットスタート
        }

        if(slotStock > 0) // スロットストックが1以上なら、ストック配列を更新
        {
            /* 配列の中で一番低いレベルと、その場所を前から探す */
            int minLv = SLOTSTOCKMAXLEVEL; // maxレベルを入れておく
            int minPlace = SLOTSTOCKCOUNTS; // 配列外の場所を入れておく
            for(int i = 0; i < SLOTSTOCKCOUNTS; i++)
            {
                if(slotStockInsideArrey[i] < minLv)
                {
                    minLv = slotStockInsideArrey[i]; // lv
                    minPlace = i; // 場所
                    // Debug.Log(minLv + " " + minPlace + "[SlotManager]");
                }
            }
            if(minPlace != SLOTSTOCKCOUNTS) // 場所が更新されたなら、ストックをレベルアップさせる
            {
                slotStockInsideArrey[minPlace]++; // レベルアップ
                SlotStockLookChange(); // 見た目更新
            }
            slotStock--; // 処理を行ったので、1個減らす
        }
    }

    public async void SlotStart(int level)
    {
        isSlot = true;
        soundScript.PlaySE(CommonConstManager.SLOTSTART);
        /* スロットの中身をランダムに変更 */
        for(int i = 0; i < 3; i++)
        {
            slotNumArrey[i] = UnityEngine.Random.Range(0, SLOTKINDS); // 0 ~ 8は数字に対応、9はボール
        }

        /* 操作部分 すでに揃っていたら無視 */
        if(!(slotNumArrey[0] == slotNumArrey[1] && slotNumArrey[1] == slotNumArrey[2]))
        {
            int randomNum; // 乱数をしまう変数
            /* フィールド上にボールがないなら、抽選する */
            if(fieldScript.FieldBallProperty == 0)
            {
                Debug.Log("ボール抽選開始[SlotManager]");
                randomNum = UnityEngine.Random.Range(1, 101); // 1 ~ 100まで
                if(randomNum <= BALLADPERCENT) // 成功したら内部の数字を変更 BALLADPERCENTが高いほど、成功確率は高くなる
                {
                    for(int i = 0; i < 3; i++)
                    {
                        slotNumArrey[i] = SLOTBALL;
                    }
                    Debug.Log("ボールが0個だったので内部で抽選を行い、成功しました[SlotManager]");
                }
            }
            /* 常にSLOTADPERCENT%の確率で7以外が揃う */
            randomNum = UnityEngine.Random.Range(1, 101); // 1 ~ 100まで
            if(randomNum <= SLOTADPERCENT) //成功したら内部の数字を7以外のなにかにランダムで変更
            {
                randomNum = UnityEngine.Random.Range(0, SLOTKINDS - 1); // 7を取り除くので -1 これによって、末尾の図柄が出なくなるので、それを7が出たとき割り当てる
                if(randomNum == SLOT7) // 7が選ばれてしまったら、
                {
                    randomNum = SLOTKINDS - 1; // 末尾の図柄に置き換える
                }
                for(int i = 0; i < 3; i++)
                {
                    slotNumArrey[i] = randomNum; // 出た図柄で揃える
                }
                Debug.Log(SLOTADPERCENT + "%抽選に成功したので、図柄を" + randomNum + "に揃えました[SlotManager]");
            }
        }

        /* スロットの非同期メソッド */
        Task TaskA = SpinAsync(slotStopTimeArrey[0], 0);
        Task TaskB = SpinAsync(slotStopTimeArrey[1], 1);
        Task TaskC = SpinAsync(slotStopTimeArrey[2], 2);

        await Task.WhenAll(TaskA, TaskB, TaskC); // 全タスクが終わるまで待機
        
        /* スロットの数値がすべて一致したら、あたり */
        if(slotNumArrey[0] == slotNumArrey[1] && slotNumArrey[1] == slotNumArrey[2])
        {
            int hitNum = slotNumArrey[0]; // 数字を変数に入れて、わかりやすくする
            Debug.Log(hitNum + "が揃いました[SlotManager]");

            /* 当選処理 */
            switch(hitNum)
            {
                case SLOTBALL: // BALLが揃ったら
                {
                    ballGenerateScript.PayoutNormalBallProperty++; // ノーマルボールの払い出し個数を+1する
                    Debug.Log("ボール当選[SlotManager]");
                    UIScript.SomethingDisplay("ノーマルボール獲得！"); // 得たものを表示
                    break;
                }
                case SLOT7: // 7が揃ったら 暫定処理としてメダルを払い出す
                {
                    Debug.Log("7が揃いました!![SlotManager]");
                    int payout = PAYOUTSEVEN * level; // payout枚数を計算 ストックレベルがそのまま倍率になる
                    medalGenerateScript.PayoutMedalProperty += payout; // 払い出し枚数に加算
                    UIScript.SomethingDisplay(payout + "枚獲得！"); // 得たものを表示
                    break;
                }
                default: // 数字が揃ったらメダル払い出し
                {
                    int payout; // 払い出し枚数 この後計算する
                    if(hitNum % 2 == 0) // 2で割ったあまりが0なら、偶数が揃った
                    {
                        Debug.Log("偶数が揃いました[SlotManager]");
                        payout = PAYOUTEVEN * level; // payout枚数を計算 ストックレベルがそのまま倍率になる
                    }
                    else // そうでないなら奇数
                    {
                        Debug.Log("奇数が揃いました![SlotManager]");
                        payout = PAYOUTODD * level; // payout枚数を計算 ストックレベルがそのまま倍率になる
                    }
                    medalGenerateScript.PayoutMedalProperty += payout; // 払い出し枚数に加算
                    UIScript.SomethingDisplay(payout + "枚獲得！"); // 得たものを表示
                    break;
                }
            }
            soundScript.PlaySE(CommonConstManager.SLOTSUCCESS);
        }

        await WaitTaskAsync(WAIT); // 連続しないように少し待機
        isSlot = false;
        await WaitTaskAsync(WAIT); // 他の処理(SCなど)との衝突を避けるために、フラグ処理後も遅延させる
    }

    /* スロットを動かす非同期メソッド */
    private async Task SpinAsync(float spinSecond, int spinPlace)
    {
        /* 指定した時間スロットを動かす */
        float currentTime = 0; // 現実世界でどのくらい時間が経ったかを入れる 端末スペックに関係なく決めた時間スロットを回すことが可能
        while(currentTime <= spinSecond)
        {
            lastSlotNumArrey[spinPlace]++;
            lastSlotNumArrey[spinPlace] %= SLOTKINDS; // 配列オーバーしないように種類数であまりをとる
            slotArrey[spinPlace].sprite = slotSprArrey[lastSlotNumArrey[spinPlace]];
            currentTime += Time.deltaTime + (SLOTSPANTIME / 1000f); // 現実世界での経過時間とスロット外見の切り替え時のディレイ(msなので1000fで割る 1000だとint / int = intになる)を経過時間に加算
            // Debug.Log(spinPlace + ": " + currentTime);
            await Task.Delay(SLOTSPANTIME); // SLOTSPANTIME分待機            
        }

        /* スロットの外見と中身を一致させる */
        while(lastSlotNumArrey[spinPlace] != slotNumArrey[spinPlace]) // 外見と中身が一致していない間
        {
            lastSlotNumArrey[spinPlace]++;
            lastSlotNumArrey[spinPlace] %= SLOTKINDS;
            slotArrey[spinPlace].sprite = slotSprArrey[lastSlotNumArrey[spinPlace]];
            //Debug.Log("外見" + lastSlotNumArrey[spinPlace]);
            //Debug.Log("中身" + slotNumArrey[spinPlace]);
            /* スロットの数値の同期が完了したら関数終了 */
            await Task.Delay(SLOTSPANLATETIME); // 待機
        }
        soundScript.PlaySE(CommonConstManager.REELSTOP); // リールストップの音を鳴らす
    }

    /* 指定した時間待機するタスク */
    private async Task WaitTaskAsync(int delayms)
    {
        await Task.Delay(delayms);
    }

    /* スロットを開始できるか判定 */
    private bool CanStartSlot()
    {
        /* 先頭のストックlvが1以上 & スロット中でない & SC中でないか */
        return (slotStockInsideArrey[0] >= 1 && isSlot != true && eventOrderScript.IsEventProperty != true); // 条件を満たしていればtrueを返す
    }

    /* スロットが動いているかを外部から確認するためのプロパティ */
    public bool IsSlotProperty
    {
        get
        {
            return isSlot;
        }
    }

    /* スロットストックを外部から操作するプロパティ 上限に達していたら増えない */
    public int SlotStockProperty
    {
        get
        {
            return slotStock;
        }
        set
        {
            if(value >= 0) // 正または0の値なら代入許可
            {
                slotStock = value;
            }
            Debug.Log("現在のストック数:" + slotStock);
        }
    }

    /* スロットストックの描画更新 */
    private void SlotStockLookChange()
    {
        for(int i = 0; i < SLOTSTOCKCOUNTS; i++)
        {
            /* ストックレベルで分岐 */
            switch(slotStockInsideArrey[i])
            {
                case 1: // 1lv
                stockArrey[i].sprite = stockSprArrey[0];
                stockArrey[i].enabled = true; // 表示する
                break;

                case 2: // 2lv
                stockArrey[i].sprite = stockSprArrey[1];
                stockArrey[i].enabled = true; // 表示する
                break;

                case 3: // 3lv
                stockArrey[i].sprite = stockSprArrey[2];
                stockArrey[i].enabled = true; // 表示する
                break;

                default: // それ以外なら、表示しない
                stockArrey[i].enabled = false;
                break;
            }
        }
    }
}
