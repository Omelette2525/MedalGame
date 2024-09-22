using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonConst;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataManager : MonoBehaviour
{
    private long medal; // 所持メダル
    private long maxMedal; // 所持メダルが一番多かったときの値
    private bool isSupply = false; // 補給フラグ
    
    const int SUPPLYBOR = 100; // メダル補給するかどうかのボーダー
    const int SUPPLYMEDAL = 10; // 1回で補給する量
    const long FIRSTMEDAL = 100; // 持ちメダルの初期値
    private float currentTime; // 経過時間 スライダーの描画に使う
    // Start is called before the first frame update
    void Start()
    {
        /* 初期化 */
        medal = FIRSTMEDAL;
        maxMedal = medal;
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
        medal += SUPPLYMEDAL; // メダル増やす
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
            }
        }
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
