using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//探して来てくれる
using System.Linq;

public class PokemonParty : MonoBehaviour
{
      //トレーナーのポケモンを管理
      [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons { get => pokemons;}

    private void Start()
    {
        //ゲーム開始時に初期化
        foreach(Pokemon pokemon in pokemons)
        {
            pokemon.Init();
        }
    }


    //戦闘可能なポケモンを渡す
    public Pokemon GetHealthyPokemon()
    {
        //戦闘可能なポケモン(HP>0)
        return pokemons.Where(monster => monster.HP > 0).FirstOrDefault();
        //FirstOrDefault;一番最初の要素　or　要素が無ければdefault(intなら0,bool false)
    }
}
