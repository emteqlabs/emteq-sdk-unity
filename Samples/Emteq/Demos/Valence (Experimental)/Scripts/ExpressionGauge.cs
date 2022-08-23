using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmteqLabs
{
    public class ExpressionGauge : MonoBehaviour
    {
        [SerializeField] private Transform pointer;
        [SerializeField] private Transform endPosMarker;

        private Vector3 startPos;
        private Vector3 endPos;

        public TMP_Text pointerValuetext;

        void Start()
        {
            startPos = pointer.localPosition;
            endPos = endPosMarker.localPosition;
        }

        public void SetValue(float current)
        {
            pointerValuetext.text = current.ToString();
            Vector3 newPos = new Vector3
                {x = (endPos.x - startPos.x) * current + startPos.x, y = startPos.y, z = startPos.z};
            pointer.localPosition = newPos;
        }
    }
}