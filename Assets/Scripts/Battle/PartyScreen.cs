using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    //�|�P�����I����ʂ̊Ǘ�
    PartyMemberUI[] memberSlots;

    //PartyMemberUI�̎擾
    public void Init()
    {
        //�����̎q�v�f����f�[�^���W�߂Ă��閽��
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
        
    }
    //BattleSystem����莝���̃|�P�����f�[�^��������āA���ꂼ��Ƀf�[�^���Z�b�g����
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
        messageText.text = "�|�P������I�����Ă�������";
    }
}
