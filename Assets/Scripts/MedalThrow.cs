using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MedalThrow : MonoBehaviour
{
    private PlayerDataManager playerDataScript;
    public float coolTime = 0.25f; // メダルを投入する間隔
    public GameObject medal;
    /* x座標のボーダー */
    [SerializeField] private float medalBorderLeft;
    [SerializeField] private float medalBorderRight;
    /* yとz座標は事前に決める */
    public float medalPosY;
    public float medalPosZ;
    private float currentTime; // タイマー
    private Vector3 cursorPos; //マウスカーソルの位置
    private Vector3 medalPos; // メダルを出現させる位置
    
    // Start is called before the first frame update
    void Start()
    {
        playerDataScript = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();
        currentTime = coolTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        if(Input.GetMouseButton(0) == true) // 左ボタンが押されていたら
        {
            //Debug.Log("左クリック");
            if(playerDataScript.medal >= 1) // メダルを持っていたら
            {
                if(currentTime <= 0) // クールタイムを消化していたら
                {
                    //Debug.Log("メダル出現");
                    cursorPos = Input.mousePosition; // マウスカーソルの位置を取得
                    /* x座標の補正 */
                    if(cursorPos.x < medalBorderLeft)
                    {
                        cursorPos.x = medalBorderLeft;
                    }
                    else if(cursorPos.x > medalBorderRight)
                    {
                        cursorPos.x = medalBorderRight;
                    }
                    cursorPos.z = 10; // z座標を適当に代入
                    medalPos = Camera.main.ScreenToWorldPoint(cursorPos); // マウスカーソルの位置をワールド座標に変換
                    medalPos.y = medalPosY; //YとZ座標はあらかじめ決められた位置にセット
                    medalPos.z = medalPosZ;
                    //Debug.Log(Input.mousePosition);
                    Instantiate(medal, medalPos, medal.transform.rotation);

                    playerDataScript.medal -= 1; // メダルを減らす
                    //Debug.Log("持ちメダルは" + playerDataScript.medal + "枚");
                    currentTime = coolTime; // クールタイムリセット
                }
            }
        }
    }
}
