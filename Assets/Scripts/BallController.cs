using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private float boaderY; // 一定の高さまで落ちたボールを消去する
    [SerializeField] private SCManager SCManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        // SCManagerScript = GameObject.Find("GameManager").GetComponent<SCManager>(); // prefabにスクリプトをアタッチできないので、getcomponentで持ってくる
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < boaderY && CompareTag("Ball")) // メダルが落ちた
        {
            SCManagerScript.SCStockPlus(); // static変数にアクセスできないので、メソッドからストックを増やす
            Destroy(gameObject); // ボールを消去
        }
    }
}
