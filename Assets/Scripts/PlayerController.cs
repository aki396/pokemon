using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//UnityActionを使うため必要
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    //プレイヤーの1マス移動

    [SerializeField] float moveSpeed;

    bool isMoving;

    Vector2 input;

    Animator animator;

    //壁判定のLayer
    [SerializeField] LayerMask solidObjectsLayer;

    //草むら判定のrayer
    [SerializeField] LayerMask LongGrassLayer;

    public UnityAction OnEncounted;
    //[SerializeField] GameController gameController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {

            //キーボードの入力方向に動く
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //斜め移動対策
            if(input.x != 0)
            {
                input.y = 0;
            }

            //入力があったら
            if (input != Vector2.zero)
            {
                //向きを変える
                animator.SetFloat("Move.x", input.x);
                animator.SetFloat("Move.y", input.y);
                Vector2 targetPos = transform.position;
                targetPos += input;
                if (IsWalkable(targetPos))
                {

                    StartCoroutine(Move(targetPos));

                }

            }
        }
        animator.SetBool("IsMoving", isMoving);
    }
    //コルーチンを使って徐々に目的地に近づける
    IEnumerator Move(Vector3 targetPos)
    {
        //:移動中は受け付けない
        isMoving = true;

        //targetPosとの差があるなら繰り返す
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //tragetPosに近づける
            transform.position = Vector3.MoveTowards(
                transform.position,         //現在の場所
                targetPos,                  //目的地
                moveSpeed * Time.deltaTime);//近づけるスピード
            yield return null;
        }
        transform.position = targetPos;
        //移動が終わったら受け付ける
        isMoving = false;

        CheckForEncounters();
    }
    //targetPosに移動可能か調べる関数
    bool IsWalkable(Vector2 targetPos)
    {
        //targetPosに半径0.2の円のRayを飛ばして、ぶつかったらtrue
        //その否定だから"！"
        bool hit = Physics2D.OverlapCircle(targetPos,0.01f,solidObjectsLayer);
        return !hit;
    }

    //自分の場所から、円のLayを飛ばして、草むらLayerに当たったらランダムエンカウント
    void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, LongGrassLayer))
        {
            //ランダムエンカウント
            if(Random.Range(0,100) < 10)
            {
                Debug.Log("モンスターに遭遇");
                // gameController.StartBattle();
                OnEncounted();
                animator.SetBool("isMoving", false);
            }
        }
    }

}
