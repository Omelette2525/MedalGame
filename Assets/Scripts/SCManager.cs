using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using CommonConst;
using UnityEngine.UI;
using TMPro; // 共通定数を扱うときに簡潔に記述するためにusing

public class SCManager : MonoBehaviour
{
    [SerializeField] SCRotate SCRotateScript; // ポケット決定時に回転を止めるために使う
    [SerializeField] SlotManager slotScript; // SCがスタート可能か判定するために使う
    [SerializeField] MedalGenerate medalGenerateScript; // 払い出しメダルを増やすために使う
    [SerializeField] BallGenerate ballGenerateScript; // SC用ボールを出現させるために使う
    [SerializeField] EventOrderManager eventOrderScript; // イベントフラグにアクセス、jpcをイベントに追加するために使う
    [SerializeField] UIManager UIScript; // 得たものの表示に使う
    [SerializeField] CinemachineVirtualCamera SCCamera; // SCを映すカメラ
    public static int[] SCPocketArray = {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1}; // 各ポケットの状態 0が通常,1がjpc 0~5がbright, 6~11がshadow

    [SerializeField] GameObject[] pizzaArray; // 各ポケットのオブジェクトを格納する配列
    [SerializeField] Material[] materialArray; // 表示マテリアルを格納する配列
    /* 外れたときの払い出しメダル数を定数にする */
    const int PAYOUTWHENFAIL = 30; // 外れたら30枚払い出し
    /* ポケットの状態を定数にする */
    const int NORMALPOCKET = 0;
    const int JPCPOCKET = 1;
    /* マテリアルの位置を定数にする */
    const int BRIGHTNORMALMAT = 0;
    const int BRIGHTJPCMAT = 1;
    const int SHADOWNORMALMAT = 2;
    const int SHADOWJPCMAT = 3;
    /* その他定数 */
    const int WAIT = 3000; // 演出のためにわざと遅延させる

