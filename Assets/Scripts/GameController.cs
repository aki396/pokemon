using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    FreeRoam, //�}�b�v�ړ�
    Battle,
}

public class GameController : MonoBehaviour
{
    //�Q�[���̏�Ԃ��Ǘ�
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;


    GameState state = GameState.FreeRoam;

    //�o�g���V�X�e���Ƃ̑��݈ˑ��������FUnityAction(�֐���o�^����)
    private void Start()
    {
        playerController.OnEncounted += StartBattle;
        battleSystem.BattleOver += EndBattle;
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        //�p�[�e�B�Ɩ쐶�|�P�����̎擾
        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        //FindObjectOfType:�V�[���Ȃ������v����R���|�[�l���g��1�擾����
        Pokemon wildPokemon = FindObjectOfType<MapArea>().GetRandomWildPokemon();
        battleSystem.StartBattle(playerParty, wildPokemon);
    }
    public void EndBattle()
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
    void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }  
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
