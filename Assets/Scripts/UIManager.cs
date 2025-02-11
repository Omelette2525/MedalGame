using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using CommonConst;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerDataManager playerDataScript; // 持ちメダルなどを取得する
    [SerializeField] MedalGenerate medalGenerateScript; // 払い出しメダルを取得する
    [SerializeField] FieldManager fieldScript; // フィールドの状態を取得する
    [SerializeField] TMP_Text medalText; // 持ちメダルを表示するテキスト
    [SerializeField] TMP_Text payoutText; // 払い出しメダル
    [SerializeField] Slider supplyGauge; // 補給の様子を描画するスライダー
    [SerializeField] TMP_Text getSomethingText; // 何かを得たときに告知するテキスト
    [SerializeField] GameObject escapePanel; // Escキーを押したときに表示されるパネル
    [SerializeField] TMP_Text maxMedalText; // maxmedalを表示
    [SerializeField] TMP_Text shadowJpcMaxText; // shadowJpcの情報を表示

    /* Debug用text */
    [SerializeField] TMP_Text inMedalText;
    [SerializeField] TMP_Text outMedalText;
    [SerializeField] TMP_Text winMedalText;
    [SerializeField] TMP_Text outPerFieldPayoutText;
    [SerializeField] TMP_Text outPerInText;
    [SerializeField] TMP_Text fieldBallsText;

    private long currentMedal = 0; // 持ちメダルに変化があったか検出するための変数 とりあえず0を入れておく
    private long currentPayout = 0; // 払い出しメダル
    private long currentMaxMedal = 0; // maxmedal
    private int currentShadowJpcMaxWin = 0; // maxwin

    /* Debug用 */
    private long  currentInMedal = 0;
    private long currentOutMedal = 0;
    private long currentWinMedal = 0;
    private float currentOutPerFieldPayout = 0f;
    private float currentOutPerIn = 0f;
    private int currentFieldBalls = 0;
    

    private string medalFormat; // format形式で最初のテキストを読み込み、それに従って描画する
    private string payoutFormat;
    private string maxMedalFormat;
    private string shadowJpcMaxFormat;

    /* Debug用format */
    private string inMedalFormat;
    private string outMedalFormat;
    private string winMedalFormat;
    private string outPerFieldPayoutFormat;
    private string outPerInFormat;
    private string fieldBallsFormat;

    private float currentTime; // 時間をカウントする getSomethingTextの表示をコントロールするのに使う

    private const float DISPLAYTIME = 3f; // 情報を得たときに、どのくらい表示させるか

    // Start is called before the first frame update
    void Start()
    {
        /* format記憶 */
        medalFormat = medalText.text;
        payoutFormat = payoutText.text; 
        maxMedalFormat = maxMedalText.text;
        shadowJpcMaxFormat = shadowJpcMaxText.text;

        supplyGauge.maxValue = CommonConstManager.SUPPLYTIME / 1000; // ゲージの最大値を補給にかかる時間にしておく
        ShadowJpcMaxInfoUpdate(0, 0); // start時は0枚、0lv つまり1回も行っていないものとして表記をリセットする
        currentTime = DISPLAYTIME + 1; // 最初は表示させないためにDISPLAYTIMEより大きい値にしておく

        /* Debug用format */
        inMedalFormat = inMedalText.text;
        outMedalFormat = outMedalText.text;
        winMedalFormat = winMedalText.text;
        outPerFieldPayoutFormat = outPerFieldPayoutText.text;
        outPerInFormat = outPerInText.text;
        fieldBallsFormat = fieldBallsText.text;

        // SomethingDisplay("test"); // test
    }
    // Update is called once per frame
    void Update()
    {
        /* Escが押されたらメニューを開いたり閉じたりする */
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            escapePanel.SetActive(!escapePanel.activeSelf); // 自身の状態の反対にする
        }

        currentTime += Time.deltaTime; // 経過時間更新
        if(currentTime < DISPLAYTIME) // DISPLAYTIMEよりも経過時間が短いなら情報を表示する
        {
            getSomethingText.enabled = true;
        }
        else // そうでないなら非表示
        {
            getSomethingText.enabled = false;
        }


        /* 描画する情報を受け取る */
        long getMedal = playerDataScript.MedalProperty;
        long getPayout = medalGenerateScript.PayoutMedalProperty;
        long getMaxMedal = playerDataScript.MaxMedalProperty;
        float getSupplyGauge = playerDataScript.CurrentTimeProperty;
        int getShadowJpcMaxWin = playerDataScript.SJpcMaxWinProperty;
        /* Debug用 */
        long getInMedal = fieldScript.InMedalProperty;
        long getOutMedal = fieldScript.OutMedalProperty;
        long getWinMedal = fieldScript.WinMedalProperty;
        float getOutPerFieldPayout = fieldScript.OutPerFieldPayoutProperty;
        float getOutPerIn = fieldScript.OutPerInProperty;
        int getFieldBalls = fieldScript.FieldBallProperty;

        /* 描画更新 */
        ObserveInfo<long>(ref currentMedal, getMedal, medalText, medalFormat);
        ObserveInfo<long>(ref currentPayout, getPayout, payoutText, payoutFormat);
        ObserveInfo<long>(ref currentMaxMedal, getMaxMedal, maxMedalText, maxMedalFormat);

        /* payoutは0枚になったら非表示 */
        if(currentPayout == 0 && payoutText.enabled == true)
        {
            payoutText.enabled = false;
        }
        else if(currentPayout != 0 && payoutText.enabled == false)
        {
            payoutText.enabled = true;
        }

        /* 補給ゲージの描画更新 */
        /* ゲージ0の状態が続いたら消す */
        if(supplyGauge.value == 0 && getSupplyGauge == 0 && supplyGauge.gameObject.activeSelf == true)
        {
            supplyGauge.gameObject.SetActive(false);
        }
        /* 値が変わったら更新して表示 */
        if(supplyGauge.value != getSupplyGauge)
        {
            supplyGauge.value = getSupplyGauge;
            supplyGauge.gameObject.SetActive(true);
        }

        /* shadowJpcMaxの描画更新 */
        /* 値が変わったら更新して表示 */
        if(currentShadowJpcMaxWin != getShadowJpcMaxWin)
        {
            currentShadowJpcMaxWin = getShadowJpcMaxWin;
            ShadowJpcMaxInfoUpdate(currentShadowJpcMaxWin, playerDataScript.SJpcMaxLevelProperty);
        }

        /* Debug用 */
        ObserveInfo<long>(ref currentInMedal, getInMedal, inMedalText, inMedalFormat);
        ObserveInfo<long>(ref currentOutMedal, getOutMedal, outMedalText, outMedalFormat);
        ObserveInfo<long>(ref currentWinMedal, getWinMedal, winMedalText, winMedalFormat);
        ObserveInfo<float>(ref currentOutPerFieldPayout, getOutPerFieldPayout, outPerFieldPayoutText, outPerFieldPayoutFormat);
        ObserveInfo<float>(ref currentOutPerIn, getOutPerIn, outPerInText, outPerInFormat);
        ObserveInfo<int>(ref currentFieldBalls, getFieldBalls, fieldBallsText, fieldBallsFormat);
    }

    /* ジェネリック関数  IComparableインタフェースの実装を条件とする */
    void ObserveInfo<Type>(ref Type nowInfo, Type getInfo, TMP_Text outText, string format) where Type: IComparable // nowInfoは参照渡しにして、変更を検知したら更新できるようにする
    {
        if(nowInfo.CompareTo(getInfo) != 0) // 取得した情報と現在持っている情報が違うなら更新 ジェネリック関数は比較演算子を使えないので、IComparableというインターフェースが持つCompareToというメソッドを使う
        {
            nowInfo = getInfo; // 中身更新
            outText.text = string.Format(format, nowInfo); // テキストを今の情報に変更
        }
    }

    /* 1度も行われていない場合は表記が違う & 変数が2つなので、ジェネリック関数を使わない */
    public void ShadowJpcMaxInfoUpdate(int maxWin, int level)
    {
        /* levelが0なら、まだ一度もsjpcを行っていない */
        if(level == 0)
        {
            shadowJpcMaxText.text = string.Format(shadowJpcMaxFormat, "Nothing");
        }
        else // そうでないなら、情報を表示
        {
            shadowJpcMaxText.text = string.Format(shadowJpcMaxFormat, maxWin + " (Lv:" + level + ")");
        }
    }

    /* 要求された情報を表示する */
    public void SomethingDisplay(string str)
    {
        getSomethingText.text = str;
        currentTime = 0; // 経過時間リセット
    }

    /* ゲームを終了する */
    public void EndGame()
    {
        #if UNITY_EDITOR // エディター上での処理
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        #else // ビルド上での処理
        {
            Application.Quit();
        }
        #endif // ここでif文終了
    }
}
