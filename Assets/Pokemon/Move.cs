using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    //Pokemonが実際に使う時の技データ

    //技のマスターデータを持つ
    //使いやすい様にするためにPPを持つ

    //Pokemon.csが参照するのでPublicにしておく
    public MoveBase Base { get; set; }
    public int PP { get; set; }


    //初期設定
    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
}
