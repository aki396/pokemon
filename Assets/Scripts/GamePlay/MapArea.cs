using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    //�쐶�̃|�P�������Ǘ�����
    [SerializeField] List<Pokemon> pokemons;

    //�����_���œn��
    public Pokemon GetRandomWildPokemon()
    {
        int r = Random.Range(0, pokemons.Count);
        Pokemon pokemon = pokemons[r];
        pokemon.Init();  //�o����тɏ�����
        return pokemon;
    }
}
