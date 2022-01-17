using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    FreeRoam, //マップ移動
    Battle,
}

public class GameController : MonoBehaviour
{
    //ゲームの状態を管理
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;


    GameState state = GameState.FreeRoam;

    //バトルシステムとの相互依存を解消：UnityAction(関数を登録する)
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
        //パーティと野生ポケモンの取得
        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        //FindObjectOfType:シーンないから一致するコンポーネントを1つ取得する
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