    private float currentTime; // debug用 scにかかる時間を計測する
    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
    }
    public void SCInit()
    {
        SCRotateScript.RotateFlagProperty = false; // 回転ストップ
        SCRotateScript.RotateAngleInit(); // 角度初期化
    }
    /* EventOrderManagerからこの関数を実行する */
    public async void SCStart()
    {
        SCCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11; // カメラ切り替え
        SCRotateScript.RotateFlagProperty = true; // 回転開始
        /* 間を開けてからSCスタート */
        await WaitTaskAsync(WAIT);
        ballGenerateScript.SCBallGenerate();
        currentTime = 0; // debug用
    }
    public async Task PocketInAsync()
    {
        SCRotateScript.RotateFlagProperty = false; // 回転ストップ
        Debug.Log("経過時間: " + currentTime + "[SCManager]"); // debug用

        Vector3 stopRotate = SCRotateScript.CurrentAngleProperty; // 停止位置の内部角度を取得
        int stopNumber = Mathf.Clamp((int)Mathf.Floor(stopRotate.x / 30.0f), 0, 11); // 30度区切りで停止位置を判定し、番号を割り当てる 0 ~ 11
        //Debug.Log("停止角度は" + stopRotate);
        //Debug.Log("停止ポケットは" + stopNumber);

        if(SCPocketArray[stopNumber] == JPCPOCKET) // jpcなら
        {
            if(stopNumber <= 5) // 5以下ならbrightjpc
            {
                /* jpcポケットの数をチェックして、レベルとする 初期ポケットは最初から含んでおく */
                int level = 1;
                for(int i = 0; i < 5; i++)
                {
                    if(SCPocketArray[i] == JPCPOCKET)
                    {
                        level++;
                    }
                }
                Debug.Log("brightJPC[SCManager]");
                int[] addEvent = {CommonConstManager.BRIGHTJPC, level}; // イベント情報を生成
                eventOrderScript.EventOrderProperty.Insert(0, addEvent); // イベントの先頭にbrightJPCを追加
                // Debug.Log(string.Join(", ", eventOrderScript.EventOrderProperty) + "[SCManager]"); // イベントリストの中身を表示 第一引数の文字で要素間を区切る
                UIScript.SomethingDisplay("BrightJPC level" + level + "獲得！"); // 得たものを表示
                for(int i = 0; i < 5; i++) // 初期化 1ポケットはjpcのまま
                {
                    /* 内部状態とマテリアルを変更 */
                    SCPocketArray[i] = NORMALPOCKET;
                    pizzaArray[i].GetComponent<Renderer>().material = materialArray[BRIGHTNORMALMAT];
                    /* textを変更 */
                    GameObject child = pizzaArray[i].transform.GetChild(0).gameObject; // 子オブジェクトを取得
                    child.GetComponent<TMP_Text>().text = PAYOUTWHENFAIL.ToString(); // 失敗時に払い出す枚数をstringにして渡す
                }
            }
            else // 6以上ならshadowjpc
            {
                /* jpcポケットの数をチェックして、レベルとする 初期ポケットは最初から含んでおく */
                int level = 1;
                for(int i = 6; i < 11; i++)
                {
                    if(SCPocketArray[i] == JPCPOCKET)
                    {
                        level++;
                    }
                }
                Debug.Log("ShadowJPC[SCManager]");
                int[] addEvent = {CommonConstManager.SHADOWJPC, level}; // イベント情報を生成
                eventOrderScript.EventOrderProperty.Insert(0, addEvent); // イベントの先頭にshadowJPCを追加
                // Debug.Log(string.Join(", ", eventOrderScript.EventOrderProperty) + "[SCManager]"); // イベントリストの中身を表示 第一引数の文字で要素間を区切る
                UIScript.SomethingDisplay("ShadowJPC level" + level + "獲得！"); // 得たものを表示
                for(int i = 6; i < 11; i++) // 初期化 1ポケットはjpcのまま
                {
                    /* 内部状態とマテリアルを変更 */
                    SCPocketArray[i] = NORMALPOCKET;
                    pizzaArray[i].GetComponent<Renderer>().material = materialArray[SHADOWNORMALMAT];
                    /* textを変更 */
                    GameObject child = pizzaArray[i].transform.GetChild(0).gameObject; // 子オブジェクトを取得
                    child.GetComponent<TMP_Text>().text = PAYOUTWHENFAIL.ToString(); // 失敗時に払い出す枚数をstringにして渡す
                }
            }
        }
        else // jpcでないなら
        {
            Renderer stopObjmat = pizzaArray[stopNumber].GetComponent<Renderer>(); // 停止ポケットのレンダラーを取得
            if(stopNumber <= 5)
            {
                stopObjmat.material = materialArray[BRIGHTJPCMAT]; // マテリアル変更
                Debug.Log("brightPocket[SCManager]");
            }
            else
            {
                stopObjmat.material = materialArray[SHADOWJPCMAT]; // マテリアル変更
                Debug.Log("ShadowPocket[SCManager]");
            }
            /* textを変更 */
            GameObject child = pizzaArray[stopNumber].transform.GetChild(0).gameObject; // 停止ポケットの子オブジェクトを取得
            child.GetComponent<TMP_Text>().text = "JPC"; // テキストをJPCに変更する
            SCPocketArray[stopNumber] = JPCPOCKET; // jpcポケットに変化
            medalGenerateScript.PayoutMedalProperty += PAYOUTWHENFAIL; // 払い出し枚数を増やす
            UIScript.SomethingDisplay(PAYOUTWHENFAIL + "枚獲得"); // 得たものを表示
        }
        /* 非同期処理を用いて、間を入れる */
        /* 間、カメラ切り替え、間、フラグ処理、間 */
        await WaitTaskAsync(WAIT);
        SCCamera.GetComponent<CinemachineVirtualCamera>().Priority = 1; // カメラ切り替え
        Debug.Log("カメラ[SCManager]");
        await WaitTaskAsync(WAIT);
        SCRotateScript.RotateFlagProperty = true; // 回転開始
        eventOrderScript.IsEventProperty = false; // イベントフラグを解除
        Debug.Log("フラグ[SCManager]");
    }

    /* 指定した時間待機するタスク */
    private async Task WaitTaskAsync(int delayms)
    {
        await Task.Delay(delayms);
    }
}