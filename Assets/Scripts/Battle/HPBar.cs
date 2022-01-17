using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    //HP�̑����̕`�������
    [SerializeField] GameObject health;

    public void SetHP(float hp)
    {
        health.transform.localScale = new Vector3(hp, 1f, 1f);
    }
   public IEnumerator SetHPSmooth(float newHP)
    {
        float currentHP = health.transform.localScale.x;
        float changeAmount = currentHP - newHP;

        //currentHP �Ɓ@newHP�ɍ�������Ȃ�J��Ԃ�
        while (currentHP - newHP > Mathf.Epsilon)
        {
            currentHP -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHP, 1, 1);
            yield return null;

        }
        health.transform.localScale = new Vector3(newHP, 1, 1);
    }

}
