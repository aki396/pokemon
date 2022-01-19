using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    //ポケモン選択画面の管理
    PartyMemberUI[] memberSlots;

    //PartyMemberUIの取得
    public void Init()
    {
        //自分の子要素からデータを集めてくる命令
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
        
    }
    //BattleSystemから手持ちのポケモンデータをもらって、それぞれにデータをセットする
    public void SetPartyData(List<Pokemon> pokemons)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if(i < pokemons.Count)
            {

                memberSlots[i].SetData(pokemons[i]);

            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }
        messageText.text = "ポケモンを選択してください";
    }
}
