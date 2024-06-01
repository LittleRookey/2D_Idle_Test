using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Litkey.UI
{
	// [ExecuteAlways]
	public class PlayerHealthUI : MonoBehaviour
	{
		public float Hp;
		public float MaxHp;
		public float Sp;


		public Image hp;
		public Image damaged;
		public Image sp;


		[SerializeField] private PlayerHealth targetHealth;
		[SerializeField] private Transform orientation;
		[SerializeField] private TextMeshProUGUI healthText;
		[SerializeField] private Image hpBorderImage;
		[SerializeField] private DOTweenAnimation onHitAnim;
		private Material borderMat;
		//[SerializeField] private TextMeshProUGUI expTe;
		public bool disableOnDeath;

		private void Start()
		{
			borderMat = hpBorderImage.material;
			Hp = targetHealth.CurrentHealth;
			MaxHp = targetHealth.MaxHealth;
			UpdateHealth(Hp, MaxHp);
			Sp = 0;
		}

		private void OnEnable()
		{
			targetHealth.onTakeDamage += UpdateHealth;
			if (disableOnDeath)
				targetHealth.OnDeath.AddListener(DisableHealthBar);
			targetHealth.OnReturnFromPool += ResetHealthBar;
			targetHealth.OnHit.AddListener(UIHitEffect);
		}

		private void OnDisable()
		{
			targetHealth.onTakeDamage -= UpdateHealth;
			if (disableOnDeath)
				targetHealth.OnDeath.RemoveListener(DisableHealthBar);
			targetHealth.OnReturnFromPool -= ResetHealthBar;
			targetHealth.OnHit.RemoveListener(UIHitEffect);
		}

		public void DisableHealthBar(LevelSystem attacker)
		{
			orientation.gameObject.SetActive(false);

		}

		public void ResetHealthBar()
		{
			sp.fillAmount = 1f;
			hp.fillAmount = 1f;
			damaged.fillAmount = 1f;


			Hp = targetHealth.CurrentHealth;
			MaxHp = targetHealth.MaxHealth;
			UpdateHealth(Hp, MaxHp);
			Sp = 0;


			orientation.gameObject.SetActive(true);
			Debug.Log("HealthBar Reset");
		}
		public void UpdateHealth(float current, float max)
		{
			hp.fillAmount = current / max;
			

			float previousHP = Hp;

			Hp = current;
			MaxHp = max;
			//damaged.fillAmount = Mathf.Lerp(damaged.fillAmount, hp.fillAmount, Time.deltaTime * speed);
			DOTween.To(() => previousHP, x => previousHP = x, current, 0.2f).OnUpdate(() => 
			{
				damaged.fillAmount = previousHP / max;
				if (healthText != null)
				{
					healthText.SetText($"{previousHP.ToString("F0")} / {TMProUtility.GetColorText(MaxHp.ToString(), new Color(184, 56, 83))}");
				}
			});
		}

		private float startValue = 0f;
		private void UIHitEffect()
        {
			var hitSequence = DOTween.Sequence();

			hitSequence.Append(DOTween.To(() => startValue, x => startValue = x, 0.3f, 0.05f)
				.OnUpdate(() =>
				{
					borderMat.SetFloat("_HitEffectBlend", startValue);
				}))
                .AppendInterval(0.05f) // Optional: Delay before the hit effect fades out
                .Append(DOTween.To(() => startValue, x => startValue = x, 0f, 0.05f)
				.OnUpdate(() =>
				{
					borderMat.SetFloat("_HitEffectBlend", startValue);
				}))
				.Append(DOTween.To(() => startValue, x => startValue = x, 0.3f, 0.05f)
				.OnUpdate(() =>
				{
					borderMat.SetFloat("_HitEffectBlend", startValue);
				}))
				.AppendInterval(0.05f) // Optional: Delay before the hit effect fades out
				.Append(DOTween.To(() => startValue, x => startValue = x, 0f, 0.05f)
				.OnUpdate(() =>
				{
					borderMat.SetFloat("_HitEffectBlend", startValue);
				}));
			onHitAnim.DORestartById("Enter");
		}
		

	}
}