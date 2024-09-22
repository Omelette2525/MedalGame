using System.Collections;
using System.Collections.Generic;
using CommonConst;
using UnityEngine;

public class MedalGenerate : MonoBehaviour
{
    /* メダルにアタッチする必要があるスクリプト */
    [SerializeField] PlayerDataManager playerDataScript;
    [SerializeField] SlotManager slotScript;
    [SerializeField] FieldManager fieldScript; // このスクリプトでもinMedalを増やすために使う
    [SerializeField] SoundController soundScript; // このスクリプトでもメダルを投げるときのSEで使う
    [SerializeField] EventOrderManager eventOrderScript; // イベントフラグにアクセスするために使う
    [SerializeField] float throwCoolTime; // メダルを投入する間隔
    [SerializeField] float payoutCoolTime; // メダルを払い出す間隔
    [SerializeField] MedalController medal; // プレハブ

    /* メダル払い出し時に必要な情報 */
    private int payoutMedal = 0; // ゲーム側が払い出す残メダル スロットなどで当選すると、この値が増える メダルを払い出すとこの値が減る
    
    /* 払い出し時のボーダー */
    [SerializeField] float winMedalBorderFrontZ; // 手前側
    [SerializeField] float winMedalBorderBackZ; // 奥側
    [SerializeField] float winMedalBorderLeftX; // 左側
    [SerializeField] float winMedalBorderRightX; // 右側

    [SerializeField] float winMedalPosY; // y座標の位置

    /* マウスクリック時に出現させる際に必要な情報 */
    /* x座標のカーソルのボーダー カーソル座標は左下が0, 0*/
    /* 一定の値にしてしまうとxの長さが変わったときに対応できないので、割合を指定してそこから計算する */
    [SerializeField] float cursorBorderRatioX; // 0.1なら画面の真ん中8割の中に補正する
    /* yとz座標は事前に決める */
    [SerializeField] float medalPosY;
    [SerializeField] float medalPosZ;
    [SerializeField] Vector3 throwPower; // メダルを投げる強さ
    private float throwTimer; // メダル投げタイマー
    private float payoutTimer; // 払い出しタイマー

    const int PAYOUTONCE = 5; // 一度に払い出すメダルの枚数
    private int payoutState; // 払い出しの状態 決まった単位で払い出しをするために、この数値がPAYOUTONCEの倍数になったらクールタイムをつける
    // Start is called before the first frame update
    void Start()
    {
        throwTimer = 0; // 最初はクールタイムなし
        payoutTimer = 0;
        payoutState = 0; // 1枚も払い出していない状態
    }

    // Update is called once per frame
    void Update()
    {
        throwTimer -= Time.deltaTime; // 経過時間を引く 0以下ならクールタイムを消化しきっている
        payoutTimer -= Time.deltaTime; // 払い出しタイマーも同様にする
        bool throwJudge = CanThrowMedal(); // メダルを投げるかメソッドで判定
        /* trueならメダルを投げる */
        if(throwJudge == true)
        {
            MedalThrow();
            playerDataScript.MedalProperty--; // メダルを減らす
            fieldScript.InMedalProperty++; // メダルを投入したので、inMedalを増やす
            throwTimer = throwCoolTime; // クールタイムリセット
        }

        bool payoutJudge = CanPayoutMedal(); // メダルを払い出すか判定
        if(payoutJudge == true) // 払い出し可能なら
        {
            float randomX = UnityEngine.Random.Range(winMedalBorderLeftX, winMedalBorderRightX); // x座標のボーダーは事前に決めておく
            float randomZ = UnityEngine.Random.Range(winMedalBorderFrontZ, winMedalBorderBackZ); // z座標のボーダーも事前に決めておく そこからランダムに決める
            Vector3 payoutPos = new Vector3 (randomX, winMedalPosY, randomZ); // ランダムに決めたx, z座標と、事前に決めておいたy座標を組み合わせて払い出しメダルの出現位置とする

            MedalGen(payoutPos); // 関数でメダル生成
            fieldScript.WinMedalProperty++; // メダルを払い出したので、winMedalを増やす
            payoutMedal--; //残り払い出し枚数を減らす

            payoutState++; //払い出し枚数を増やす
            if(payoutState % PAYOUTONCE == 0) // 払い出し枚数がPAYOUTONCEの倍数になったらクールタイム
            {
                payoutTimer = payoutCoolTime;
            }
        }
        else // 払い出し可能でないなら、何枚払い出したかの状態をリセット
        {
            payoutState = 0;
        }
    }

