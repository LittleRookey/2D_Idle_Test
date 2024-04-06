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


		[SerializeField] private Health targetHealth;
		[SerializeField] private Transform orientation;
		[SerializeField] private TextMeshProUGUI healthText;
		//[SerializeField] private TextMeshProUGUI expTe;
		public bool disableOnDeath;
		//[ContextMenu("Create Material")]
		//private void CreateMaterial()
		//{
		//	// if (separator.material == null)
		//	{
		//		separator.material = new Material(Shader.Find("ABS/UI/Health Separator"));
		//	}
		//}

		private void Start()
		{

			Hp = targetHealth.CurrentHealth;
			MaxHp = targetHealth.MaxHealth;
			UpdateHealth(Hp, MaxHp);
			Sp = 0;
			if (healthText!= null)
            {
				healthText.SetText($"{Hp} / {MaxHp}");
            }
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
			if (disableOnDeath)
				targetHealth.OnDeath += DisableHealthBar;
			targetHealth.OnReturnFromPool += ResetHealthBar;
		}

		private void OnDisable()
		{
			targetHealth.onTakeDamage -= UpdateHealth;
			if (disableOnDeath)
				targetHealth.OnDeath -= DisableHealthBar;
			targetHealth.OnReturnFromPool -= ResetHealthBar;
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
					healthText.SetText($"{previousHP.ToString("F0")} / {MaxHp}");
				}
			});
		}



	}
}