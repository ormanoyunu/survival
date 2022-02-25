namespace SurvivalTemplatePro
{
    public class Player : Character
    {
        public static Player LocalPlayer 
        {
            get => m_Player;
            private set
            {
                if (m_Player != value)
                {
                    m_Player = value;
                    onLocalPlayerChanged?.Invoke(m_Player);     
                }
            }
        }

        /// <summary>
        ///  Player: Current Player
        /// </summary>
        public static event OnLocalPlayerChanged onLocalPlayerChanged;

        private static Player m_Player;


        protected override void Awake()
        {
            if (LocalPlayer != null)
                Destroy(this);
            else
            {
                base.Awake();
                LocalPlayer = this;
            }
        }
    }

    public delegate void OnLocalPlayerChanged(Player player);
}