using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    //�Z�̃}�X�^�[�f�[�^


    //���O�A�ڍׁA�^�C�v�A�З́A���m���APP(�Z���g���������|�C���g)

    [SerializeField] new string name;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;

    [SerializeField] int power;
    [SerializeField] int accuracy; //���m��
    [SerializeField] int pp;

    //�V���A���C�Y�t�B�[���h�Ȃ̂ő��̃t�@�C��(Move.cs��)����Q�Əo���Ȃ��̂�
    //�v���p�e�B���g��
    public string Name { get => name; }
    public string Description { get => description; }
    public PokemonType Type { get => type; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public int PP { get => pp; }



}
