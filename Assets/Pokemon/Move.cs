using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    //Pokemon�����ۂɎg�����̋Z�f�[�^

    //�Z�̃}�X�^�[�f�[�^������
    //�g���₷���l�ɂ��邽�߂�PP������

    //Pokemon.cs���Q�Ƃ���̂�Public�ɂ��Ă���
    public MoveBase Base { get; set; }
    public int PP { get; set; }


    //�����ݒ�
    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
}
