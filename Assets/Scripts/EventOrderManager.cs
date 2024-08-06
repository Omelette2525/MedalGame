using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConst; //共通定数を扱うためにusing

public class EventOrderManager : MonoBehaviour
{
    [SerializeField] SCManager SCManagerScript;

    public List<int> eventOrder;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* 発生させるイベントが存在するなら、追加が早い順に実行 */
        if(eventOrder.Count > 0)
        {
            switch(eventOrder[0])
            {
                case CommonConstManager.SC:
                break;
            }
        }
    }
}
