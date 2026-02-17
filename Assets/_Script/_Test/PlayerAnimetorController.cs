// ファイル名: PlayerAnimatorController.cs
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // PlayerMovementControllerから呼ばれる
    public void PlayWalkAnimation(Vector3 direction)
    {
        // Animatorのパラメータ（例: "isWalking"をtrueにするなど）を設定して
        // 歩くアニメーションを再生させる
        animator.SetBool("isWalking", true);

        // 必要であれば、方向によってアニメーションを切り替える
        // animator.SetFloat("MoveX", direction.x);
        // animator.SetFloat("MoveZ", direction.z);
    }

    // PlayerMovementControllerから呼ばれる
    public void PlayIdleAnimation()
    {
        // 待機アニメーションに戻す
        animator.SetBool("isWalking", false);
    }
}