using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotateObject : MonoBehaviour {
	public bool rotate=true;
	[SerializeField] private float Speed = 10;
	public bool useMaterial = true;

	SpriteRenderer sprite;
	Material mat;

	string rotateEnableKeyword = "ROTATEUV_ON";
	string rotateKeyword = "_RotateUvAmount";
	void Awake() {
		rotate = true;
		sprite = GetComponent<SpriteRenderer>();
		mat = sprite.material;
		if (useMaterial)
        {
			mat.EnableKeyword(rotateEnableKeyword);
        }
		else
        {
			mat.DisableKeyword(rotateEnableKeyword);
		}
	}

    private void OnEnable()
    {
        if (useMaterial)
        {
			float startVal = 0f;
			mat.SetFloat(rotateKeyword, startVal);
			DOTween.To(() => startVal, x => {
				startVal = x;
				mat.SetFloat(rotateKeyword, startVal);
			}, 6.28f, 5f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
		}
    }

    // Update is called once per frame
    void Update () {
		if (useMaterial)
        {
			return;
		}
		if(rotate == true){
			transform.Rotate ( Vector3.forward * (Speed * Time.deltaTime ) );
		}
	}
}
