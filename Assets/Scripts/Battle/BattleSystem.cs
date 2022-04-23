using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;//UnityAction���g�����ߕK�v


public enum BattleState
{
    Start,
    ActionSelection, //�s���I��
    MoveSelection,   //�Z�I��
    PerformMove,    //�Z�̎��s
    Busy,         //������
    PartyScreen,  //�|�P�����I�����
    BattleOver,   //�o�g���I�����
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
        ActionSelection();
    }
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.EnableActionSelector(true);
        StartCoroutine(dialogBox.TypeDialog($"�ǂ�����H"));
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
        //�p�[�e�B�X�N���[����\��
        //�|�P�����f�[�^�𔽉f
    }
    //faintedUnit:���ꂽ�����X�^�[
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        //���ꂽ�����X�^�[��
        //PlayerUnit�Ȃ�


        if (faintedUnit.IsPlayerUnit)
        {

            //�o�g���V�X�e���Ƃ̑��݈ˑ��������FUnityAction(�֐���o�^����)
            //�킦��|�P����������Ȃ�A���̃|�P�������Z�b�g���āA�����̃^�[���ɂ���
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon == null)
            {
                //���Ȃ��Ȃ�o�g���I��
                state = BattleState.BattleOver;
                BattleOver();
            }
            else
            {
                //���Ƀ����X�^�[������Ȃ�I�����
                OpenPartyAction();
            }
        }
        else
        {
            //enemyUnit�Ȃ�o�g���I��
            BattleOver();
        }
    }

    //PlayerMove�̎��s
    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        //�Z������
        Move move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMove)
        {
            //�퓬�\�Ȃ�EnemyMove
            StartCoroutine(EnemyMove());
        }



    }
    //�G�̃^�[��
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        //�Z������:�����_��
        Move move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit,playerUnit,move);

        if(state == BattleState.PerformMove)
        {
            //�퓬�\�Ȃ�EnemyMove
            ActionSelection();
        }
    }
    //�Z�̎���(���s����Unit,�Ώ�Unit,�Z)
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}��{move.Base.Name}���g����");
        yield return new WaitForSeconds(1);
        sourceUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        targetUnit.PlayerHitAnimation();

        //Enemy�_���[�W�v�Z
        DamageDetails damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);

        // HP���f
        yield return playerHud.UpdateHP(); //TODO:��ŏC��

        //����/�N���e�B�J���̃��b�Z�[�W
        yield return ShowDamageDetails(damageDetails);


        //�퓬�s�\�Ȃ烁�b�Z�[�W

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name}�͐퓬�s�\�ɂȂ���");
            targetUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.7f);
            //�o�g���I��
            //gameController.EndBattle();
            //�o�g���V�X�e���Ƃ̑��݈ˑ��������FUnityAction(�֐���o�^����)
            //�킦��|�P����������Ȃ�A���̃|�P�������Z�b�g���āA�����̃^�[���ɂ���
            CheckForBattleOver(targetUnit);
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
                StartCoroutine(PlayerMove());

            }
        //�L�����Z��
        if (Input.GetKeyDown(KeyCode.X))
        {
            //�Z����
            //�Z�I����UI��\��
            dialogBox.EnableMoveSelector(false);
            //���b�Z�[�W����
            dialogBox.EnableDialogText(true);
            //PlayerAction�ɂ���
            ActionSelection();

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
            Pokemon selectedMember = playerParty.Pokemons[currentMember];
            //����ւ���F���݂̃L�����Ɛ퓬�s�\�͖���
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessage("���̃����X�^�[�͕m���ł�");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessage("���������X�^�[�ł��B");
                return;
            }

            //�|�P�����I����ʂ�����
            partyScreen.gameObject.SetActive(false);
            //��Ԃ�BUSY�ɂ���
            state = BattleState.Busy;
            //����ւ��̏���������
            StartCoroutine(SwitchPokemon(selectedMember));

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            //�|�P�����I����ʂ������
            partyScreen.gameObject.SetActive(false);
            ActionSelection();

        }
    }
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        //�O�̂��������
        //�C���F�퓬�s�\�Ȃ�߂�͂���Ȃ�������̃^�[���ɂȂ�Ȃ�
        bool fainted = playerUnit.Pokemon.HP <= 0;
        if (!fainted)
        {
            yield return dialogBox.TypeDialog($"�߂�{playerUnit.Pokemon.Base.Name}!");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }


        //�V�����̂��Z�b�g
        //next���Z�b�g
        //�����X�^�[�̐����ƕ`��
        playerUnit.Setup(newPokemon); //plsyer�̐퓬�\�|�P�������Z�b�g
        playerHud.SetData(playerUnit.Pokemon);//HUD�̕`��
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        // dialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name}apeared.");
        yield return dialogBox.TypeDialog($"����{playerUnit.Pokemon.Base.Name}�I");

        if (fainted)
        {
            ActionSelection();
        }
        else
        {
            //���b�Z�[�W���łāA1�b���ActionSerector��\������
            StartCoroutine(EnemyMove());
        }


        //PlayerAction();�퓬�s�\�ł̓���ւ��Ȃ玩���̃^�[��
    }


}
