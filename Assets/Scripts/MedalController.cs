using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalController : MonoBehaviour
{
    [SerializeField] private PlayerDataManager playerDataScript; // prefabにシーン内のオブジェクトをアタッチすることはできないので、生成したメダルはアタッチが外れている
    [SerializeField] private float boaderZ; // 横穴に落ちたかどうかはz軸で判定
    [SerializeField] private float boaderY; // 一定の高さまで落ちたメダルを消去する
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < boaderY && CompareTag("Medal")) // メダルが落ちた
        {
            if(gameObject.transform.position.z < boaderZ) // 手前側で落ちたらメダルゲット
            {
                playerDataScript.medal += 1;
                //Debug.Log("持ちメダルは" + playerDataScript.medal + "枚");
            }
            Destroy(gameObject); // メダルを消去
        }
    }

    /* 生成する際に外れたアタッチをつける */
    public void MedalSetUp(PlayerDataManager script)
    {
        playerDataScript = script; // 受け取ったスクリプトをセットする
    }
}
