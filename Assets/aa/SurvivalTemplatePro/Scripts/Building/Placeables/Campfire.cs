using SurvivalTemplatePro.InventorySystem;
using SurvivalTemplatePro.WorldManagement;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class Campfire : Interactable, IExternalContainer
    {
        public bool FireActive
        {
            get => m_FireIsActive;
            private set
            {
                if (m_FireIsActive != value)
                {
                    m_FireIsActive = value;

                    if (m_FireIsActive)
                        onFireStarted?.Invoke();
                    else
                        onFireStopped?.Invoke();
                }
            }
        }

        public float FireDurationRealtime => m_FireDurationRealtime;
        public float TemperatureStrength => m_TemperatureStrength;
        public int MaxProximityTemperature => m_MaxProximityTemperature;

        public ItemContainer ItemContainer { get; private set; }

        public event UnityAction onFireStarted;
        public event UnityAction onFireStopped;
        public event UnityAction<float> onFuelAdded;

        [BHeader("General (Campfire)")]

        [SerializeField, Range(1, 10)]
        [Tooltip("How many cooking spots (item slots) this campfire has.")]
        private int m_CookingSpots = 3;

        [SerializeField, Range(0.01f, 10f)]
        [Tooltip("Multiplies the effects of any fuel added (heat and added time).")]
        private float m_FuelDurationMod = 1f;

        [BHeader("Cooking (Campfire)")]

        [SerializeField]
        [Tooltip("The property that tells the campfire how cooked an item is.")]
        private ItemPropertyReference m_CookedAmountProperty;

        [SerializeField]
        [Tooltip("The property that tells the campfire in what item should the cooked item transform.")]
        private ItemPropertyReference m_CookedOutputProperty;

        [SerializeField, Range(1f, 30f)]
        [Tooltip("The amount of time it takes to cook an item.")]
        private float m_ItemCookDuration = 10f;

        [BHeader("Temperature (Campfire)")]

        [SerializeField]
        private TemperatureEffector m_TemperatureEffector;

        [SerializeField, Range(1f, 1000f)]
        private float m_MaxTemperatureAchieveTime = 500;

        [SerializeField, Range(0f, 15f)]
        private float m_MaxFireRadius = 5f;

        [SerializeField, Range(40, 60)]
        private int m_MaxProximityTemperature = 50;

        private float m_InGameDayScale;

        private float m_FireDurationRealtime;
        private bool m_FireIsActive = false;
        private float m_TemperatureStrength;
        private GameTime m_FireDuration;


        #region Save&Load
        public void OnLoad()
        {

        }
        #endregion

        public void StartFire(float fuelDuration)
        {
            FireActive = true;
            m_FireDurationRealtime = Mathf.Clamp(m_FireDurationRealtime + fuelDuration * m_FuelDurationMod, 0f, m_MaxTemperatureAchieveTime);
        }

        public void StopFire()
        {
            FireActive = false;
            m_FireDurationRealtime = 0f;
            DescriptionText = string.Empty;
        }

        public void AddFuel(float fuelDuration)
        {
            m_FireDurationRealtime = Mathf.Clamp(m_FireDurationRealtime + fuelDuration * m_FuelDurationMod, 0f, m_MaxTemperatureAchieveTime);
            onFuelAdded?.Invoke(fuelDuration);
        }

        private void Start()
        {
            ItemContainer = new ItemContainer("Cooking", 100, m_CookingSpots, ItemContainerFlags.External, null, null, null);

            // How much smaller is the game day vs a realtime day (a value of 1 means the day takes 1440 real minutes, like the real day takes)
            m_InGameDayScale = WorldManagerBase.Instance.GetDayDurationInMinutes() / 1440;
            m_TemperatureEffector.Radius = m_MaxFireRadius;
        }

        private void Update()
        {
            if (m_FireIsActive)
            {
                UpdateFire();
                UpdateCooking();

                if (HoverActive)
                    UpdateDescriptionText();
            }
        }

        private void UpdateFire()
        {
            if (m_FireDurationRealtime <= 0)
            {
                StopFire();
                return;
            }

            m_FireDurationRealtime -= Time.deltaTime;
            m_FireDuration = new GameTime(m_FireDurationRealtime, m_InGameDayScale);

            m_TemperatureStrength = m_FireDurationRealtime / m_MaxTemperatureAchieveTime;
            m_TemperatureEffector.TemperatureStrength = m_TemperatureStrength;
        }

        private void UpdateCooking()
        {
            for (int i = 0; i < ItemContainer.Slots.Length; i++)
            {
                if (ItemContainer.Slots[i].HasItem)
                {
                    Item item = ItemContainer.Slots[i].Item;

                    if (item.TryGetProperty(m_CookedAmountProperty, out ItemProperty cookedAmount))
                    {
                        cookedAmount.Float += (Time.deltaTime / (item.CurrentStackSize * m_ItemCookDuration));

                        if (cookedAmount.Float >= 1f)
                        {
                            if (item.TryGetProperty(m_CookedOutputProperty, out ItemProperty cookedOutputProperty))
                            {
                                ItemInfo cookOutput = ItemDatabase.GetItemById(cookedOutputProperty.ItemId);

                                if (cookOutput != null)
                                    ItemContainer.Slots[i].SetItem(new Item(cookOutput, item.CurrentStackSize));
                            }
                        }
                    }
                }
            }
        }

        private void UpdateDescriptionText()
        {
            GameTime fireDuration = m_FireDuration;

            string infoString = $"Duration: {fireDuration.GetTimeToStringWithSuffixes(true, true, false)} \n";    
            infoString += $"Heat: +{Mathf.RoundToInt(m_TemperatureStrength * m_MaxProximityTemperature)}C";

            DescriptionText = infoString;
        }
    }
}