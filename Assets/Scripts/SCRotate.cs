using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCRotate : MonoBehaviour
{
    public Transform rotateObj; // 回転させるオブジェクトをアタッチ
    public Vector3 rotateInit = new Vector3(0, 450, 90); // 初期位置
    public float rotateSpeed; // 回転速度
    public bool rotateFlag = true; // 回転させるかどうか falseでストップ
    public Vector3 currentAngle; // 内部角度
    // Start is called before the first frame update
    void Start()
    {
        currentAngle = rotateObj.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if(rotateFlag == true)
        {
            float angle = rotateSpeed * Time.deltaTime; // 回転させる角度を決める ゲーム内時間と同期する
            rotateObj.Rotate(0, angle, 0);  // x軸でangle度回転させる
            currentAngle.x += angle; // 回転させた分内部角度を増やす
            if(currentAngle.x >= 360) // 360度を超えたら0度に戻す
            {
                currentAngle.x -= 360;
            }
        }
    }
}
