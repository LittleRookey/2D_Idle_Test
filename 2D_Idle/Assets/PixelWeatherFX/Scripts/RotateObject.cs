using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotateObject : MonoBehaviour {
    public bool rotate = true;
    [SerializeField] private float Speed = 10;
    public bool useMaterial = true;

    private SpriteRenderer sprite;
    private MaterialPropertyBlock mpb;
    private static readonly int RotateEnableProperty = Shader.PropertyToID("_ROTATEUV_ON");
    private static readonly int RotateAmountProperty = Shader.PropertyToID("_RotateUvAmount");

    private Tween rotateTween;

    void Awake()
    {
        rotate = true;
        sprite = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();

        SetRotateEnable(useMaterial);
    }

    private void OnEnable()
    {
        if (useMaterial)
        {
            StartMaterialRotation();
        }
    }

    private void OnDisable()
    {
        rotateTween?.Kill();
    }

    private void SetRotateEnable(bool enable)
    {
        sprite.GetPropertyBlock(mpb);
        mpb.SetFloat(RotateEnableProperty, enable ? 1f : 0f);
        sprite.SetPropertyBlock(mpb);
    }

    private void StartMaterialRotation()
    {
        float startVal = 0f;
        SetRotateAmount(startVal);

        rotateTween = DOTween.To(() => startVal, SetRotateAmount, 6.28f, 5f)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    private void SetRotateAmount(float amount)
    {
        sprite.GetPropertyBlock(mpb);
        mpb.SetFloat(RotateAmountProperty, amount);
        sprite.SetPropertyBlock(mpb);
    }

    void Update()
    {
        if (useMaterial)
        {
            return;
        }
        if (rotate)
        {
            transform.Rotate(Vector3.forward * (Speed * Time.deltaTime));
        }
    }
}
