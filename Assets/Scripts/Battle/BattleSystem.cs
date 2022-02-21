using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;//UnityAction���g�����ߕK�v


public enum BattleState
{
    Start,
    PlayerAction, //�s���I��
    PlayerMove,   //�Z�I��
    EnemyMove,
    Busy,         //������
    PartyScreen,  //�|�P�����I�����
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

    //�o�g���V�X�e���Ƃ̑��݈ˑ��������FUnityAction(�֐���o�^����)
    public UnityAction BattleOver;

    BattleState state;
    int currentAction; // 0:Fight 1:Run
    int currentMove; // 0:����@1:�E��@2:���� 3:�E��
    int currentMember; //

    //�����̕ϐ����ǂ�����擾����́H
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
        //�����X�^�[�̐����ƕ`��
        playerUnit.Setup(playerParty.GetHealthyPokemon()); //plsyer�̐퓬�\�|�P�������Z�b�g
        enemyUnit.Setup(wildPokemon);  //�쐶�|�P�������Z�b�g
        partyScreen.Init();
        playerHud.SetData(playerUnit.Pokemon);//HUD�̕`��
        enemyHud.SetData(enemyUnit.Pokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        // dialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name}apeared.");
        yield return dialogBox.TypeDialog($"�쐶�� {enemyUnit.Pokemon.Base.Name}����яo���ė���.");

        //���b�Z�[�W���łāA1�b���ActionSerector��\������
        yield return new WaitForSeconds(1);
        PlayerAction();
    }
    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogBox.EnableActionSelector(true);
        StartCoroutine(dialogBox.TypeDialog($"�ǂ�����H"));
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
        //�p�[�e�B�X�N���[����\��
        //�|�P�����f�[�^�𔽉f
    }

    //PlayerMove�̎��s
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        //�Z������
        Move move = playerUnit.Pokemon.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name}��{move.Base.Name}���g����");
        yield return new WaitForSeconds(1);
        playerUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        enemyUnit.PlayerHitAnimation();

        //Enemy�_���[�W�v�Z
        DamageDetails damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);

        // HP���f
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        //�퓬�s�\�Ȃ烁�b�Z�[�W

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name}�͐퓬�s�\�ɂȂ���");
            enemyUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.7f);
            //�o�g���I��
            // gameController.EndBattle();
            //�o�g���V�X�e���Ƃ̑��݈ˑ��������FUnityAction(�֐���o�^����)
            BattleOver();

        }
        else
        {
            //�퓬�\�Ȃ�EnemyMove
            StartCoroutine(EnemyMove());
        }

    }
    //�G�̃^�[��
    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        //�Z������:�����_��
        Move move = enemyUnit.Pokemon.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name}��{move.Base.Name}���g����");
        yield return new WaitForSeconds(1);
        enemyUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        playerUnit.PlayerHitAnimation();

        //Enemy�_���[�W�v�Z
        DamageDetails damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);

        // HP���f
        yield return playerHud.UpdateHP();

        //����/�N���e�B�J���̃��b�Z�[�W
        yield return ShowDamageDetails(damageDetails);


        //�퓬�s�\�Ȃ烁�b�Z�[�W

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name}�͐퓬�s�\�ɂȂ���");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.7f);
            //�o�g���I��
            //gameController.EndBattle();
            //�o�g���V�X�e���Ƃ̑��݈ˑ��������FUnityAction(�֐���o�^����)
            //�킦��|�P����������Ȃ�A���̃|�P�������Z�b�g���āA�����̃^�[���ɂ���
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon == null)
            {
                BattleOver();
            }
            else
            {
                //next���Z�b�g
                //�����X�^�[�̐����ƕ`��
                playerUnit.Setup(nextPokemon); //plsyer�̐퓬�\�|�P�������Z�b�g
                playerHud.SetData(playerUnit.Pokemon);//HUD�̕`��
                dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                // dialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name}apeared.");
                yield return dialogBox.TypeDialog($"����{playerUnit.Pokemon.Base.Name}�I");

                //���b�Z�[�W���łāA1�b���ActionSerector��\������
                yield return new WaitForSeconds(1);
                PlayerAction();
            }
        }
        else
        {
            //�퓬�\�Ȃ�EnemyMove
            StartCoroutine(PerformPlayerMove());
        }
    }

        IEnumerator ShowDamageDetails(DamageDetails damageDetails)
        {
            if (damageDetails.Critical > 1f)
            {
                yield return dialogBox.TypeDialog($"�}���ɓ��������I");
            }
            if (damageDetails.TypeEffectiveness > 1f)
            {
                yield return dialogBox.TypeDialog($"���ʂ͔��Q���I");
            }
            else if (damageDetails.TypeEffectiveness < 1f)
            {
                yield return dialogBox.TypeDialog($"���ʂ͂��܂ЂƂI");
            }

        }
        //Z�{�^����������MoveSelector��MoveDetails��\������
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
        //PlayerAction�ł̍s������������
        void HandleActionSelection()
        {
            //�L�[�{�[�h�̉�����͂����Run�A�����͂����Fight�ɂȂ�
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

        //�F�����Ăǂ����I�����Ă邩�킩��悤�ɂ���
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
            //�L�[�{�[�h�̉�����͂����Run�A�����͂����Fight�ɂȂ�
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


        //�F�����Ăǂ����I�����Ă邩�킩��悤�ɂ���
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                //�Z����
                //�Z�I����UI��\��
                dialogBox.EnableMoveSelector(false);
                //���b�Z�[�W����
                dialogBox.EnableDialogText(true);
                //�Z����̏���
                StartCoroutine(PerformPlayerMove());

            }
        }

        void HandlePartySelection()
    {
        //�L�[�{�[�h�̉�����͂����Run�A�����͂����Fight�ɂȂ�
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

        //�I�𒆂̃����X�^�[���ɐF������
        //�F�����Ăǂ����I�����Ă邩�킩��悤�ɂ���
        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //�����X�^�[����

        }
    }
    
}
