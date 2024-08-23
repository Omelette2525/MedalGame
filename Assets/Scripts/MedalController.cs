using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalController : MonoBehaviour
{
    /* prefabにシーン内のオブジェクトをアタッチすることはできないので、生成したメダルはアタッチが外れている */
    [SerializeField] private PlayerDataManager playerDataScript; // 落下時に所持メダルを増やすためにアタッチ
    [SerializeField] private SlotManager slotScript; // スロットストックを増やすためにアタッチ
    [SerializeField] private float boaderZ; // 横穴に落ちたかどうかはz軸で判定
    [SerializeField] private float boaderY; // 一定の高さまで落ちたメダルを消去する
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < boaderY) // メダルが落ちた
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
    public void MedalSetUp(PlayerDataManager getPlayerScript, SlotManager getSlotScript)
    {
        /* 受け取ったスクリプトをセットする */
        playerDataScript = getPlayerScript;
        slotScript = getSlotScript;
    }

    /* スロットチェッカーを通過したらスロットストックを増やす */
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SlotChecker"))
        {
            slotScript.SlotStockPlus();
        }        
    }
}
