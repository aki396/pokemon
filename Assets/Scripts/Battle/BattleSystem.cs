using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;//UnityActionを使うため必要


public enum BattleState
{
    Start,
    PlayerAction, //行動選択
    PlayerMove,   //技選択
    EnemyMove,
    Busy,         //処理中
    PartyScreen,  //ポケモン選択状態
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
        PlayerAction();
    }
    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogBox.EnableActionSelector(true);
        StartCoroutine(dialogBox.TypeDialog($"どうする？"));
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
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

    //PlayerMoveの実行
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        //技を決定
        Move move = playerUnit.Pokemon.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name}は{move.Base.Name}を使った");
        yield return new WaitForSeconds(1);
        playerUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        enemyUnit.PlayerHitAnimation();

        //Enemyダメージ計算
        DamageDetails damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);

        // HP反映
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        //戦闘不能ならメッセージ

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name}は戦闘不能になった");
            enemyUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.7f);
            //バトル終了
            // gameController.EndBattle();
            //バトルシステムとの相互依存を解消：UnityAction(関数を登録する)
            BattleOver();

        }
        else
        {
            //戦闘可能ならEnemyMove
            StartCoroutine(EnemyMove());
        }

    }
    //敵のターン
    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        //技を決定:ランダム
        Move move = enemyUnit.Pokemon.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name}は{move.Base.Name}を使った");
        yield return new WaitForSeconds(1);
        enemyUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        playerUnit.PlayerHitAnimation();

        //Enemyダメージ計算
        DamageDetails damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);

        // HP反映
        yield return playerHud.UpdateHP();

        //相性/クリティカルのメッセージ
        yield return ShowDamageDetails(damageDetails);


        //戦闘不能ならメッセージ

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name}は戦闘不能になった");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.7f);
            //バトル終了
            //gameController.EndBattle();
            //バトルシステムとの相互依存を解消：UnityAction(関数を登録する)
            //戦えるポケモンがいるなら、次のポケモンをセットして、自分のターンにする
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon == null)
            {
                BattleOver();
            }
            else
            {
                //nextをセット
                //モンスターの生成と描画
                playerUnit.Setup(nextPokemon); //plsyerの戦闘可能ポケモンをセット
                playerHud.SetData(playerUnit.Pokemon);//HUDの描画
                dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                // dialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name}apeared.");
                yield return dialogBox.TypeDialog($"いけ{playerUnit.Pokemon.Base.Name}！");

                //メッセージがでて、1秒後にActionSerectorを表示する
                yield return new WaitForSeconds(1);
                PlayerAction();
            }
        }
        else
        {
            //戦闘可能ならEnemyMove
            StartCoroutine(PerformPlayerMove());
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
            if (state == BattleState.PlayerAction)
            {
                HandleActionSelection();
            }
            else if (state == BattleState.PlayerMove)
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
                    PlayerMove();
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
                StartCoroutine(PerformPlayerMove());

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

        }
    }
    
}
