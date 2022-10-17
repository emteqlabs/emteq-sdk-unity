using System.Collections;
using UnityEngine;

namespace EmteqLabs
{
    public class ExpressionAvatar : MonoBehaviour
    {
        public ExpressionType SelectedExpression = ExpressionType.Neutral;
        public GameObject[] ExpressionIcons;

        public bool expressionSet = true;
        
        void Start()
        {
            ChangeExpression();
        }

        public void ChangeExpression()
        {
            switch (SelectedExpression)
            {
                case ExpressionType.Neutral:
                    SetExpression("Neutral");
                    break;
                case ExpressionType.Smile:
                    SetExpression("SmileBoth");
                    break;
                case ExpressionType.SmileLeftSide:
                    SetExpression("SmileLeft");
                    break;
                case ExpressionType.SmileRightSide:
                    SetExpression("SmileRight");
                    break;
                case ExpressionType.Frown:
                    SetExpression("FrownBoth");
                    break;
                case ExpressionType.FrownLeftSide:
                    SetExpression("FrownLeft");
                    break;
                case ExpressionType.FrownRightSide:
                    SetExpression("FrownRight");
                    break;
                case ExpressionType.Surprise:
                    SetExpression("SurpriseBoth");
                    break;
                case ExpressionType.SurpriseLeftSide:
                    SetExpression("SurpriseLeft");
                    break;
                case ExpressionType.SurpriseRightSide:
                    SetExpression("SurpriseRight");
                    break;
                case ExpressionType.CloseEyes:
                    SetExpression("CloseEyesBoth");
                    break;
                case ExpressionType.CloseLeftEye:
                    SetExpression("CloseEyesLeft");
                    break;
                case ExpressionType.CloseRightEye:
                    SetExpression("CloseEyesRight");
                    break;
                case ExpressionType.RaisedEyeBrow:
                    SetExpression("EyebrowRaiseBoth");
                    break;
                case ExpressionType.RaisedEyeBrowLeftSide:
                    SetExpression("EyebrowRaiseLeft");
                    break;
                case ExpressionType.RaisedEyeBrowRightSide:
                    SetExpression("EyebrowRaiseRight");
                    break;
                case ExpressionType.PuckerLips:
                    SetExpression("PuckerLips");
                    break;
            }
        }
        private void SetExpression(string expressionName)
        {
            foreach (GameObject icon in ExpressionIcons)
            {
                if (icon.name.Contains(expressionName))
                {
                    icon.SetActive(true);
                }
                else
                {
                    icon.SetActive(false); 
                }
            }
        }
    }
}