using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class MessageDisplayerUI : PlayerUIBehaviour
	{
		private static MessageDisplayerUI instance;

		[SerializeField]
		private GameObject m_MessageTemplate;

		[SerializeField]
		private Color m_MessageColor;

		[SerializeField]
		private float m_FadeDelay = 3f;

		[SerializeField]
		private float m_FadeSpeed = 0.3f;


        public override void OnAttachment()
        {
			instance = this;
		}

		public static void PushMessage(string message)
		{
			if (instance != null)
				instance.Internal_PushMessage(message, instance.m_MessageColor, 16);
		}

		public static void PushMessage(string message, Color color = default, int lineHeight = 16)
		{
			if (instance != null)
				instance.Internal_PushMessage(message, color, lineHeight);
		} 

		private void Internal_PushMessage(string message, Color color = default, int lineHeight = 16)
		{
			GameObject messageObject = Instantiate(m_MessageTemplate, transform, false);
			messageObject.SetActive(true);
			messageObject.transform.SetAsLastSibling();

			Text text = messageObject.GetComponentInChildren<Text>();
			CanvasGroup group = messageObject.GetComponent<CanvasGroup>();

			if (text && group)
			{
				text.text = message.ToUpper();
				text.color = new Color(color.r, color.g, color.b, 1f);

				text.GetComponent<LayoutElement>().preferredHeight = lineHeight;

				group.alpha = color.a;
				StartCoroutine(FadeMessage(group));
			}
		}

		private IEnumerator FadeMessage(CanvasGroup group)
		{
			if (!group)
				yield break;

			yield return new WaitForSeconds(m_FadeDelay);
			
			while (group.alpha > Mathf.Epsilon)
			{
				group.alpha = Mathf.MoveTowards(group.alpha, 0f, Time.deltaTime * m_FadeSpeed);
				yield return null;
			}

			Destroy(group.gameObject);
		}
	}
}
