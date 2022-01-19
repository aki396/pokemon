using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//�T���ė��Ă����
using System.Linq;

public class PokemonParty : MonoBehaviour
{
      //�g���[�i�[�̃|�P�������Ǘ�
      [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons { get => pokemons;}

    private void Start()
    {
        //�Q�[���J�n���ɏ�����
        foreach(Pokemon pokemon in pokemons)
        {
            pokemon.Init();
        }
    }


    //�퓬�\�ȃ|�P������n��
    public Pokemon GetHealthyPokemon()
    {
        //�퓬�\�ȃ|�P����(HP>0)
        return pokemons.Where(monster => monster.HP > 0).FirstOrDefault();
        //FirstOrDefault;��ԍŏ��̗v�f�@or�@�v�f���������default(int�Ȃ�0,bool false)
    }
}
