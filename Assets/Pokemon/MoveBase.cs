using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    //技のマスターデータ


    //名前、詳細、タイプ、威力、正確性、PP(技を使う時消費するポイント)

    [SerializeField] new string name;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;

    [SerializeField] int power;
    [SerializeField] int accuracy; //正確性
    [SerializeField] int pp;

    //シリアライズフィールドなので他のファイル(Move.cs等)から参照出来ないので
    //プロパティを使う
    public string Name { get => name; }
    public string Description { get => description; }
    public PokemonType Type { get => type; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public int PP { get => pp; }



}
