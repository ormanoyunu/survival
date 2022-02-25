namespace SurvivalTemplatePro
{
    public class Condition
    {
        public int ConditionsCount => Conditions.GetInvocationList().Length;

        public delegate bool BoolDelegate();

        private event BoolDelegate Conditions;


        public void AddCondition(BoolDelegate condition) => Conditions += condition;
        public void RemoveCondition(BoolDelegate condition) => Conditions -= condition;

        public bool EvaluateConditions()
        {
            if (Conditions != null)
            {
                var invocationList = Conditions.GetInvocationList();

                for (int i = 0; i < invocationList.Length; i++)
                {
                    if (!(bool)invocationList[i].DynamicInvoke())
                        return false;
                }
            }

            return true;
        }
    }

    public class Condition<T>
    {
        public int ConditionsCount => Conditions.GetInvocationList().Length;

        public delegate bool BoolDelegateWithParam(T condition);

        private event BoolDelegateWithParam Conditions;


        public void AddCondition(BoolDelegateWithParam condition) => Conditions += condition;
        public void RemoveCondition(BoolDelegateWithParam condition) => Conditions -= condition;

        public bool EvaluateConditions()
        {
            if (Conditions != null)
            {
                var invocationList = Conditions.GetInvocationList();

                for (int i = 0; i < invocationList.Length; i++)
                {
                    if (!(bool)invocationList[i].DynamicInvoke())
                        return false;
                }
            }

            return true;
        }
    }
}