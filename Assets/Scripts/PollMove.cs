using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollMove : MonoBehaviour
{
    [SerializeField] GameObject poll; // 動くオブジェクト
    [SerializeField] float speed; // 動くスピード
    [SerializeField] Vector3 moveRange; // 動く範囲 中心から同じ距離だけ動く 符号で最初にうごく向きが変えられる
    private Vector3 InitPos; // 初期位置 ここを中心に動く
    // Start is called before the first frame update
    void Start()
    {
        InitPos = poll.transform.position; // 初期位置取得
    }

    // Update is called once per frame
    void Update()
    {
        poll.transform.position = InitPos + moveRange * Mathf.Sin(speed * Time.time); // 初期位置からsin関数の値分動かす
    }
}
