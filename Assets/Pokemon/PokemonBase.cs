using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ポケモンのマスクデータ：外部から変更しない(インスペクターからのみ変更可能）
[CreateAssetMenu]
public class PokemonBase : ScriptableObject
{
    //名前、説明、画像、タイプ、ステータス

    [SerializeField] new string name;
    [SerializeField] string description;

    //画像
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    //タイプ
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //ステータス:hp,at,def,sAT,sDF,sp
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    //覚える技一覧

    [SerializeField] List<LearnableMove> learnableMoves;

    //他ファイルからattackの値は取得出来るが変更は出来ない
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

//覚える技クラス：どのレベルで覚えるのか
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
    None, //無し
    Normal, //ノーマル
    Fire, //火
    Water,//水
    Electric, //電気
    Grass,//草
    Ice,  //氷
    Fighting,//格闘
    Poison, //毒
    Ground, //地面
    Flying, //飛行
    Psychic, //エスパー
    Bug,     //虫
    Rock,    //岩
    Ghost,   //ゴースト
    Dragon,　//ドラゴン
}
public class TypeChart
{
    static float[][] chart =
    {
        //攻撃/防御         NOR   FIR   WAT
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
