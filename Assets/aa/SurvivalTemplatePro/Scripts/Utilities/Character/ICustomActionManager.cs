using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface ICustomActionManager : ICharacterModule
    {
        bool CustomActionActive { get; }

        event UnityAction<CustomActionParams> onActionStart;
        event UnityAction onActionEnd;

        void StartAction(CustomActionParams actionParams);
        void TryCancelAction();
    }

    public struct CustomActionParams
    {
        public float StartTime { get; }
        public float EndTime { get; }
        public string Name { get; }
        public string Description { get; }
        public bool CanCancel { get; }
        public UnityAction OnCompleteCallbacks { get; }
        public UnityAction OnCanceledCallbacks { get; }


        public CustomActionParams(string name, string description, float duration, bool canCancel, UnityAction onCompleteCallbacks, UnityAction onCanceledCallbacks)
        {
            this.StartTime = Time.time;
            this.EndTime = Time.time + duration;
            this.Name = name;
            this.Description = description;
            this.CanCancel = canCancel;
            this.OnCompleteCallbacks = onCompleteCallbacks;
            this.OnCanceledCallbacks = onCanceledCallbacks;
        }

        public float GetProgress() => 1f - ((EndTime - Time.time) / (EndTime - StartTime));
    }
}