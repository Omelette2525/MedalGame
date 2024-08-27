using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerate : MonoBehaviour
{
    private BallController genBall; // 生成したボールの情報を持っておく
    [SerializeField] BallController ball; // プレハブ
    [SerializeField] SCManager SCManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void debugBall()
    {
        Vector3 debug = new Vector3(0, 0, -5);
        BallGen(debug);
    }

    void BallGen(Vector3 ballPos)
    {
        genBall = Instantiate(ball, ballPos, ball.transform.rotation); // ボールを出す
        genBall.BallSetUp(SCManagerScript); // ボールにアタッチするスクリプト(BallController)で必要なスクリプトを引数で渡す GetComponentする必要がなくなるので、cpu負荷減
    }
}
