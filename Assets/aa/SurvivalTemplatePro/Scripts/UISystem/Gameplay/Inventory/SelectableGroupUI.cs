using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class SelectableGroupUI : MonoBehaviour
	{
		[SerializeField]
		private bool m_FindChildrenAtStart = true;

        [SerializeField]
        private bool m_SelectFirstChildAtStart = true;

        [Space]

        [SerializeField]
        private RectTransform m_SelectionFrame;

        [SerializeField]
        private bool m_SetFrameColorToSelected;

        [Space]

        [SerializeField]
        private SoundPlayer m_SelectAudio;

		private SelectableUI[] m_Selectables;
		private SelectableUI m_ActiveSelectable;

        private Graphic m_SelectionFrameGraphic;

        private float m_NextTimeCanPlayAudio;


        public void SetSlots(SelectableUI[] selectables)
		{
			if (m_Selectables != null)
			{
                for (int i = 0;i < m_Selectables.Length;i++)
                {
                    m_Selectables[i].onPointerDown -= SelectSlot;
                    m_Selectables[i].onPointerUp -= SelectSlot;
                }

				if (m_ActiveSelectable != null)
				{
					m_ActiveSelectable.Deselect();
					m_ActiveSelectable = null;
				}
			}

			if (selectables != null && selectables.Length > 0)
			{
				m_Selectables = selectables;

                for (int i = 0; i < m_Selectables.Length; i++)
                {
                    m_Selectables[i].onPointerDown += SelectSlot;
                    m_Selectables[i].onPointerUp += SelectSlot;

                    m_Selectables[i].Deselect();
                }
			}
		}

		public void SelectSlot(SelectableUI slot)
		{
            if (m_ActiveSelectable == slot)
                return;

			if (m_ActiveSelectable != null)
				m_ActiveSelectable.Deselect();

            m_ActiveSelectable = slot;
            m_ActiveSelectable.Select();

            if (m_SelectionFrame != null)
            {
                if(!m_SelectionFrame.gameObject.activeSelf)
                    m_SelectionFrame.gameObject.SetActive(true);

                m_SelectionFrame.SetParent(slot.transform);
                m_SelectionFrame.anchoredPosition = Vector2.zero;

                if (m_SetFrameColorToSelected)
                    m_SelectionFrameGraphic.color = m_ActiveSelectable.GetComponent<Graphic>().color;
            }

            // Play selection audio
            if (m_NextTimeCanPlayAudio < Time.time)
            {
                m_SelectAudio.Play2D();
                m_NextTimeCanPlayAudio = Time.time + 0.4f;
            }
        }

        private void Start()
        {
            m_SelectionFrameGraphic = m_SelectionFrame.GetComponentInChildren<Graphic>();

            if (m_FindChildrenAtStart)
            {
                SetSlots(GetComponentsInChildren<SelectableUI>());

                if (m_SelectFirstChildAtStart && m_Selectables != null && m_Selectables.Length != 0)
                    SelectSlot(m_Selectables[0]);
            }
        }
    }
}