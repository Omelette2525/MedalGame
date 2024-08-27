using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public int medal; // 所持メダル 初期値はインスペクターで指定可能
    private bool isSupply = false;
    const int SUPPLYBOR = 100; // メダル補給するかどうかのボーダー
    const int SUPPLYTIME = 6000; // メダル補給にかかる時間
    const int SUPPLYMEDAL = 10; // 1回で補給する量
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* メダルがボーダー未満で、補給中でないなら補給開始 */
        if(medal < SUPPLYBOR)
        {
            if(isSupply != true)
            {
                isSupply = true; // フラグを立てる
                var _ = MedalSupplyAsync(); // taskを利用しないことを明示
            }            
        }
    }

    private async Task MedalSupplyAsync()
    {
        await Task.Delay(SUPPLYTIME); // 遅延
        medal += SUPPLYMEDAL; // メダル増やす
        isSupply = false; // フラグリセット
    }
}
