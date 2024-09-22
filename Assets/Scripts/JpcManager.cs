using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using CommonConst;
using System.Threading;
using Unity.VisualScripting;
using Cinemachine;
using TMPro;
using UnityEngine.UI; // 共通定数を扱うときに簡潔に記述するためにusing

public class JpcManager : MonoBehaviour
{
    [SerializeField] BallGenerate ballGenerateScript; // jpc用のボールを出現させるために使う
    [SerializeField] MedalGenerate medalGenerateScript; // 払い出しメダルを増やすために使う
    [SerializeField] EventOrderManager eventOrderScript; // イベントフラグの操作に使う
    [SerializeField] UIManager UIScript; // 得たものの表示に使う
    [SerializeField] Canvas shadowJpcCanvas; // shadowJpcのuiの表示を切り替えるのに使う
    [SerializeField] TMP_Text shadowJpcInfo; // shadowJPCの情報を表示するのに使う
    [SerializeField] Image[] shadowJpcSteps; // shadowJpcのstepを表示するのに使う
    [SerializeField] CinemachineVirtualCamera brightJpcCamera; // brightjpcのカメラ
    [SerializeField] CinemachineVirtualCamera shadowJpcCamera; // shadowjpcのカメラ
    [SerializeField] Rotate brightJpcRotate; // brightjpcdonutsを回しているスクリプト
    [SerializeField] Rotate shadowJpcRotate; // shadowjpcdonutsを回しているスクリプト
    [SerializeField] GameObject[] brightJpcDonutsArrey; // brightJPCの各ポケットを入れる
    [SerializeField] GameObject[] shadowJpcDonutsArrey; // shadowJPCの各ポケットを入れる


    [SerializeField] GameObject jpcPocket50; // 50pocketのプレハブ
    [SerializeField] GameObject jpcPocket100; // 100pocketのプレハブ
    [SerializeField] GameObject jpcPocket200; // 200pocketのプレハブ
    [SerializeField] GameObject jpcPocket300; // 300pocketのプレハブ
    [SerializeField] GameObject jpcPocketJP; // JPpocketのプレハブ
    [SerializeField] GameObject jpcPocketOUT; // outpocketのプレハブ

    private int nowJpc; // 現在何のjpcを実行しているのか格納する変数 0なら何も実行していない
    private int brightJpBonus; // brightJPを獲得したときの獲得メダル
    private bool isJpc; // jpc実行中かどうか
    private bool isPocketInNow; // ポケットにボールが入っているか 入っているなら重複を防ぐために処理しない
    

    private int shadowJpcNowBalls; // shadowJpcでの現在の球数 outの救済判定に使う
    private int shadowJpcState; // shadowJpcで何回Jpポケットに入ったか 3回目からjpbonus獲得
    private int shadowJpcWinMedals; // shadowJpcでの獲得枚数 抽選によってoutに入るまで増えていく
    private int shadowJpcSafeBalls; // shadowJpcでのセーフ球数 デフォルトは2球で、lvが上がると増える

    private string shadowJpcInfoFormat; // textのformatを保存する

    private float jpcTimer; // jpcの経過時間 ボールがスタックしていないかの確認に使う

    /* 各jpcのlv1での状態を初期状態とする */
    private GameObject[] brightJpcDonutsInit;
    private GameObject[] shadowJpcDonutsInit;

    private const int BRIGHTJPINIT = 1000; // brightJPの初期値
    private const int SHADOWJPBONUS = 1000; // shadowJP獲得時に得られるメダル数
    private const int SHADOWJPCLOWBONUS = 50; // shadowJPポケットに1,2回入ったときに得られるメダル数
    private const int SHADOWJPCSAFEBALLS = 2; // 何球目までoutに入ってもセーフにするか
    private const int SHADOWJPCOUTBONUS = 25; // shadowjpcでoutポケットに入ったときに得られるメダル数
    private const int WAIT = 3000; // 演出のためにわざと遅延させる
    private const float STACKTIME = 15f; // 抽選時間がこの時間をオーバーしたら抽選機を減速させる

