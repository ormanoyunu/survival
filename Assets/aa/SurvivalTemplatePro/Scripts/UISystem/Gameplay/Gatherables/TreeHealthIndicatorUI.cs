using SurvivalTemplatePro.InventorySystem;
using SurvivalTemplatePro.ResourceGathering;
using SurvivalTemplatePro.WieldableSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class TreeHealthIndicatorUI : PlayerUIBehaviour
    {
        [BHeader("References")]

        [SerializeField]
        private Renderer m_Renderer;

        [SerializeField]
        private Animator m_Animator;

        [BHeader("Filtering")]

        [SerializeField]
        private ItemReference[] m_ItemsToShowOn;

        [SerializeField]
        private GatherableDefinition m_TreeDefinition;

        [BHeader("Settings")]

        [SerializeField, Range(0f, 30f)]
        private float m_ShowDistance = 5f;

        [SerializeField, Range(0f, 120f)]
        private float m_ShowAngle = 60f;

        [SerializeField]
        private float m_AlphaLerpSpeed = 3f;

        [Space]

        [SerializeField]
        private bool m_RotateWithPlayer;

        [SerializeField]
        private Gradient m_HealthColorGradient;

        private Gatherable m_ClosestTree;
        private Gatherable m_LastClosestTree;
        private List<Gatherable> m_AllTreesInScene;

        private bool m_IsCurentlyShown = false;
        private bool m_HasCorrectItemEquipped = false;
        private float m_LastTreeHealth;
        private float m_Opacity;

        private int m_HashedTreeDamagedTrigger;
        private int m_HashedShowIndicator;
        private int m_HashedHideIndicator;

        private int m_HealthAmountShaderId;
        private int m_HealthColorShaderId;
        private int m_AlphaShaderId;

        private IWieldablesController m_WieldableController;
        private ILookHandler m_LookHandler;


        public override void OnAttachment()
        {
            m_HealthAmountShaderId = Shader.PropertyToID("_HealthAmount");
            m_HealthColorShaderId = Shader.PropertyToID("_HealthColor");
            m_AlphaShaderId = Shader.PropertyToID("_Alpha");

            m_HashedShowIndicator = Animator.StringToHash("Show");
            m_HashedHideIndicator = Animator.StringToHash("Hide");
            m_HashedTreeDamagedTrigger = Animator.StringToHash("Damage");

            m_Renderer.material.SetFloat(m_HealthAmountShaderId, 1f);
            m_Renderer.material.SetColor(m_HealthColorShaderId, m_HealthColorGradient.Evaluate(1));

            m_WieldableController = GetModule<IWieldablesController>();
            m_LookHandler = GetModule<ILookHandler>();

            Gatherable.TryGetAllGatherablesWithDefinition(m_TreeDefinition, out m_AllTreesInScene);

            m_WieldableController.onWieldableEquipped += OnWieldableChanged;
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            bool hasCorrectItem = false;

            // Check if the equipped wieldable is valid
            if (wieldable != null)
            {
                Item equippedItem = wieldable.AttachedItem;

                if (equippedItem != null)
                {
                    int equippedItemId = equippedItem.Id;

                    foreach (int itemId in m_ItemsToShowOn)
                    {
                        if (equippedItemId == itemId)
                        {
                            hasCorrectItem = true;
                            break;
                        }
                    }
                }
            }

            // Enable indicator
            if (!m_HasCorrectItemEquipped && hasCorrectItem)
            {
                m_LookHandler.onPostViewUpdate += OnViewUpdate;
            }
            // Disable indicator
            else if (!hasCorrectItem && m_HasCorrectItemEquipped)
            {
                m_LookHandler.onPostViewUpdate -= OnViewUpdate;

                m_IsCurentlyShown = false;
                m_ClosestTree = null;
                m_Opacity = 0f;

                UpdateIndicatorOpacity();
            }

            m_HasCorrectItemEquipped = hasCorrectItem;
        }

        public override void OnDetachment()
        {
            m_WieldableController.onWieldableEquipped -= OnWieldableChanged;
            m_LookHandler.onPostViewUpdate -= OnViewUpdate;
        }

        private void OnViewUpdate()
        {
            UpdateIndicatorOpacity();

            m_ClosestTree = null;

            if (m_AllTreesInScene.Count > 0)
            {
                for (int i = 0;i < m_AllTreesInScene.Count; i++)
                {
                    if (m_AllTreesInScene[i] == null)
                        continue;

                    Gatherable tree = m_AllTreesInScene[i];

                    Vector3 playerPosition = Player.transform.position;
                    Vector3 previewPosition = tree.transform.position + tree.transform.TransformVector(tree.GatherOffset);
                    Vector3 dirFromPlayerToTree = previewPosition - playerPosition;

                    float distanceToPreviewSqr = (playerPosition - previewPosition).sqrMagnitude;

                    if (distanceToPreviewSqr < m_ShowDistance * m_ShowDistance && Vector3.Angle(dirFromPlayerToTree, Player.transform.forward) < m_ShowAngle)
                    {
                        m_ClosestTree = tree;
                        transform.position = previewPosition;
                        m_IsCurentlyShown = true;

                        break;
                    }
                    else
                    {
                        m_IsCurentlyShown = false;
                        m_ClosestTree = null;
                    }
                }
            }
            else
            {
                m_IsCurentlyShown = false;
                m_ClosestTree = null;
            }

            if (m_ClosestTree != null)
            {
                if (m_LastTreeHealth != m_ClosestTree.Health)
                    SetHealthAmount(m_ClosestTree);

                if (m_RotateWithPlayer && m_ClosestTree.Health > 0.1f && m_IsCurentlyShown)
                    RotateWithPlayerCamera();
            }

            if (m_ClosestTree != m_LastClosestTree)
                m_Animator.SetTrigger(m_ClosestTree != null ? m_HashedShowIndicator : m_HashedHideIndicator);

            m_LastClosestTree = m_ClosestTree;
        }

        private void RotateWithPlayerCamera()
        {
            Quaternion rot = Quaternion.LookRotation(transform.position - m_LookHandler.transform.position, Vector3.up);
            rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, 0);

            transform.rotation = rot;
        }

        private void UpdateIndicatorOpacity()
        {
            float opacityTarget = m_IsCurentlyShown && (m_ClosestTree == null || m_ClosestTree.Health > 0.1f) ? 1f : 0f;

            m_Opacity = Mathf.Lerp(m_Opacity, opacityTarget, Time.deltaTime * m_AlphaLerpSpeed);
            m_Renderer.material.SetFloat(m_AlphaShaderId, m_Opacity);
        }

        private void SetHealthAmount(Gatherable tree)
        {
            float health = tree.Health / tree.MaxHealth;

            if (health <= 0)
            {
                m_IsCurentlyShown = false;
                return;
            }

            Color healthColor = m_HealthColorGradient.Evaluate(health);

            m_Renderer.material.SetFloat(m_HealthAmountShaderId, health);
            m_Renderer.material.SetColor(m_HealthColorShaderId, healthColor);

            m_Animator.SetTrigger(m_HashedTreeDamagedTrigger);

            m_LastTreeHealth = tree.Health;
        }
     }
}
