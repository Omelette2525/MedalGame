using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalGenerate : MonoBehaviour
{
    [SerializeField] private PlayerDataManager playerDataScript;
    [SerializeField] private SlotManager slotScript;
    private MedalController genMedal; // 生成したメダルの情報を持っておく
    public float coolTime = 0.25f; // メダルを投入する間隔
    [SerializeField] private MedalController medal; // プレハブ

    /* マウスクリック時に出現させる際に必要な情報 */
    /* x座標のボーダー */
    [SerializeField] private float medalBorderLeft;
    [SerializeField] private float medalBorderRight;
    /* yとz座標は事前に決める */
    [SerializeField] private float medalPosY;
    [SerializeField] private float medalPosZ;
    [SerializeField] private Vector3 throwPower; // メダルを投げる強さ
    private float currentTime; // タイマー
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
            Vector3 cursorPos = Input.mousePosition; // マウスカーソルの位置を取得
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
            Vector3 medalPos = Camera.main.ScreenToWorldPoint(cursorPos); // マウスカーソルの位置をワールド座標に変換し、メダル出現位置とする
            /* YとZ座標はあらかじめ決められた位置にセット */
            medalPos.y = medalPosY;
            medalPos.z = medalPosZ;
            MedalGen(medalPos); // カーソル位置にメダル生成

            /* 生成したメダルに力をかけて飛ばす */
            Rigidbody medalRb = genMedal.GetComponent<Rigidbody>(); // 力を加えるためにrigidbodyを取得
            medalRb.AddForce(throwPower, ForceMode.Impulse);

            playerDataScript.medal -= 1; // メダルを減らす
            currentTime = coolTime; // クールタイムリセット
        }
    }

    /* メダル生成 */
    void MedalGen(Vector3 medalPos)
    {
        genMedal = Instantiate(medal, medalPos, medal.transform.rotation); // メダルを出す
        genMedal.MedalSetUp(playerDataScript, slotScript); // メダルにアタッチするスクリプト(MedalDestroy)で必要なスクリプトを引数で渡す　GetComponentする必要がなくなるので、cpu負荷減
    }

    /* メダルを投げるかの判定 */
    bool isThrow()
    {
        /* 左ボタンが押されていたら & メダルを持っていたら & クールタイムを消化していたら */
        return (Input.GetMouseButton(0) == true && playerDataScript.medal >= 1 && currentTime <= 0); // 条件を満たしていればtrueを返す
    }
}