    // Start is called before the first frame update
    void Start()
    {
        brightJpBonus = BRIGHTJPINIT; // 初期値にセット
        shadowJpcInfoFormat = shadowJpcInfo.text; // formatを保存する
        shadowJpcCanvas.enabled = false; // 最初は非表示
        jpcTimer = 0; // 最初は0秒
        isPocketInNow = false; // 最初は何も入っていない

        /* lv1での状態 */
        brightJpcDonutsInit = new GameObject[]
        {
            jpcPocketJP, jpcPocket100, jpcPocket200, jpcPocket100, jpcPocket200,
            jpcPocket100, jpcPocket200, jpcPocket100, jpcPocket200,
            jpcPocket300, jpcPocket200, jpcPocket100, jpcPocket200, jpcPocket100,
            jpcPocket200, jpcPocket100, jpcPocket200, jpcPocket100
        };

        shadowJpcDonutsInit = new GameObject[]
        {
            jpcPocketJP, jpcPocket50, jpcPocket200, jpcPocketOUT, jpcPocket50,
            jpcPocketJP, jpcPocket50, jpcPocketOUT, jpcPocket50,
            jpcPocketJP, jpcPocket50, jpcPocket100, jpcPocketOUT, jpcPocket50,
            jpcPocketJP, jpcPocket50, jpcPocketOUT, jpcPocket50
        };

        brightJpcDonutsChange(1); // lv1の状態で初期化しておく
        ShadowJpcDonutsChange(); // 初期化しておく
    }

    // Update is called once per frame
    void Update()
    {
        if(isJpc == true) // jpc中ならタイマーを作動させる
        {
            jpcTimer += Time.deltaTime;
            if(jpcTimer >= STACKTIME) // stackしていると判定されたら抽選機を減速させる
            {
                switch(nowJpc)
                {
                    case CommonConstManager.BRIGHTJPC:
                    brightJpcRotate.RotateSpeedProperty = brightJpcRotate.InitSpeedProperty / 2; // 半分に減速
                    break;
                    case CommonConstManager.SHADOWJPC:
                    shadowJpcRotate.RotateSpeedProperty = shadowJpcRotate.InitSpeedProperty / 2; // 半分に減速
                    break;
                    default:
                    break;
                }
            }
        }
    }

    public async void BrightJpc(int upgradeLv)
    {
        brightJpcDonutsChange(upgradeLv); // lvに応じてポケットの内容を変更する lvが高いほどjpポケットは増え、メダル数も増える
        isJpc = true; // jpcスタート
        nowJpc = CommonConstManager.BRIGHTJPC; // brightjpcを実行していることを入れておく
        brightJpcCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11; // カメラ切り替え
        await WaitTaskAsync(WAIT); // 間を入れる
        ballGenerateScript.BrightBallGenerate(); // brightballを出す
        jpcTimer = 0; // タイマー初期化
        isPocketInNow = false; // 何も入っていない状態に更新
    }
    public async void ShadowJpc(int upgradeLv)
    {
        isJpc = true; // jpcフラグをtrueにする
        nowJpc = CommonConstManager.SHADOWJPC; // shadowJpcを実行していることを入れておく
        shadowJpcCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11; // カメラ切り替え
        await WaitTaskAsync(WAIT); // 間を入れる
        /* 初期化 */
        shadowJpcState = 0;
        shadowJpcNowBalls = 1;
        shadowJpcWinMedals = 0;
        shadowJpcSafeBalls = SHADOWJPCSAFEBALLS + (upgradeLv - 1); // 1lvなら変化なし 2lv~ 1ずつ増える
        ShadowJpcInfoRefresh(); // 情報を更新
        shadowJpcCanvas.enabled = true; // 表示をtrueにする
        ballGenerateScript.ShadowBallGenerate(); // shadowBallを出す
        jpcTimer = 0; // タイマー初期化
        isPocketInNow = false; // 何も入っていない状態に更新
    }

