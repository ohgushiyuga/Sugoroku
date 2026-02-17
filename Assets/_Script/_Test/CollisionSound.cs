using UnityEngine;
public class CollisionSoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトのタグや名前で条件分岐することも可能
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}