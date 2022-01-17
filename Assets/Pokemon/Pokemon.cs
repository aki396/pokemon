using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���x���ɉ������X�e�[�^�X�̈Ⴄ�����X�^�[�𐶐�����N���X
//���ӁF�f�[�^�݈̂���������C#�̃N���X
[System.Serializable]
public class Pokemon 
{
    //�v���p�e�B����f�[�^��ݒ�o����l�ɂ���
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    //�x�[�X�ƂȂ�f�[�^
    public PokemonBase Base { get => _base; }

    public int Level { get => level; }

    public int HP { get; set; }

    //�g����Z
    public List<Move> Moves { get; set; }

    //�R���X�g���N�^�F�������̏����ݒ� => �K�v�Ȃ��Ȃ����̂�init�֐��ɕύX
    public void Init()
    {

       // Base = pBase;

       // Level = pLevel;

        HP = MaxHP;

        Moves = new List<Move>();

        //�g����Z�̐ݒ�F�o����Z�̃��x���ȏ�Ȃ�AMoves�ɒǉ�
        foreach (LearnableMove LearnableMove in Base.LearnableMoves)
        {
            if (Level >= LearnableMove.Level)
            {
                //�����Ă���񂾂�����Z���o����
                Moves.Add(new Move(LearnableMove.Base1));
            }

            //4�ȏ�̋Z�͎g���Ȃ�
            if(Moves.Count >= 4)
            {
                break;
            }
       
        }
    }

    //level�ɉ������X�e�[�^�X��Ԃ����́F�v���p�e�B(+�����������邱�Ƃ��o����)
    //�֐��o�[�W����
    //public int Attack()
    //{
    //    return Mathf.FloorToInt((_base.Attack * level) / 100f) + 5;
    //}

    //�v���p�e�B
    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    public int MaxHP
    {
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10; }
    }

    //�C��
    //�E�퓬�s�\�E�N���e�B�J���E�����̏���n��
    public DamageDetails TakeDamage(Move move,Pokemon attacker)
    {
        //�N���e�B�J��
        float critical = 1f;
        //6.25���ŃN���e�B�J��
        if(Random.value * 100 <= 6.25f)
        {
            critical = 2f;
        }
        //����
        float type = TypeChart.GetEffectivenss(move.Base.Type, Base.Type1) * TypeChart.GetEffectivenss(move.Base.Type, Base.Type2);
        DamageDetails damageDetails = new DamageDetails
        {
            Fainted = false,
            Critical = critical,
            TypeEffectiveness = type

        };
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;

        if(HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

}
public class DamageDetails
{
    public bool Fainted { get; set; } //�퓬�s�\���ǂ���
    public float Critical { get; set; } //�N���e�B�J�����ǂ���
    public float TypeEffectiveness { get; set; } //����
}
