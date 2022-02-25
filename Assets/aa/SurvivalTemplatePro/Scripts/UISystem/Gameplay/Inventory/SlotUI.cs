using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    [ExecuteInEditMode]
	public abstract class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
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

		public event UnityAction<SlotUI, PointerEventData> onPointerDown;
		public event UnityAction<SlotUI, PointerEventData> onPointerUp;

		public event UnityAction<State> onStateChanged;

		[SerializeField]
		protected Graphic _Graphic;

		[SerializeField]
		private Transition m_Transition;

        [SerializeField]
        private SoundPlayer m_PointerDownAudio;

		[SerializeField]
		private SoundPlayer m_PointerEnterAudio;

		protected State m_State = State.Normal;

		private CanvasRenderer m_Renderer;
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

			m_PointerEnterAudio.Play2D();

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

            m_PointerDownAudio.Play2D();

            onPointerDown?.Invoke(this, data);
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

            onPointerUp?.Invoke(this, data);
        }

        public virtual void OnPointerExit(PointerEventData data)
        {
            m_PointerHovering = false;

            if (!m_Pressed)
                RefreshState(State.Normal);
            else
                RefreshState(State.Pressed);
        }

        protected virtual void Awake()
		{
			if (!Application.isPlaying)
				return;

			m_Renderer = GetComponent<CanvasRenderer>();
		}

		protected virtual void OnEnable()
		{
			if (_Graphic == null)
				_Graphic = GetComponent<Graphic>();

			if (m_Transition == null)
				m_Transition = new Transition();

			OnValidate();
		}

		protected virtual void OnDisable()
		{	
			m_Renderer = m_Renderer ?? GetComponent<CanvasRenderer>();

			if (m_Renderer != null)
				m_Renderer.SetColor(Color.white);
		}

		protected virtual void OnDestroy()
		{
			m_Renderer = m_Renderer ?? GetComponent<CanvasRenderer>();

			if (m_Renderer != null)
				m_Renderer.SetColor(Color.white);
		}

		protected virtual void OnValidate()
		{
			m_Renderer = m_Renderer ?? GetComponent<CanvasRenderer>();

			if (m_Renderer != null)
				m_Renderer.SetColor(m_Transition.NormalColor);
		}

		private void RefreshState(State state)
		{
			m_State = state;
            Color color = m_Transition.NormalColor;

			if (state == State.Highlighted)
				color = m_Transition.HighlightedColor;
			else if(state == State.Pressed)
				color = m_Transition.PressedColor;

			if (m_Selected)
				color = m_Transition.HighlightedColor;

			_Graphic.CrossFadeColor(color, m_Transition.FadeDuration, true, true);

			onStateChanged?.Invoke(m_State);
		}
    }
}