    /* jpcの機械の何らかのポケットに入った */
    public async Task JpcPocketIn(int pocket)
    {
        /* pocketIn中なら処理しない */
        if(isPocketInNow != true)
        {
            /* 現在何のjpcを実行中か */
            switch(nowJpc)
            {
                /* brightJpc */
                case CommonConstManager.BRIGHTJPC:
                int winMedal; // 獲得メダルを入れる変数
                if(pocket == CommonConstManager.POCKETJP) // JP
                {
                    winMedal = brightJpBonus; // 獲得メダルを更新
                    Debug.Log("BRIGHTJP獲得[JpcManager]");
                }
                else // JP以外ならpocketの中身がそのまま獲得メダル
                {
                    winMedal = pocket; // 獲得メダルを更新
                    Debug.Log(winMedal + "枚獲得[JpcManager]");
                }
                medalGenerateScript.PayoutMedalProperty += winMedal;
                UIScript.SomethingDisplay(winMedal + "枚獲得！"); // 得たものを表示 jp時の演出は別で作る
                isJpc = false; // jpcが終了したので、falseにする
                break;

                /* shadowJpc */
                case CommonConstManager.SHADOWJPC:
                if(pocket == CommonConstManager.POCKETJP) // jpポケット
                {
                    shadowJpcState++; // jpポケットに入った回数を増やす
                    if(shadowJpcState >= 3) // 3回以上入っているなら、ボーナス獲得
                    {
                        shadowJpcWinMedals += SHADOWJPBONUS;
                        UIScript.SomethingDisplay(SHADOWJPBONUS + "枚獲得！"); // 得たものを表示 jp時の演出は別で作る
                    }
                    else // 3回未満なら、SHADOWJPCLOWBONUS枚増やす
                    {
                        shadowJpcWinMedals += SHADOWJPCLOWBONUS;
                        UIScript.SomethingDisplay(SHADOWJPCLOWBONUS + "枚獲得！"); // 得たものを表示
                    }
                }
                else if(pocket == CommonConstManager.POCKETOUT) // outポケット
                {
                    shadowJpcWinMedals += SHADOWJPCOUTBONUS; // out時のボーナスを獲得
                    ShadowJpcInfoRefresh(); // 情報を更新
                    UIScript.SomethingDisplay(SHADOWJPCOUTBONUS + "枚獲得"); // 得たものを表示
                    if(shadowJpcNowBalls <= shadowJpcSafeBalls) // セーフ球数の範囲内なら、jpc継続
                    {
                        Debug.Log("safe[JpcManager]");
                        // safe時の演出を作る
                    }
                    else // 範囲外なら、jpc終了
                    {
                        Debug.Log(shadowJpcWinMedals + "枚獲得[JpcManager]");
                        medalGenerateScript.PayoutMedalProperty += shadowJpcWinMedals;
                        isJpc = false; // jpcが終了したので、falseにする
                        break; // そのままswitch文を抜ける
                    }
                }
                else // その他なら、pocketの中身がそのまま獲得メダル
                {
                    winMedal = pocket;
                    shadowJpcWinMedals += winMedal;
                    UIScript.SomethingDisplay(winMedal + "枚獲得！"); // 得たものを表示
                }
                /* 情報を更新、間、次のボールを出す、情報更新 */
                ShadowJpcInfoRefresh(); // 情報を更新
                await WaitTaskAsync(WAIT);
                ballGenerateScript.ShadowBallGenerate(); // shadowBallを出す
                shadowJpcNowBalls++; // 球数を増やす
                ShadowJpcInfoRefresh(); // 情報を更新
                break;

                default: // brightでもshadowでもないなら何もしない
                Debug.Log("未定義のjpc、またはjpcが実行されていません[JpcManager]");
                break;
            }
            isPocketInNow = true; // ポケットに入ったのでtrueにする
        }
        
        jpcTimer = 0; // 1球ごとにタイマー初期化
        /* 抽選機の速度を元に戻す */
        brightJpcRotate.RotateSpeedProperty = brightJpcRotate.InitSpeedProperty;
        shadowJpcRotate.RotateSpeedProperty = shadowJpcRotate.InitSpeedProperty;
        if(isJpc == false) // jpcが終了していたら、カメラを切り替えてイベント終了
        {
            await WaitTaskAsync(WAIT);
            /* カメラ切り替え */
            brightJpcCamera.GetComponent<CinemachineVirtualCamera>().Priority = 1; // カメラ切り替え
            shadowJpcCamera.GetComponent<CinemachineVirtualCamera>().Priority = 1; // カメラ切り替え

            shadowJpcCanvas.enabled = false; // shadowjpcの情報を非表示にする
            await WaitTaskAsync(WAIT);
            eventOrderScript.IsEventProperty = false; // イベント実行中フラグをfalseにする
        }
        isPocketInNow = false; // 処理が終わったのでポケットin状態を解除
    }
    /* 指定した時間待機するタスク */
    private async Task WaitTaskAsync(int delayms)
    {
        await Task.Delay(delayms);
    }

    /* jpcポケットを入れ替えるメソッド */
    private GameObject ChangePocket(GameObject currentObj, GameObject newObj)
    {
        /* 元のオブジェクトのposition, rotate, parentを取得しておく */
        Vector3 objPos = currentObj.transform.position;
        Quaternion objRot = currentObj.transform.rotation;
        Transform objParent = currentObj.transform.parent;

        Destroy(currentObj); // 元のオブジェクトを破壊
        GameObject genObj = Instantiate(newObj, objPos, objRot, objParent); // 元のオブジェクトと同じ状態で新しいポケットを生成
        return genObj; // 外れた参照をつけ直すために返す
    }

    public void TestMethod() // test
    {
        brightJpcDonutsChange(1);
    }

