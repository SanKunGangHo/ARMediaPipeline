using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StepGraph : MonoBehaviour
{
    public TMP_Text graphName;
    
    public Image L_GraphImage;
    public Image R_GraphImage;
    
    public TMP_Text L_AngleText;
    public TMP_Text R_AngleText;
    
    public TMP_Text Best_L_AngleText;
    public TMP_Text Best_R_AngleText;

    public void GraphOut(string _graphName, float angle_L = 0f, float angle_R = 0f, float best_L = 0f, float best_R = 0f)
    {
        Debug.Log($"{_graphName} + {angle_L} + {angle_R} + {best_L} + {best_R}");
        graphName.text = _graphName;
        
        L_GraphImage.fillAmount = angle_L / 180;
        R_GraphImage.fillAmount = angle_R / 180;

        if (float.IsNaN(angle_L) || angle_L <= 0)
        {
            L_AngleText.text = "000";
        }

        if (float.IsNaN(angle_R) || angle_R <= 0)
        {
            R_AngleText.text = "000";
        }
        
        if (float.IsNaN(best_L) || angle_L <= 0)
        {
            Best_L_AngleText.text = "000";
        }

        if (float.IsNaN(angle_R) || angle_R <= 0)
        {
            Best_R_AngleText.text = "000";
        }

        if (angle_L >= 0)
        {
            L_AngleText.text = angle_L.ToString("000");
        }

        if (angle_R >= 0)
        {
            R_AngleText.text = angle_R.ToString("000");
        }
        
        if (best_L >= 0)
        {
            Best_L_AngleText.text = best_L.ToString("000");
        }

        if (best_R >= 0)
        {
            Best_R_AngleText.text = best_R.ToString("000");
        }
    }
}
