using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ACF.Tests
{
	// [ExecuteAlways]
	public class HealthBarTest : MonoBehaviour
	{
		private const string STEP = "_Steps";
		private const string RATIO = "_HSRatio";
		private const string WIDTH = "_Width";
		private const string THICKNESS = "_Thickness";

		private static readonly int floatSteps = Shader.PropertyToID(STEP);
		private static readonly int floatRatio = Shader.PropertyToID(RATIO);
		private static readonly int floatWidth = Shader.PropertyToID(WIDTH);
		private static readonly int floatThickness = Shader.PropertyToID(THICKNESS);

		public float Hp;
		public float MaxHp;
		public float Sp;

		public float hpShieldRatio;
		public float RectWidth = 100f;
		[Range(0, 5f)] public float Thickness = 2f;

		public Image hp;
		public Image damaged;
		public Image sp;
		public Image separator;
		Vector3 newSPVec;
		Vector3 newHPVec;

		[SerializeField] private Health targetHealth;
		[SerializeField] private Transform orientation;
		public bool useMaterial;
        //[ContextMenu("Create Material")]
        //private void CreateMaterial()
        //{
        //	// if (separator.material == null)
        //	{
        //		separator.material = new Material(Shader.Find("ABS/UI/Health Separator"));
        //	}
        //}

        private void Awake()
        {
			if (targetHealth == null)
			{
				if (transform.parent.TryGetComponent<Health>(out Health targHealth))
					targetHealth = targHealth;
			}
			
        }
        private void Start()
		{
			newSPVec = Vector3.one;
			newHPVec = Vector3.one;
			newDamagedVec = Vector3.one;

			Hp = targetHealth.CurrentHealth;
			MaxHp = targetHealth.MaxHealth;
			UpdateHealth(Hp, MaxHp);
			Sp = 0;

			//			while (Sp > 0)
			//			{
			//				Sp -= 280 * Time.deltaTime;
			//				yield return null;
			//			}

			//			Sp = 0;

			//			yield return new WaitForSeconds(2f);

			//			for (int i = 0; i < 8; i++)
			//			{
			//				Hp -= 120;
			//				yield return new WaitForSeconds(1f);
			//			}

			//			for (int i = 0; i < 8; i++)
			//			{
			//				MaxHp += 200;
			//				Hp = MaxHp;

			//				yield return new WaitForSeconds(1f);
			//			}

			//#if UNITY_EDITOR
			//			UnityEditor.EditorApplication.isPlaying = false;
			//#endif
		}

		private void OnEnable()
		{
			targetHealth.onTakeDamage += UpdateHealth;
			targetHealth.OnDeath += DisableHealthBar;
			targetHealth.OnReturnFromPool += ResetHealthBar;
		}

		private void OnDisable()
		{
			targetHealth.onTakeDamage -= UpdateHealth;
			targetHealth.OnDeath -= DisableHealthBar;
			targetHealth.OnReturnFromPool -= ResetHealthBar;
		}

		public void DisableHealthBar(LevelSystem attacker)
        {
			orientation.gameObject.SetActive(false);

		}

		public void ResetHealthBar()
        {
			sp.transform.localScale = Vector3.one;
			hp.transform.localScale = Vector3.one;
			damaged.transform.localScale = Vector3.one;
			newSPVec = Vector3.one;
			newHPVec = Vector3.one;
			newDamagedVec = Vector3.one;

			Hp = targetHealth.CurrentHealth;
			MaxHp = targetHealth.MaxHealth;
			UpdateHealth(Hp, MaxHp);
			Sp = 0;

			separator.material.SetFloat(floatSteps, Hp/300f);
			orientation.gameObject.SetActive(true);
			Debug.Log("HealthBar Reset");
		}
		public void UpdateHealth(float current, float max)
		{
			float step;
			// 쉴드가 존재 할 때
			if (Sp > 0)
			{
				// 현재체력 + 쉴드 > 최대 체력
				if (Hp + Sp > MaxHp)
				{
					hpShieldRatio = Hp / (Hp + Sp);

					sp.transform.localScale = newSPVec;
					step = (Hp) / 300f;
					newHPVec.x = Hp / (Hp + Sp);
					hp.transform.localScale = newHPVec;
					//hp.fillAmount = Hp / (Hp + Sp);
				}
				else
				{
					newSPVec.x = (Hp + Sp) / MaxHp;
					sp.transform.localScale = newSPVec;
					hpShieldRatio = Hp / MaxHp;
					step = Hp / 300f;

					newHPVec.x = Hp / MaxHp;
					hp.transform.localScale = newHPVec;
				}
			}
			else
			{
				newSPVec.x = 0f;
				sp.transform.localScale = newSPVec;
				step = MaxHp / 300f;
				hpShieldRatio = 1f;

				newHPVec.x = current / max;
				hp.transform.localScale = newHPVec;
			}

			//damaged.fillAmount = Mathf.Lerp(damaged.fillAmount, hp.fillAmount, Time.deltaTime * speed);
			DOTween.To(() => damaged.fillAmount, x => damaged.fillAmount = x, current / max, 0.2f);
			separator.material.SetFloat(floatSteps, step);
			separator.material.SetFloat(floatRatio, hpShieldRatio);
			separator.material.SetFloat(floatWidth, RectWidth);
			separator.material.SetFloat(floatThickness, Thickness);
		}

		Vector3 newDamagedVec;

		private void Update()
		{
			//	if (MaxHp < Hp)
			//	{
			//		MaxHp = Hp;
			//	}

			//	float step;

			//	// 쉴드가 존재 할 때
			//	if (Sp > 0)
			//	{
			//		// 현재체력 + 쉴드 > 최대 체력
			//		if (Hp + Sp > MaxHp)
			//		{
			//			hpShieldRatio = Hp / (Hp + Sp);

			//			sp.transform.localScale = newSPVec;
			//			step = (Hp) / 300f;
			//			newHPVec.x = Hp / (Hp + Sp);
			//			hp.transform.localScale = newHPVec;
			//			//hp.fillAmount = Hp / (Hp + Sp);
			//		}
			//		else
			//		{
			//			newSPVec.x = (Hp + Sp) / MaxHp;
			//			sp.transform.localScale = newSPVec;
			//			hpShieldRatio = Hp / MaxHp;
			//			step = Hp / 300f;

			//			newHPVec.x = Hp / MaxHp;
			//			hp.transform.localScale = newHPVec;
			//		}
			//	}
			//	else
			//	{
			//		newSPVec.x = 0f;
			//		sp.transform.localScale = newSPVec;
			//		step = MaxHp / 300f;
			//		hpShieldRatio = 1f;

			//		newHPVec.x = Hp / MaxHp;
			//		hp.transform.localScale = newHPVec;
			//	}

			//          // sp.fillAmount = 1 - hpShieldRatio;

			//          damaged.fillAmount = Mathf.Lerp(damaged.fillAmount, hp.fillAmount, Time.deltaTime * speed);
			//          //newDamagedVec.x = Mathf.Lerp(newDamagedVec.x, hp.fillAmount, Time.deltaTime * speed);
			//          //damaged.transform.localScale = newDamagedVec;

			//          separator.material.SetFloat(floatSteps, step);
			//	separator.material.SetFloat(floatRatio, hpShieldRatio);
			//	separator.material.SetFloat(floatWidth, RectWidth);
			//	separator.material.SetFloat(floatThickness, Thickness);
			//}
		}
	}
}