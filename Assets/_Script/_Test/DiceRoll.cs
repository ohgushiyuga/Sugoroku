using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;

public class DiceRoll : MonoBehaviour
{
    [Header("コンポーネント")]
    [SerializeField] private Rigidbody diceRigidbody;
    [SerializeField] private Camera mainCamera;

    [Header("設定")]
    [SerializeField] private float forcePower = 10f;
    [SerializeField] private float torquePower = 10f;
    [SerializeField] private Transform[] facePoints = new Transform[6];
    
    [Header("アニメーション")]
    [Tooltip("サイコロが転がるアニメーションの最低保証時間")]
    [SerializeField] private float rollAnimationTime = 2.0f;
    [Tooltip("結果を見せるためのズーム時間")]
    [SerializeField] private float zoomDuration = 1.0f;

    public static event Action<int> OnDiceResult;

    private Vector3 initialPosition;
    private bool isRollable = false;
    private bool isRolling = false;

    void Awake()
    {
        if (diceRigidbody == null) diceRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        initialPosition = transform.position;
    }
    
    void OnEnable()
    {
        if (GameState.isMapReady) { isRollable = true; }
        else { isRollable = false; }
    }

    /// <summary>
    /// UIのButtonコンポーネントから呼び出される
    /// </summary>
    public void RollDice()
    {
        if (!isRollable || isRolling) return;
        StartCoroutine(RollSequence());
    }

    /// <summary>
    /// サイコロを振ってから結果を確定させるまでの一連の流れを管理するコルーチン
    /// </summary>
    private IEnumerator RollSequence()
    {
        isRollable = false;
        isRolling = true;

        // 1. サイコロを投げる
        transform.position = initialPosition + Vector3.up * 2f;
        transform.rotation = Random.rotation;
        diceRigidbody.linearVelocity = Vector3.zero;
        diceRigidbody.angularVelocity = Vector3.zero;
        diceRigidbody.AddForce(Vector3.up * forcePower + Random.insideUnitSphere * forcePower, ForceMode.Impulse);
        diceRigidbody.AddTorque(Random.insideUnitSphere * torquePower, ForceMode.Impulse);

        // 2. アニメーションが終わるまで待つ
        yield return new WaitForSeconds(rollAnimationTime);

        // 3. サイコロが完全に止まるまで待つ
        while (!diceRigidbody.IsSleeping())
        {
            yield return null;
        }

        // 4. 結果を確定させ、ズームする
        int result = GetDiceResult();
        
        ZoomToDiceFace(result);
        
        // 5. ズームした状態で結果を見せる
        yield return new WaitForSeconds(zoomDuration);

        // 6. 結果を通知し、自分自身を非表示にする
        OnDiceResult?.Invoke(result);
        gameObject.SetActive(false);
        
        isRolling = false;
    }

    private int GetDiceResult()
    {
        float maxDot = -2f;
        int faceIndex = -1;
        for (int i = 0; i < facePoints.Length; i++)
        {
            if (facePoints[i] == null) continue;
            Vector3 dir = (facePoints[i].position - transform.position).normalized;
            float dot = Vector3.Dot(dir, Vector3.up);
            if (dot > maxDot)
            {
                maxDot = dot;
                faceIndex = i;
            }
        }
        return faceIndex + 1;
    }
    
    /// 指定された出目の面にカメラをズームさせる
    private void ZoomToDiceFace(int result)
    {
        if (mainCamera == null || result <= 0 || result > facePoints.Length) return;
        
        Transform face = facePoints[result - 1];
        if (face == null) return;
        
        Vector3 dir = (face.position - transform.position).normalized;
        Vector3 targetPos = face.position + dir * 3f;
        
        mainCamera.transform.position = targetPos;
        mainCamera.transform.LookAt(transform.position);
    }
}