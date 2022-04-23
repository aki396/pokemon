using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//DOTween�g�p���ɕK�v
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    //�ύX�F�����X�^�[��BattleSystem����󂯎��
   // [SerializeField] PokemonBase _base; //�키�����X�^�[���Z�b�g����
   // [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }
    public bool IsPlayerUnit { get => isPlayerUnit; }

    Vector3 originalPos;
    Color originalColor;
    Image image;
    //�o�g���Ŏg�������X�^�[��ێ�
    //�����X�^�[�̉摜�𔽉f����

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = transform.localPosition;
        originalColor = image.color;
    }
    //�o�g���V�X�e������󂯎��ׂɈ�����Pokemon�������
    public void Setup(Pokemon pokemon)
    {
        //_base���烌�x���ɉ����������X�^�[�𐶐�����
        //BattleSystem�Ŏg������v���p�e�B�ɂ����

        // Pokemon = new Pokemon(_base, level);//�o�g���V�X�e������󂯎��O
        
        Pokemon = pokemon;

        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }
        //���퓬������̓����ɂȂ��Ă�̂̉����̂���
        image.color = originalColor;
        PlayerEnterAnimation();

    }
    //�o��Anim
    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
        {
            //���[�ɔz�u
            transform.localPosition = new Vector3(-850, originalPos.y);
        }
        else
        {
            //�E�[�ɔz�u
            transform.localPosition = new Vector3(850, originalPos.y);
        }
        //�퓬���̈ʒu�܂ŃA�j���[�V����
        transform.DOLocalMoveX(originalPos.x, 0.5f);
    }
    //�U��Anim
    public void PlayerAttackAnimation()
    {
        //�V�[�P���X
        //�E�ɓ�������A���̈ʒu�ɖ߂�
        Sequence sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(transform.DOLocalMoveX(originalPos.x + 50, 0.25f));//���ɒǉ�
        }
        else
        {
            sequence.Append(transform.DOLocalMoveX(originalPos.x - 50, 0.25f));//���ɒǉ�
        }
        sequence.Append(transform.DOLocalMoveX(originalPos.x, 0.2f));//����Ɍ��ɒǉ�
    }
    //�_���[�WAnim
    public void PlayerHitAnimation()
    {
        //�F����xGLAY�ɂ��Ă���߂�
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }
    //�퓬�s�\Anim
    public void PlayerFaintAnimation()
    {
        //���ɉ�����Ȃ��甖���Ȃ�
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(originalPos.y -150f, 0.1f));
        sequence.Join(image.DOFade(0, 0.5f));
    }

}
