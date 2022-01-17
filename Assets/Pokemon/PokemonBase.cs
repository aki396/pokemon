using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�|�P�����̃}�X�N�f�[�^�F�O������ύX���Ȃ�(�C���X�y�N�^�[����̂ݕύX�\�j
[CreateAssetMenu]
public class PokemonBase : ScriptableObject
{
    //���O�A�����A�摜�A�^�C�v�A�X�e�[�^�X

    [SerializeField] new string name;
    [SerializeField] string description;

    //�摜
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    //�^�C�v
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //�X�e�[�^�X:hp,at,def,sAT,sDF,sp
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    //�o����Z�ꗗ

    [SerializeField] List<LearnableMove> learnableMoves;

    //���t�@�C������attack�̒l�͎擾�o���邪�ύX�͏o���Ȃ�
    public int Attack { get => attack; }
    public int Defense { get => defense; }
    public int SpAttack { get => spAttack; }
    public int SpDefense { get => spDefense; }
    public int Speed { get => speed; }
    public int MaxHP { get => maxHP; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; }
    public string Name { get => name; }
    public string Description { get => description; }
    public Sprite FrontSprite { get => FrontSprite1; }
    public Sprite FrontSprite1 { get => frontSprite; }
    public Sprite BackSprite { get => backSprite; }
    public PokemonType Type1 { get => type1; }
    public PokemonType Type2 { get => type2; }
}

//�o����Z�N���X�F�ǂ̃��x���Ŋo����̂�
[Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase _base;
    [SerializeField] int level;

    public MoveBase Base1 { get => _base; }
    public int Level { get => level; }
}
public enum PokemonType
{
    None, //����
    Normal, //�m�[�}��
    Fire, //��
    Water,//��
    Electric, //�d�C
    Grass,//��
    Ice,  //�X
    Fighting,//�i��
    Poison, //��
    Ground, //�n��
    Flying, //��s
    Psychic, //�G�X�p�[
    Bug,     //��
    Rock,    //��
    Ghost,   //�S�[�X�g
    Dragon,�@//�h���S��
}
public class TypeChart
{
    static float[][] chart =
    {
        //�U��/�h��         NOR   FIR   WAT
        /*NOR*/ new float[]{ 1f,   1f,   1f},
        /*FIR*/ new float[]{ 1f, 0.5f, 0.5f},
        /*WAT*/ new float[]{ 1f,   2f, 0.5f},
    };
    public static float GetEffectivenss(PokemonType attackType, PokemonType defenseType)
    {
        if(attackType == PokemonType.None || defenseType == PokemonType.None)
        {
            return 1f;
        }
        int row = (int)attackType -1;
        int col = (int)defenseType -1;
        return chart[row][col];
    }
}
