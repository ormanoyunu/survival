using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface ICharacter : IMonoBehaviour
    {
        bool IsInitialized { get; }

        Transform View { get; }
        Collider[] Colliders { get; }

        IAudioPlayer AudioPlayer { get; }
        ICharacterMover Mover { get; }
        IHealthManager HealthManager { get; }
        IInventory Inventory { get; }

        event UnityAction onInitialized;


        bool TryGetModule<T>(out T module) where T : ICharacterModule;
        void GetModule<T>(out T module) where T : ICharacterModule;
        T GetModule<T>() where T : ICharacterModule;
        bool HasCollider(Collider collider);
    }
}