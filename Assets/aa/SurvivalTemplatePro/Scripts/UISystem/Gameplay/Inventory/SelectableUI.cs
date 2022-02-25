using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class SelectableUI : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		#region Internal
		public enum State
		{
			Normal,
			Highlighted,
			Pressed,
		}

		[Serializable]
		public class Transition
		{
			public Color NormalColor = Color.grey;

			public Color HighlightedColor = Color.grey;

			public Color PressedColor = Color.grey;

			[Range(0.01f, 1f)]
			public float FadeDuration = 0.1f;
		}
		#endregion

		public event UnityAction<SelectableUI> onPointerDown;
		public event UnityAction<SelectableUI> onPointerUp;

		[SerializeField]
		protected Graphic _Graphic;

		[SerializeField]
		private Transition m_Transition;

		protected State m_State = State.Normal;

		protected bool m_Pressed;
		protected bool m_Selected;
		protected bool m_PointerHovering;


		public virtual void Select()
		{
			m_Selected = true;

			RefreshState(m_State);
		}

		public virtual void Deselect()
		{
			m_Selected = false;

			RefreshState(m_PointerHovering ? State.Highlighted : State.Normal);
		}

		public virtual void OnPointerEnter(PointerEventData data)
		{
			m_PointerHovering = true;

			if (!m_Pressed)
				RefreshState(State.Highlighted);
		}

		public virtual void OnPointerDown(PointerEventData data)
		{
			if (data.button == PointerEventData.InputButton.Left)
			{
				m_Pressed = true;
				RefreshState(State.Pressed);
			}


			onPointerDown?.Invoke(this);
		}

		public virtual void OnPointerUp(PointerEventData data)
		{
			m_Pressed = false;

			SlotUI slotUnderPointer = data.pointerCurrentRaycast.gameObject == null ? null : data.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();

			if (slotUnderPointer != null)
			{
				if (slotUnderPointer != this)
					RefreshState(State.Normal);
				else
					RefreshState(State.Highlighted);
			}
			else
				RefreshState(State.Normal);

			onPointerUp?.Invoke(this);
		}

		public virtual void OnPointerExit(PointerEventData data)
		{
			m_PointerHovering = false;

			if (!m_Pressed)
				RefreshState(State.Normal);
			else
				RefreshState(State.Pressed);
		}

		protected virtual void OnValidate()
		{
			if (_Graphic == null)
				_Graphic = GetComponent<Graphic>();

			if (m_Transition == null)
				m_Transition = new Transition();
		}

		private void RefreshState(State state)
		{
			Color color = m_Transition.NormalColor;

			if (state == State.Highlighted)
				color = m_Transition.HighlightedColor;
			else if (state == State.Pressed)
				color = m_Transition.PressedColor;

			if (m_Selected)
				color = m_Transition.HighlightedColor;

			_Graphic.CrossFadeColor(color, m_Transition.FadeDuration, true, true);
		}
	}
}