    /* brightJpcDonutsを編集する */
    private void brightJpcDonutsChange(int level)
    {
        GameObject[] objArrey = new GameObject[18]; // 宣言
        brightJpcDonutsInit.CopyTo(objArrey, 0); // lv1の状態をコピー objArrey = bright~~ だとシャローコピーとなり、bright~~が変更されてしまう

        /* lv1~5は、1lvUpでjp:+1, 100:-2, 300:+1 */
        for(int i = 1; i < Mathf.Min(level, 5); i++) // 6lvなら4回のみ実行する
        {
            objArrey = ChangeRandomPocketToAim(jpcPocket100, objArrey, jpcPocketJP); // 100pocketの中からランダムで1つ決めて、jpポケットに変更する
            objArrey = ChangeRandomPocketToAim(jpcPocket100, objArrey, jpcPocket200); // 100 -> 200
            objArrey = ChangeRandomPocketToAim(jpcPocket200, objArrey, jpcPocket300); // 200 -> 300
        }

        if(level >= 6) // lv6以上 jp:6, 100:0, 200:5, 300:7    200:-3, jp:+1, 300:+2
        {
            objArrey = ChangeRandomPocketToAim(jpcPocket200, objArrey, jpcPocketJP); // 200pocketの中からランダムで1つ決めて、jpポケットに変更する
            objArrey = ChangeRandomPocketToAim(jpcPocket200, objArrey, jpcPocket300); // 200 -> 300
            objArrey = ChangeRandomPocketToAim(jpcPocket200, objArrey, jpcPocket300); // 200 -> 300
        }
        
        /* 事前に決めたobjArreyを使って実際のオブジェクトを入れ替える */
        int j = 0; // 変数用
        foreach(GameObject obj in objArrey)
        {
            brightJpcDonutsArrey[j] = ChangePocket(brightJpcDonutsArrey[j], obj);
            j++;
        }
    }

    /* shadowJpcDonutsを編集する */
    private void ShadowJpcDonutsChange()
    {
        GameObject[] objArrey = new GameObject[18]; // 宣言
        shadowJpcDonutsInit.CopyTo(objArrey, 0); // lv1の状態をコピー objArrey = shadow~~ だとシャローコピーとなり、shadow~~が変更されてしまう
        /* 現状lvが変わってもshadowJpcはポケットを変更しない */
        /* 事前に決めたobjArreyを使って実際のオブジェクトを入れ替える */
        int j = 0; // 変数用
        foreach(GameObject obj in objArrey)
        {
            shadowJpcDonutsArrey[j] = ChangePocket(shadowJpcDonutsArrey[j], obj);
            j++;
        }
    }
    /* 変更するオブジェクトの位置をランダムに決める */
    private GameObject[] ChangeRandomPocketToAim(GameObject condition, GameObject[] objArrey, GameObject aimObj) // conditionは探索するポケットの種類を指定する ランダムで決めたポケットをaimObjに変更する
    {
        /* 条件に当てはまるobjがいくつあるか数える */
        int count = 0;
        foreach(GameObject obj in objArrey)
        {
            if(obj == condition)
            {
                count++;
            }
        }
        if(count == 0) // 1つも条件に当てはまらなかったら終了
        {
            Debug.Log("条件に当てはまるポケットが見つかりませんでした[JpcManager]");
            return objArrey;
        }

        int randomNum = Random.Range(1, count + 1); // 1 ~ 当てはまった個数の範囲で乱数生成し、何個目のobjを変更するか決める
        int find = 0; // 見つかった個数を記録する
        for(int i = 0; i < objArrey.Length; i++) // objarreyのsizeだけfor文を回す
        {
            if(objArrey[i] == condition) // 条件に当てはまったらfindを増やす
            {
                find++;
                if(find == randomNum) // findがrandomNumと一致したら、その場所のobjをaimObjに変更
                {
                    objArrey[i] = aimObj;
                    return objArrey; // 変更したので終了
                }
            }
        }

        Debug.Log("for文の中でobjの添字を返せませんでした[JpcManager]");
        return objArrey; // for文がうまく回らなかったら終了
    }

    /* shadowjpcの情報を更新する */
    private void ShadowJpcInfoRefresh()
    {
        string state; // 宣言
        /* 今のボールがセーフ状態か見る */
        if(shadowJpcNowBalls <= shadowJpcSafeBalls)
        {
            state = "Safety";
        }
        else
        {
            state = "Danger";
        }
        shadowJpcInfo.text = string.Format(shadowJpcInfoFormat, shadowJpcWinMedals, shadowJpcNowBalls, state); // 獲得メダル、現在の球数、セーフ状態を表示する
        for(int i = 0; i < 3; i++) // step数は3なので3回繰り返す
        {
            if(i < shadowJpcState) // stateの状態と比べて透明度を切り替える state0なら全て半透明、state1ならstep[0]のみ不透明
            {
                shadowJpcSteps[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                shadowJpcSteps[i].color = new Color(1, 1, 1, 0.5f);
            }
        }
    }
}
