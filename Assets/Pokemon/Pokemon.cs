using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レベルに応じたステータスの違うモンスターを生成するクラス
//注意：データのみ扱う純粋なC#のクラス
[System.Serializable]
public class Pokemon 
{
    //プロパティからデータを設定出来る様にする
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    //ベースとなるデータ
    public PokemonBase Base { get => _base; }

    public int Level { get => level; }

    public int HP { get; set; }

    //使える技
    public List<Move> Moves { get; set; }

    //コンストラクタ：生成時の初期設定 => 必要なくなったのでinit関数に変更
    public void Init()
    {

       // Base = pBase;

       // Level = pLevel;

        HP = MaxHP;

        Moves = new List<Move>();

        //使える技の設定：覚える技のレベル以上なら、Movesに追加
        foreach (LearnableMove LearnableMove in Base.LearnableMoves)
        {
            if (Level >= LearnableMove.Level)
            {
                //超えているんだったら技を覚える
                Moves.Add(new Move(LearnableMove.Base1));
            }

            //4つ以上の技は使えない
            if(Moves.Count >= 4)
            {
                break;
            }
       
        }
    }

    //levelに応じたステータスを返すもの：プロパティ(+処理を加えることが出来る)
    //関数バージョン
    //public int Attack()
    //{
    //    return Mathf.FloorToInt((_base.Attack * level) / 100f) + 5;
    //}

    //プロパティ
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

    //修正
    //・戦闘不能・クリティカル・相性の情報を渡す
    public DamageDetails TakeDamage(Move move,Pokemon attacker)
    {
        //クリティカル
        float critical = 1f;
        //6.25％でクリティカル
        if(Random.value * 100 <= 6.25f)
        {
            critical = 2f;
        }
        //相性
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
    public bool Fainted { get; set; } //戦闘不能かどうか
    public float Critical { get; set; } //クリティカルかどうか
    public float TypeEffectiveness { get; set; } //相性
}