    /* メダル生成 */
    MedalController MedalGen(Vector3 medalPos)
    {
        MedalController genMedal = Instantiate(medal, medalPos, medal.transform.rotation); // メダルを出すと同時に、生成したメダルの情報を持っておく
        genMedal.MedalSetUp(playerDataScript, slotScript, fieldScript, soundScript); // メダルにアタッチするスクリプト(MedalController)で必要なスクリプトを引数で渡す GetComponentする必要がなくなるので、cpu負荷減
        return genMedal; // 呼び出し元にgenMedalを返して、使えるようにする
    }

    /* メダルを投げる関数 */
    void MedalThrow()
    {
        Vector3 cursorPos = Input.mousePosition; // マウスカーソルの位置を取得
        /* x座標の補正 */
        float cursorBorderLeftX = Screen.width * cursorBorderRatioX; // ratio分は使えないエリア
        float cursorBorderRightX = Screen.width - (Screen.width * cursorBorderRatioX); // 右端から計算

        if(cursorPos.x < cursorBorderLeftX)
        {
            cursorPos.x = cursorBorderLeftX;
        }
        else if(cursorPos.x > cursorBorderRightX)
        {
            cursorPos.x = cursorBorderRightX;
        }
        cursorPos.z = 10; // z座標を適当に代入
        Vector3 medalPos = Camera.main.ScreenToWorldPoint(cursorPos); // マウスカーソルの位置をワールド座標に変換し、メダル出現位置とする
        /* YとZ座標はあらかじめ決められた位置にセット */
        medalPos.y = medalPosY;
        medalPos.z = medalPosZ;
        MedalController genMedal = MedalGen(medalPos); // カーソル位置にメダル生成 生成したメダルの情報を受け取る

        /* 生成したメダルに力をかけて飛ばす */
        Rigidbody medalRb = genMedal.GetComponent<Rigidbody>(); // 力を加えるためにrigidbodyを取得
        medalRb.AddForce(throwPower, ForceMode.Impulse);

        soundScript.PlaySE(CommonConstManager.MEDALTHROW); // 音を鳴らす
    }

    /* メダルを投げるかの判定 */
    bool CanThrowMedal()
    {
        /* 左ボタンが押されていたら & メダルを持っていたら & クールタイムを消化していたら & イベント中でないなら イベント中だとカメラの位置が変わってメダルの位置がおかしくなる */
        return (Input.GetMouseButton(0) == true && playerDataScript.MedalProperty >= 1 && throwTimer <= 0 && eventOrderScript.IsEventProperty != true); // 条件を満たしていればtrueを返す
    }

    /* メダルを払い出すかの判定 */
    bool CanPayoutMedal()
    {
        /* 払い出しがまだ残っていたら & クールタイムを消化していたら & イベント中でないなら */
        return (payoutMedal >= 1 && payoutTimer <= 0 && eventOrderScript.IsEventProperty != true);
    }

    /* 払い出しメダルをゲットしたときは、外部からプロパティにアクセスして、payoutMedalの値を増やす */
    public int PayoutMedalProperty
    {
        get
        {
            return payoutMedal;
        }
        set
        {
            if(value >= 0) // 0または正の値なら代入許可
            {
                payoutMedal = value;
            }
        }
    }

    /* デバッグ用関数 */
    public void DebugPayout()
    {
        payoutMedal += 30;
        Debug.Log("payoutMedalを増やしました[MedalGenerate]");
    }
}
