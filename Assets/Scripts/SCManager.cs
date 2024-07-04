using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using CommonConst;

public class SCManager : MonoBehaviour
{
    [SerializeField] private SCRotate SCRotateScript;
    [SerializeField] private SlotManager SlotManagerScript;
    [SerializeField] private CinemachineVirtualCamera SCCamera;
    private Vector3 rotateInit; // 初期位置
    private Vector3 stopRotate; // 停止位置の角度
    private int stopNumber; // 停止位置のポケットナンバー
    public static int[] SCPocketArray = {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1}; // 各ポケットの状態 0が通常,1がjpc 0~5がbright, 6~11がshadow
    public bool isSC = false; // SC中かどうか
    public static int SCStock = 0; // SCの残り回数
    [SerializeField] private GameObject[] pizzaArray; 
    [SerializeField] private Material[] materialArray;
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
    // Start is called before the first frame update
    void Start()
    {
        rotateInit = SCRotateScript.rotateInit;
    }

    // Update is called once per frame
    void Update()
    {
        /* ストックがあって、SC中, スロット中でないならSC開始 */
        if(SCStock >= 1)
        {
            if(isSC != true && SlotManagerScript.isSlot != true)
            {
                SCStock -= 1;
                SCStart();
            }
            else if(SlotManagerScript.isSlot)
            {
                //Debug.Log("スロット中なので、SC拒否");
            }
        }
    }
    public void SCInit()
    {
        SCRotateScript.rotateFlag = false; // 回転ストップ
        SCRotateScript.rotateObj.eulerAngles = rotateInit; // 角度を初期化
        SCRotateScript.currentAngle = rotateInit; // 内部角度を初期化
    }
    public void SCStart()
    {
        SCCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11; // カメラ切り替え
        SCRotateScript.rotateFlag = true; // 回転開始
        isSC = true; // SC中フラグを有効にする
    }
    public async void PocketIn()
    {
        SCRotateScript.rotateFlag = false; // 回転ストップ

        Debug.Log(SCRotateScript.rotateObj.localEulerAngles);
        stopRotate = SCRotateScript.currentAngle; // 内部角度を取得
        stopNumber = Mathf.Clamp((int)Mathf.Floor(stopRotate.x / 30.0f), 0, 11); // 30度区切りで停止位置を判定する 0 ~ 11
        Debug.Log("停止角度は" + stopRotate);
        Debug.Log("停止ポケットは" + stopNumber);
        if(SCPocketArray[stopNumber] == JPCPOCKET) // jpcなら
        {
            if(stopNumber <= 5)
            {
                Debug.Log("brightJPC");
                for(int i = 0; i < 5; i++) // 初期化 1ポケットはjpcのまま
                {
                    SCPocketArray[i] = NORMALPOCKET;
                    pizzaArray[i].GetComponent<Renderer>().material = materialArray[BRIGHTNORMALMAT];
                }
            }
            else
            {
                Debug.Log("ShadowJPC");
                for(int i = 6; i < 11; i++) // 初期化 1ポケットはjpcのまま
                {
                    SCPocketArray[i] = NORMALPOCKET;
                    pizzaArray[i].GetComponent<Renderer>().material = materialArray[SHADOWNORMALMAT];
                }
            }
        }
        else
        {
            Renderer stopObjmat = pizzaArray[stopNumber].GetComponent<Renderer>(); // 停止ポケットのレンダラーを取得
            if(stopNumber <= 5)
            {
                stopObjmat.material = materialArray[BRIGHTJPCMAT]; // マテリアル変更
                Debug.Log("brightPocket");
            }
            else
            {
                stopObjmat.material = materialArray[SHADOWJPCMAT]; // マテリアル変更
                Debug.Log("ShadowPocket");
            }
            SCPocketArray[stopNumber] = JPCPOCKET; // jpcポケットに変化
        }
        /* 非同期処理を用いて、間を入れる */
        /* 間、カメラ切り替え、間、フラグ処理、間 */
        await WaitTaskAsync(WAIT);
        SCCamera.GetComponent<CinemachineVirtualCamera>().Priority = 1; // カメラ切り替え
        Debug.Log("カメラ");
        await WaitTaskAsync(WAIT);
        SCRotateScript.rotateFlag = true; // 回転開始
        isSC = false; // SC中フラグを無効にする
        Debug.Log("フラグ");
    }
    public void SCStockPlus()
    {
        SCStock++;
    }

    /* 指定した時間待機するタスク */
    private async Task WaitTaskAsync(int delayms)
    {
        await Task.Delay(delayms);
    }
}