using UnityEngine;

namespace SurvivalTemplatePro
{
    public class Billboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main != null)
            {
                Quaternion rot = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
                rot = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);

                transform.rotation = rot;
            }
        }
    }
}