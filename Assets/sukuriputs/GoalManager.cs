using UnityEngine;
using System.Collections.Generic; // List���g�����߂ɕK�v

// ���ǉ��F�Q�[�����ʁi���ʁj���V�[���Ԃŋ��L�E�ێ����邽�߂̃N���X
// ������`���邱�ƂŁA���̃X�N���v�g����́uGameResultData���Ȃ��v�Ƃ����G���[�������܂�

public class GoalManager : MonoBehaviour
{
    [Header("�S�[����")]
    [Tooltip("�S�[�������v���C���[�ԍ������Ԃɒǉ�����܂��i�C���X�y�N�^�[�m�F�p�j")]
    public List<int> finishedRanking = new List<int>();

    // �Q�[���J�n���Ƀf�[�^�����Z�b�g
    void Start()
    {
        // �O��̃Q�[�����ʂ��c���Ă���Ƃ����Ȃ��̂ŃN���A����
        GameResultData.FinalRank.Clear();
        finishedRanking.Clear();
    }

    // �Փ˔���iTrigger�j
    void OnTriggerEnter(Collider other)
    {
        // �Ԃ��������肪 SimpleCharacterController �������Ă��邩�m�F
        SimpleCharacterController player = other.GetComponent<SimpleCharacterController>();

        if (player != null)
        {
            // �v���C���[�ԍ����擾
            int pNum = player.PlayerNumber;

            // �܂������L���O�Ɋ܂܂�Ă��Ȃ���Βǉ��i��d�S�[���h�~�j
            // ���[�J���̃��X�g(finishedRanking)�Ŕ���
            if (!finishedRanking.Contains(pNum))
            {
                // 1. ���̃X�N���v�g���̃��X�g�ɒǉ��i�C���X�y�N�^�[�\���p�j
                finishedRanking.Add(pNum);

                // 2. ���ÓI�N���X(GameResultData)�ɂ��ǉ��i���̃V�[���ւ̎󂯓n���p�j
                GameResultData.FinalRank.Add(pNum);

                int rank = finishedRanking.Count;
                Debug.Log($"<color=yellow>Player {pNum} Finished! Rank: {rank}</color>");

                // --- �I�v�V�����F�S�[�������瑀��ł��Ȃ����� ---
                // �v���C���[�̃X�N���v�g�𖳌������ē����Ȃ�����ꍇ
                player.enabled = false;

                // �����������������S�Ɏ~�߂����ꍇ�͈ȉ����ǉ�
                var rb = player.GetComponent<Rigidbody>();
                if (rb != null) rb.linearVelocity = Vector3.zero;
            }
        }
    }
}