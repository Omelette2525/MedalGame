using System.Collections;
using System.Collections.Generic;
using CommonConst;
using Unity.VisualScripting;
using UnityEngine;

public class BallGenerate : MonoBehaviour
{   
    [SerializeField] BallController normalBall; // プレハブ
    [SerializeField] BallController SCBall; // プレハブ
    [SerializeField] BallController brightBall; // プレハブ
    [SerializeField] BallController shadowBall; // プレハブ

    /* ボールにアタッチするために保持しておく */
    [SerializeField] SCManager SCScript; //  SCBallがポケットに入ったことを伝えるために使う
    [SerializeField] JpcManager jpcScript;
    [SerializeField] EventOrderManager eventOrderScript;
    [SerializeField] FieldManager fieldScript; // ボール数を内部でカウントするために使う
    [SerializeField] SoundController soundScript; // イベントボール出現時に音を鳴らす
    [SerializeField] Vector3 normalPos; // レールからボールを出すときの座標
    [SerializeField] Vector3 SCBallPos; // SC用のボールを出現させる座標
    [SerializeField] Vector3 brightBallPos; // SC用のボールを出現させる座標
    [SerializeField] Vector3 shadowBallPos; // SC用のボールを出現させる座標
    [SerializeField] Vector3 SCBallPower; // SC用のボールをどんな勢いで飛ばすか

    [SerializeField] float coolTime; // ボールを連続して払い出すときの間隔
    private float normalBallTimer; // タイマー
    private int payoutNormalBall = 0; // 払い出す必要があるノーマルボールの数

    // Start is called before the first frame update
    void Start()
    {
        normalBallTimer = 0f; // 最初はクールタイムなし
    }

    // Update is called once per frame
    void Update()
    {
        normalBallTimer -= Time.deltaTime; // 経過時間を引く この値が0以下ならクールタイムを消化しきっている
        bool normalBallJudge = CanGenNormalBall(); // ノーマルボールを払い出せるか判定
        if(normalBallJudge == true) // ボールが払い出し可能なら
        {
            NormalBallGenerate(); // ノーマルボールを払い出す
            payoutNormalBall--; // 払い出しボール数-1
        }
    }

    public void debugBall()
    {
        Vector3 debug = new Vector3(-8, 7.5f, 0);
        BallGen(debug, normalBall);
    }

    /* スロットなどでノーマルボールを獲得したときの処理 */
    public void NormalBallGenerate()
    {
        Vector3 ballPos = normalPos; // 出現位置はレールから
        BallGen(ballPos, normalBall);
        fieldScript.FieldBallProperty++; // ボールカウントを増やす
    }

    /* SCで使うボールを生成するときの処理 */
    public void SCBallGenerate()
    {
        Vector3 ballPos = SCBallPos;
        BallController genBall = BallGen(ballPos, SCBall); // 生成したボールの情報を受け取る
        Rigidbody ballRb = genBall.GetComponent<Rigidbody>(); // 力を加えるためにRigidbodyを取得
        // ballRb.AddForce(SCBallPower, ForceMode.Acceleration); // 指定した力でボールに力を加えることで、勢いよくクルーンに入る
        ballRb.velocity = SCBallPower; // 速度を与えてみる
        soundScript.PlaySE(CommonConstManager.EVENTBALLGEN); // scボール出現時に音を鳴らす
    }

    /* BrightJPCで使うボールを生成するときの処理 */
    public void BrightBallGenerate()
    {
        Vector3 ballPos = brightBallPos;
        BallGen(ballPos, brightBall);
        soundScript.PlaySE(CommonConstManager.EVENTBALLGEN); // brightボール出現時に音を鳴らす
    }
    /* ShadowJPCで使うボールを生成するときの処理 */
    public void ShadowBallGenerate()
    {
        Vector3 ballPos = shadowBallPos;
        BallGen(ballPos, shadowBall);
        soundScript.PlaySE(CommonConstManager.EVENTBALLGEN); // shadowボール出現時に音を鳴らす
    }

    BallController BallGen(Vector3 ballPos, BallController ball)
    {
        BallController genBall = Instantiate(ball, ballPos, ball.transform.rotation); // ボールを出す
        genBall.BallSetUp(SCScript, jpcScript, eventOrderScript, fieldScript, soundScript); // ボールにアタッチするスクリプト(BallController)で必要なスクリプトを引数で渡す GetComponentする必要がなくなるので、cpu負荷減
        return genBall; // 生成したボールの情報を返す
    }

    /* ノーマルボールを払い出すかの判定 */
    bool CanGenNormalBall()
    {
        return (normalBallTimer <= 0 && payoutNormalBall >= 1);
    }

    /* ノーマルボールの払い出し個数を外部から増やすためのプロパティ */
    public int PayoutNormalBallProperty
    {
        get
        {
            return payoutNormalBall;
        }
        set
        {
            if(value >= 0) // 正または0なら代入許可
            {
                payoutNormalBall = value;
            }
        }
    }
}
