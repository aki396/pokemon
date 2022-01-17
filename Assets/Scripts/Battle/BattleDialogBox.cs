using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Color hightlightcolor;
    [SerializeField] int letterPersecond; //�ꕶ��������̎���
    //�����Fdialog��Text���擾���ĕύX����
    [SerializeField] Text dialogText;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;


    //Text��ύX���邽�߂̊֐�
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }
    //�^�C�v�`���ŕ�����\������
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(char letter in dialog)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPersecond);
        }
        yield return new WaitForSeconds(0.7f);
    }

    //UI�̕\��/��\��

    //dialogText�̕\���Ǘ�
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    //actionSelector�̕\���Ǘ�
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    //moveSelector�̕\���Ǘ�
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }
    //�I�𒆂̃A�N�V�����̐F��ς���
    public void UpdateActionSelection(int selectAction)
    {
        //selectAction��0�̎���actionTexts[0]�̐F��ɂ���B����ȊO����
        //selectAction��1�̎���actionTexts[1]�̐F��ɂ���B����ȊO����
        for(int i = 0; i < actionTexts.Count; i++)
        {
            if(selectAction == i)
            {
                actionTexts[i].color = hightlightcolor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void SetMoveNames(List<Move>moves)
    {


        for(int i=0; i<moveTexts.Count; i++)
        {
            //�o���Ă��鐔�������f
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = ".";
            }

        }
    }
    //�I�𒆂̃A�N�V�����̐F��ς���
    public void UpdateMoveSelection(int selectMove,Move move)
    {
        //selectMove��0�̎���moveTexts[0]�̐F��ɂ���B����ȊO����
        //selectMove��1�̎���moveTexts[1]�̐F��ɂ���B����ȊO����
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (selectMove == i)
            {
                moveTexts[i].color = hightlightcolor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
        ppText.text = $"PP{move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();
    }

}

