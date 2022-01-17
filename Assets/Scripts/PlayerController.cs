using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//UnityAction���g�����ߕK�v
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    //�v���C���[��1�}�X�ړ�

    [SerializeField] float moveSpeed;

    bool isMoving;

    Vector2 input;

    Animator animator;

    //�ǔ����Layer
    [SerializeField] LayerMask solidObjectsLayer;

    //���ނ画���rayer
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

            //�L�[�{�[�h�̓��͕����ɓ���
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //�΂߈ړ��΍�
            if(input.x != 0)
            {
                input.y = 0;
            }

            //���͂���������
            if (input != Vector2.zero)
            {
                //������ς���
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
    //�R���[�`�����g���ď��X�ɖړI�n�ɋ߂Â���
    IEnumerator Move(Vector3 targetPos)
    {
        //:�ړ����͎󂯕t���Ȃ�
        isMoving = true;

        //targetPos�Ƃ̍�������Ȃ�J��Ԃ�
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //tragetPos�ɋ߂Â���
            transform.position = Vector3.MoveTowards(
                transform.position,         //���݂̏ꏊ
                targetPos,                  //�ړI�n
                moveSpeed * Time.deltaTime);//�߂Â���X�s�[�h
            yield return null;
        }
        transform.position = targetPos;
        //�ړ����I�������󂯕t����
        isMoving = false;

        CheckForEncounters();
    }
    //targetPos�Ɉړ��\�����ׂ�֐�
    bool IsWalkable(Vector2 targetPos)
    {
        //targetPos�ɔ��a0.2�̉~��Ray���΂��āA�Ԃ�������true
        //���̔ے肾����"�I"
        bool hit = Physics2D.OverlapCircle(targetPos,0.01f,solidObjectsLayer);
        return !hit;
    }

    //�����̏ꏊ����A�~��Lay���΂��āA���ނ�Layer�ɓ��������烉���_���G���J�E���g
    void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, LongGrassLayer))
        {
            //�����_���G���J�E���g
            if(Random.Range(0,100) < 10)
            {
                Debug.Log("�����X�^�[�ɑ���");
                // gameController.StartBattle();
                OnEncounted();
                animator.SetBool("isMoving", false);
            }
        }
    }

}
