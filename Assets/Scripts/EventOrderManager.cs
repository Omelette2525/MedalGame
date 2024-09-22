using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConst;
using System.Linq; // 共通定数を扱うときに簡潔に記述するためにusing
public class EventOrderManager : MonoBehaviour
{
    [SerializeField] SCManager SCScript; // SCへのアクセスに使う
    [SerializeField] JpcManager jpcScript; // jpcへのアクセスに使う
    [SerializeField] SlotManager slotScript; // スロットが実行中か確認するために使う

    private List<int[]> eventOrder; // 実行するイベントの順番とそのレベルを入れる
    private bool isEvent = false; // イベント実行中かどうか
    
    // Start is called before the first frame update
    void Start()
    {
        eventOrder = new List<int[]>(); // 初期化 これを忘れるとNullReferenceエラーが出る
    }

    // Update is called once per frame
    void Update()
    {
        bool eventJudge = CanExecuteEvent(); // イベント実行可能か判定
        if(eventJudge == true)
        {
            int eventKind = eventOrder[0][0]; // イベントの種類
            int eventLevel = eventOrder[0][1]; // イベントレベル レベルが存在しないものは0を入れる
            /* 発生させるイベントが存在するなら、追加が早い順に実行 */
            switch(eventKind) // 一番先頭のイベントの種類を見る
            {
                /* SC発生 */
                case CommonConstManager.SC:
                SCScript.SCStart(); // SC実行
                eventOrder.RemoveAt(0); // 実行したのでイベントリストから削除
                isEvent = true; // イベントフラグをtrueにする
                break;

                /* brightJPC発生 */
                case CommonConstManager.BRIGHTJPC:
                Debug.Log("brightJPCを実行[EventOrderManager]");
                jpcScript.BrightJpc(eventLevel); // jpc実行
                eventOrder.RemoveAt(0); // 実行したのでイベントリストから削除
                isEvent = true; // イベントフラグをtrueにする
                break;

                /* shadowJPC発生 */
                case CommonConstManager.SHADOWJPC:
                Debug.Log("shadowJPCを実行[EventOrderManager]");
                jpcScript.ShadowJpc(eventLevel); // jpc実行
                eventOrder.RemoveAt(0); // 実行したのでイベントリストから削除
                isEvent = true; // イベントフラグをtrueにする
                break;

                /* 登録されていないイベントを実行しようとした */
                default:
                Debug.Log("イベントエラー: 辞書にないイベントです。[EventOrderManager]");
                break;
            }
            // Debug.Log(string.Join(", ", eventOrder.Select(e => e.ToString())) + "[EventOrderManager]"); // イベントリストの中身を表示 第一引数の文字で要素間を区切る
        }
    }

    /* 外部からイベントリストにアクセスするためのプロパティ */
    public List<int[]> EventOrderProperty
    {
        get
        {
            return eventOrder;
        }
        set
        {
            eventOrder = value;
        }
    }

    /* 外部からイベントフラグにアクセスするためのプロパティ */
    public bool IsEventProperty
    {
        get
        {
            return isEvent;
        }
        set
        {
            isEvent = value;
        }
    }

    /* イベントが実行可能か判定する */
    bool CanExecuteEvent()
    {
        /* 実行するべきイベントが存在する & イベント実行中でない & スロット実行中でない */
        return (eventOrder.Count > 0 & isEvent != true & slotScript.IsSlotProperty != true);
    }

    public void TestMethod() // test
    {
        int[] addEvent = {CommonConstManager.BRIGHTJPC, 2};
        eventOrder.Insert(0, addEvent);
    }
    public void TestMethod2() // test
    {
        int[] addEvent = {CommonConstManager.SHADOWJPC, 1};
        eventOrder.Insert(0, addEvent);
    }
}
