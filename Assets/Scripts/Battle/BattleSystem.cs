using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;//UnityActionを使うため必要


public enum BattleState
{
    Start,
    ActionSelection, //行動選択
    MoveSelection,   //技選択
    PerformMove,    //技の実行
    Busy,         //処理中
    PartyScreen,  //ポケモン選択状態
    BattleOver,   //バトル終了状態
}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    //[SerializeField] GameController gameController;

    //バトルシステムとの相互依存を解消：UnityAction(関数を登録する)
    public UnityAction BattleOver;

    BattleState state;
    int currentAction; // 0:Fight 1:Run
    int currentMove; // 0:左上　1:右上　2:左下 3:右下
    int currentMember; //

    //これらの変数をどこから取得するの？
    PokemonParty playerParty;
    Pokemon wildPokemon;


    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }
    IEnumerator SetupBattle()
    {
        state = BattleState.Start;
        //モンスターの生成と描画
        playerUnit.Setup(playerParty.GetHealthyPokemon()); //plsyerの戦闘可能ポケモンをセット
        enemyUnit.Setup(wildPokemon);  //野生ポケモンをセット
        partyScreen.Init();
        playerHud.SetData(playerUnit.Pokemon);//HUDの描画
        enemyHud.SetData(enemyUnit.Pokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        // dialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name}apeared.");
        yield return dialogBox.TypeDialog($"野生の {enemyUnit.Pokemon.Base.Name}が飛び出して来た.");

        //メッセージがでて、1秒後にActionSerectorを表示する
        yield return new WaitForSeconds(1);
        ActionSelection();
    }
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.EnableActionSelector(true);
        StartCoroutine(dialogBox.TypeDialog($"どうする？"));
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);
    }

    void OpenPartyAction()
    {
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetPartyData(playerParty.Pokemons);
        //パーティスクリーンを表示
        //ポケモンデータを反映
    }
    //faintedUnit:やられたモンスター
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        //やられたモンスターが
        //PlayerUnitなら


        if (faintedUnit.IsPlayerUnit)
        {

            //バトルシステムとの相互依存を解消：UnityAction(関数を登録する)
            //戦えるポケモンがいるなら、次のポケモンをセットして、自分のターンにする
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon == null)
            {
                //いないならバトル終了
                state = BattleState.BattleOver;
                BattleOver();
            }
            else
            {
                //他にモンスターがいるなら選択画面
                OpenPartyAction();
            }
        }
        else
        {
            //enemyUnitならバトル終了
            BattleOver();
        }
    }

    //PlayerMoveの実行
    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        //技を決定
        Move move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMove)
        {
            //戦闘可能ならEnemyMove
            StartCoroutine(EnemyMove());
        }



    }
    //敵のターン
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        //技を決定:ランダム
        Move move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit,playerUnit,move);

        if(state == BattleState.PerformMove)
        {
            //戦闘可能ならEnemyMove
            ActionSelection();
        }
    }
    //技の実装(実行するUnit,対象Unit,技)
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}は{move.Base.Name}を使った");
        yield return new WaitForSeconds(1);
        sourceUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        targetUnit.PlayerHitAnimation();

        //Enemyダメージ計算
        DamageDetails damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);

        // HP反映
        yield return playerHud.UpdateHP(); //TODO:後で修正

        //相性/クリティカルのメッセージ
        yield return ShowDamageDetails(damageDetails);


        //戦闘不能ならメッセージ

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name}は戦闘不能になった");
            targetUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.7f);
            //バトル終了
            //gameController.EndBattle();
            //バトルシステムとの相互依存を解消：UnityAction(関数を登録する)
            //戦えるポケモンがいるなら、次のポケモンをセットして、自分のターンにする
            CheckForBattleOver(targetUnit);
        }
    }

        IEnumerator ShowDamageDetails(DamageDetails damageDetails)
        {
            if (damageDetails.Critical > 1f)
            {
                yield return dialogBox.TypeDialog($"急所に当たった！");
            }
            if (damageDetails.TypeEffectiveness > 1f)
            {
                yield return dialogBox.TypeDialog($"効果は抜群だ！");
            }
            else if (damageDetails.TypeEffectiveness < 1f)
            {
                yield return dialogBox.TypeDialog($"効果はいまひとつ！");
            }

        }
        //Zボタンを押すとMoveSelectorとMoveDetailsを表示する
        public void HandleUpdate()
        {
            if (state == BattleState.ActionSelection)
            {
                HandleActionSelection();
            }
            else if (state == BattleState.MoveSelection)
            {
                HandleMoveSelection();
            }
            else if (state == BattleState.PartyScreen)
            {
                HandlePartySelection();
            }
    }
        //PlayerActionでの行動を処理する
        void HandleActionSelection()
        {
            //キーボードの下を入力するとRun、上を入力するとFightになる
            //0:fight  1:Bag
            //2:Pokemon3:Run

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentAction++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentAction--;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentAction += 2;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentAction -= 2;
            }
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        //色をつけてどちらを選択してるかわかるようにする
        dialogBox.UpdateActionSelection(currentAction);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (currentAction == 0)
                {
                    MoveSelection();
                }
                if(currentAction == 2)
                {
                OpenPartyAction();

                }

            }
        }

        void HandleMoveSelection()
        {
            //キーボードの下を入力するとRun、上を入力するとFightになる
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
            currentMove++;

            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
            currentMove--;


            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
            currentMove += 2;

            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
            currentMove -= 2;

            }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);


        //色をつけてどちらを選択してるかわかるようにする
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                //技決定
                //技選択のUI非表示
                dialogBox.EnableMoveSelector(false);
                //メッセージ復活
                dialogBox.EnableDialogText(true);
                //技決定の処理
                StartCoroutine(PlayerMove());

            }
        //キャンセル
        if (Input.GetKeyDown(KeyCode.X))
        {
            //技決定
            //技選択のUI非表示
            dialogBox.EnableMoveSelector(false);
            //メッセージ復活
            dialogBox.EnableDialogText(true);
            //PlayerActionにする
            ActionSelection();

        }
    }

        void HandlePartySelection()
    {
        //キーボードの下を入力するとRun、上を入力するとFightになる
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember++;

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember--;


        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;

        }

        currentMove = Mathf.Clamp(currentMove, 0, playerParty.Pokemons.Count - 1);

        //選択中のモンスター名に色をつける
        //色をつけてどちらを選択してるかわかるようにする
        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //モンスター決定
            Pokemon selectedMember = playerParty.Pokemons[currentMember];
            //入れ替える：現在のキャラと戦闘不能は無理
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessage("そのモンスターは瀕死です");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessage("同じモンスターです。");
                return;
            }

            //ポケモン選択画面を消す
            partyScreen.gameObject.SetActive(false);
            //状態をBUSYにする
            state = BattleState.Busy;
            //入れ替えの処理をする
            StartCoroutine(SwitchPokemon(selectedMember));

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            //ポケモン選択画面を閉じたい
            partyScreen.gameObject.SetActive(false);
            ActionSelection();

        }
    }
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        //前のやつを下げる
        //修正：戦闘不能なら戻れはいらない＆相手のターンにならない
        bool fainted = playerUnit.Pokemon.HP <= 0;
        if (!fainted)
        {
            yield return dialogBox.TypeDialog($"戻れ{playerUnit.Pokemon.Base.Name}!");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }


        //新しいのをセット
        //nextをセット
        //モンスターの生成と描画
        playerUnit.Setup(newPokemon); //plsyerの戦闘可能ポケモンをセット
        playerHud.SetData(playerUnit.Pokemon);//HUDの描画
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        // dialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name}apeared.");
        yield return dialogBox.TypeDialog($"いけ{playerUnit.Pokemon.Base.Name}！");

        if (fainted)
        {
            ActionSelection();
        }
        else
        {
            //メッセージがでて、1秒後にActionSerectorを表示する
            StartCoroutine(EnemyMove());
        }


        //PlayerAction();戦闘不能での入れ替えなら自分のターン
    }


}
