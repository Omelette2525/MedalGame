using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PusherMove : MonoBehaviour
{
    [SerializeField] GameObject pusher;
    [SerializeField] float pusherSpeed = 10.0f; // sin関数の中身の係数 大きくなれば中身が早く動くので、プッシャーの動きも早くなる
    // public float pusherBorderFront = 3;
    // public float pusherBorderBack = 5;
    // private bool pusherFlag = true; // trueなら手前に力をかける
    [SerializeField] float pusherRange; // プッシャーの動く範囲(2なら-2~+2の範囲で動く)
    private Vector3 pusherInitPos; //プッシャーの初期位置
    private Rigidbody pusherRb;

    // Start is called before the first frame update
    void Start()
    {
        pusherInitPos = pusher.transform.position; // プッシャーの位置を取得
        pusherRb = GetComponent<Rigidbody>(); // rb取得
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pusherNewPos = new Vector3(pusherInitPos.x, pusherInitPos.y, pusherInitPos.z + pusherRange * Mathf.Sin(pusherSpeed * Time.time)); // xとyは初期位置,zはsin関数を用いて円のように動かす Time.timeはゲーム内時間
        pusherRb.MovePosition(pusherNewPos);
    }
}
