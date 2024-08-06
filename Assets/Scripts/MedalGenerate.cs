using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalGenerate : MonoBehaviour
{
    [SerializeField] private PlayerDataManager playerDataScript;
    private MedalController genMedal; // 生成したメダルの情報を持っておく
    public float coolTime = 0.25f; // メダルを投入する間隔
    [SerializeField] private MedalController medal; // プレハブ
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
        currentTime = coolTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        bool throwJudge = isThrow(); // メダルを投げるかメソッドで判定
        /* trueならメダルを投げる */
        if(throwJudge == true)
        {
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

            genMedal = Instantiate(medal, medalPos, medal.transform.rotation); // メダルを出す
            genMedal.MedalSetUp(playerDataScript); // メダルにアタッチするスクリプト(MedalDestroy)で必要なスクリプトを引数で渡す　GetComponentする必要がなくなるので、cpu負荷減

            playerDataScript.medal -= 1; // メダルを減らす
            currentTime = coolTime; // クールタイムリセット
        }
    }

    /* メダルを投げるかの判定 */
    bool isThrow()
    {
        /* 左ボタンが押されていたら & メダルを持っていたら & クールタイムを消化していたら */
        return (Input.GetMouseButton(0) == true && playerDataScript.medal >= 1 && currentTime <= 0); // 条件を満たしていればtrueを返す
    }
}
