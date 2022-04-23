using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//DOTween使用時に必要
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    //変更：モンスターはBattleSystemから受け取る
   // [SerializeField] PokemonBase _base; //戦うモンスターをセットする
   // [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }
    public bool IsPlayerUnit { get => isPlayerUnit; }

    Vector3 originalPos;
    Color originalColor;
    Image image;
    //バトルで使うモンスターを保持
    //モンスターの画像を反映する

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = transform.localPosition;
        originalColor = image.color;
    }
    //バトルシステムから受け取る為に引数にPokemonをいれる
    public void Setup(Pokemon pokemon)
    {
        //_baseからレベルに応じたモンスターを生成する
        //BattleSystemで使うからプロパティにいれる

        // Pokemon = new Pokemon(_base, level);//バトルシステムから受け取る前
        
        Pokemon = pokemon;

        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }
        //一回戦闘した後の透明になってるのの解除のため
        image.color = originalColor;
        PlayerEnterAnimation();

    }
    //登場Anim
    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
        {
            //左端に配置
            transform.localPosition = new Vector3(-850, originalPos.y);
        }
        else
        {
            //右端に配置
            transform.localPosition = new Vector3(850, originalPos.y);
        }
        //戦闘時の位置までアニメーション
        transform.DOLocalMoveX(originalPos.x, 0.5f);
    }
    //攻撃Anim
    public void PlayerAttackAnimation()
    {
        //シーケンス
        //右に動いた後、元の位置に戻る
        Sequence sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(transform.DOLocalMoveX(originalPos.x + 50, 0.25f));//後ろに追加
        }
        else
        {
            sequence.Append(transform.DOLocalMoveX(originalPos.x - 50, 0.25f));//後ろに追加
        }
        sequence.Append(transform.DOLocalMoveX(originalPos.x, 0.2f));//さらに後ろに追加
    }
    //ダメージAnim
    public void PlayerHitAnimation()
    {
        //色を一度GLAYにしてから戻す
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }
    //戦闘不能Anim
    public void PlayerFaintAnimation()
    {
        //下に下がりながら薄くなる
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(originalPos.y -150f, 0.1f));
        sequence.Join(image.DOFade(0, 0.5f));
    }

}
