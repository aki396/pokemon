using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    //�|�P�����I����ʂ̊Ǘ�
    PartyMemberUI[] memberSlots;

    List<Pokemon> pokemons;

    //PartyMemberUI�̎擾
    public void Init()
    {
        //�����̎q�v�f����f�[�^���W�߂Ă��閽��
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
        
    }
    //BattleSystem����莝���̃|�P�����f�[�^��������āA���ꂼ��Ƀf�[�^���Z�b�g����
    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
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

    public void UpdateMemberSelection(int selectedMember)
    {
        //selectedMember�ƈ�v����Ȃ疼�O�̐F��������
        for(int i=0; i < pokemons.Count; i++)
        {
            if (i == selectedMember)
            {
                //�F��ς���
                memberSlots[i].SetSelected(true);
            }
            else
            {
                //�F�����F
                memberSlots[i].SetSelected(false);
            }
        }    
    }
    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
