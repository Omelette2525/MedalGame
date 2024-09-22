using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConst; // 共通定数を扱うときに簡潔に記述するためにusing

public class BallController : MonoBehaviour
{
    [SerializeField] float boaderY; // 一定の高さまで落ちたボールを消去する
    [SerializeField] SCManager SCScript; // SCBallがポケットに入ったことを伝えるために使う
    [SerializeField] JpcManager jpcScript; // jpcBallがポケットに入ったことを伝えるために使う
    [SerializeField] EventOrderManager eventOrderScript; // イベントにSCを登録するために使う
    [SerializeField] FieldManager fieldScript; // ボール数を内部でカウントするために使う
    [SerializeField] SoundController soundScript; // 音を鳴らすために使う
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < boaderY && CompareTag("Ball")) // メダルが落ちた
        {
            soundScript.PlaySE(CommonConstManager.BALLFALL);
            int[] addEvent = {CommonConstManager.SC, 0}; // イベント情報を生成
            eventOrderScript.EventOrderProperty.Add(addEvent); // プロパティからSCイベントを追加
            Debug.Log(string.Join(", ", eventOrderScript.EventOrderProperty) + "[BallController]"); // イベントリストの中身を表示 第一引数の文字で要素間を区切る
            fieldScript.FieldBallProperty--; // ボールカウントを減らす
            Destroy(gameObject); // ボールを消去
        }
    }

    public void BallSetUp(SCManager getSCScript, JpcManager getJpcScript, EventOrderManager getEventOrderScript, FieldManager getFieldScript, SoundController getSoundScript)
    {
        /* 参照が外れているスクリプトをセットする */
        SCScript = getSCScript;
        jpcScript = getJpcScript;
        eventOrderScript = getEventOrderScript;
        fieldScript = getFieldScript;
        soundScript = getSoundScript;
    }

    /* SCTriggerに触れたらSC終了 jpctriggerに触れたら対応した処理を呼び出す*/
    private async void OnTriggerEnter(Collider other)
    {
        Transform otherTr = other.gameObject.transform; // トリガーのtransformを取得
        if(other.CompareTag("SCTrigger"))
        {
            soundScript.PlaySE(CommonConstManager.POCKETIN); // ポケットに入ったときの音を鳴らす
            await SCScript.PocketInAsync(); // ポケットに入ったときの処理を呼び出す
            Destroy(gameObject); // SCの処理が終わったらボールを消去
        }
        else if(other.CompareTag("JPC50Trigger"))
        {
            PlaySoundAndFreeze(otherTr); // 音を鳴らして位置を固定する
            await jpcScript.JpcPocketIn(CommonConstManager.POCKET50); // 50ポケットに入ったときの処理を呼び出す
            Destroy(gameObject); // jpcの処理が終わったらボールを消去
        }
        else if(other.CompareTag("JPC100Trigger"))
        {
            PlaySoundAndFreeze(otherTr); // 音を鳴らして位置を固定する
            await jpcScript.JpcPocketIn(CommonConstManager.POCKET100); // 100ポケットに入ったときの処理を呼び出す
            Destroy(gameObject); // jpcの処理が終わったらボールを消去
        }
        else if(other.CompareTag("JPC200Trigger"))
        {
            PlaySoundAndFreeze(otherTr); // 音を鳴らして位置を固定する
            await jpcScript.JpcPocketIn(CommonConstManager.POCKET200); // 200ポケットに入ったときの処理を呼び出す
            Destroy(gameObject); // jpcの処理が終わったらボールを消去
        }
        else if(other.CompareTag("JPC300Trigger"))
        {
            PlaySoundAndFreeze(otherTr); // 音を鳴らして位置を固定する
            await jpcScript.JpcPocketIn(CommonConstManager.POCKET300); // 300ポケットに入ったときの処理を呼び出す
            Destroy(gameObject); // jpcの処理が終わったらボールを消去
        }
        else if(other.CompareTag("JPCJPTrigger"))
        {
            PlaySoundAndFreeze(otherTr); // 音を鳴らして位置を固定する
            await jpcScript.JpcPocketIn(CommonConstManager.POCKETJP); // 300ポケットに入ったときの処理を呼び出す
            Destroy(gameObject); // jpcの処理が終わったらボールを消去
        }
        else if(other.CompareTag("JPCOUTTrigger"))
        {
            PlaySoundAndFreeze(otherTr); // 音を鳴らして位置を固定する
            await jpcScript.JpcPocketIn(CommonConstManager.POCKETOUT); // 300ポケットに入ったときの処理を呼び出す
            Destroy(gameObject); // jpcの処理が終わったらボールを消去
        }
    }

    private void PlaySoundAndFreeze(Transform parent)
    {
        soundScript.PlaySE(CommonConstManager.POCKETIN); // ポケットに入ったときの音を鳴らす
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition; // ボールが動かないようにする
        gameObject.transform.SetParent(parent); // 触れたコライダーを親オブジェクトに設定する(ひっついて一緒に動く)
    }
}
