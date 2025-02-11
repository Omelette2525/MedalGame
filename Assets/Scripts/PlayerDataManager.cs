using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonConst;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerDataManager : MonoBehaviour
{
    private static long medal; // 所持メダル
    private static long maxMedal; // 所持メダルが一番多かったときの値
    private static int shadowJpcMax_Win; // shadowJpcで得た最高枚数
    private static int shadowJpcMax_Level; // shadowJpcで最高枚数を出したときのshadowJpclevel
    private bool isSupply = false; // 補給フラグ
    
    const int SUPPLYBOR = 100; // メダル補給するかどうかのボーダー
    const int SUPPLYMEDAL = 10; // 1回で補給する量
    const int FIRSTMEDAL = 100; // 持ちメダルの初期値
    private float currentTime; // 経過時間 スライダーの描画に使う
    // Start is called before the first frame update
    void Start()
    {
        /* データ読み込み */
        medal = SaveAndLoadManager.GetLong(CommonConstManager.MEDAL_P, FIRSTMEDAL); // 持ちメダル 初起動ならFIRSTMEDALの値をセットする
        maxMedal = SaveAndLoadManager.GetLong(CommonConstManager.MAXMEDAL_P, FIRSTMEDAL); // 最高持ちメダル 初起動ならFIRSTMEDAL
        shadowJpcMax_Win = SaveAndLoadManager.GetInt(CommonConstManager.SJPCMAX_WIN_P, 0); // sjpcでの最高獲得枚数 読み込めないなら0
        shadowJpcMax_Level = SaveAndLoadManager.GetInt(CommonConstManager.SJPCMAX_LEVEL_P, 0); // sjpcで最高獲得枚数を達成したときのlevel sjpcをしたことがなかったら0

        /* 初期化 */
        currentTime = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        bool supplyJudge = CanSupplyMedal(); // 補給可能か判定
        if(supplyJudge == true)
        {
            isSupply = true; // フラグを立てる
            var _ = MedalSupplyAsync(); // task(await)を利用しないことを明示(補給にディレイをかける工程でUpdateを止めたくない)
        }
        if(isSupply == true) // 補給中なら、経過時間をカウントし、ゲージ更新
        {
            currentTime += Time.deltaTime;
            // Debug.Log(currentTime + "[PlayerDataManager]");
            
        }
        else // そうでないなら経過時間リセット、ゲージ非表示
        {
            currentTime = 0;
        }
        
    }

    private async Task MedalSupplyAsync()
    {
        await Task.Delay(CommonConstManager.SUPPLYTIME); // 遅延
        MedalProperty += SUPPLYMEDAL; // メダル増やす セーブデータ更新のためここもプロパティを使う
        currentTime = 0; // 経過時間リセット
        isSupply = false; // フラグリセット
    }

    /* 持ちメダルはpublicにすると危険。privateにしてプロパティでアクセスするようにする。 */
    public long MedalProperty
    {
        get
        {
            return medal;
        }
        set
        {
            if(value >= 0) // 正または0の値なら代入許可
            {
                medal = value;
                maxMedal = System.Math.Max(maxMedal, medal); // 持ちメダルの最大が更新され得るので、更新処理
                SaveAndLoadManager.SetLong(CommonConstManager.MEDAL_P, medal); // 持ちメダルのセーブ更新
                SaveAndLoadManager.SetLong(CommonConstManager.MAXMEDAL_P, maxMedal); // maxメダルのセーブ更新
            }
        }
    }

    /* 更新はMedalPropertyで行うのでgetのみ */
    public long MaxMedalProperty
    {
        get
        {
            return maxMedal;
        }
    }

    /* sjpc最高枚数のプロパティ */
    public int SJpcMaxWinProperty
    {
        get
        {
            return shadowJpcMax_Win;
        }
    }

    /* レベルのプロパティ */
    public int SJpcMaxLevelProperty
    {
        get
        {
            return shadowJpcMax_Level;
        }
    }

    /* shadowJpcの最高獲得枚数を更新したらこの関数を呼び出す */
    public void SJpcMaxUpdate(int win, int level)
    {
        shadowJpcMax_Win = win;
        shadowJpcMax_Level = level;
        SaveAndLoadManager.SetInt(CommonConstManager.SJPCMAX_WIN_P, shadowJpcMax_Win);
        SaveAndLoadManager.SetInt(CommonConstManager.SJPCMAX_LEVEL_P, shadowJpcMax_Level);
    }

    /* メダルを補給するかの判定 */
    bool CanSupplyMedal()
    {
        /* 持ちメダルがボーダー未満 & メダル補給中ではない */
        return (medal < SUPPLYBOR && isSupply != true); // 条件を満たしていればtrueを返す
    }

    /* 補給経過時間のプロパティ */
    public float CurrentTimeProperty
    {
        get
        {
            return currentTime;
        }
    }
}
