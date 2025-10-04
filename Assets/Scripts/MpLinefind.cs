using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;

public class MpLinefind : MonoBehaviour
{
    public Color startColorB = new Color(0, 117, 255);
    public Color endColorB = new Color(0, 117, 255);
    public Color startColorW = Color.white;
    public Color endColorW = Color.white;
    
    public int armAngle;
    private LineRenderer Larm1;
    private LineRenderer Larm2;
    private LineRenderer Rarm1;
    private LineRenderer Rarm2;
    float Langle;
    float Rangle;

    public MpMarkfind mf;
    Gradient gradientB;
    Gradient gradientW;
    private Gradient _gradientAlpha;
    LineRenderer[] lines;
    private void Start()
    {
        gradientB = new Gradient();
        gradientW = new Gradient();
        _gradientAlpha = new Gradient();
        
        gradientB.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColorB, 0.0f), new GradientColorKey(endColorB, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        gradientW.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColorW, 0.0f), new GradientColorKey(endColorW, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        
        _gradientAlpha.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColorW, 0.0f), new GradientColorKey(endColorW, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );

    }

    // Update is called once per frame
    void Update()
    {
        //bicep_markfind mf = GameObject.Find("Point List Annotation").GetComponent<bicep_markfind>();
        //GameObject CB = GameObject.Find("Bicep_ChatBubble").gameObject;
        if (transform.GetChild(0).gameObject.activeInHierarchy)
        {
            lines = this.transform.GetComponentsInChildren<LineRenderer>(true);
        }
        if (lines == null) return;
        //Langle = mf.Langle;
        // Rangle = mf.Rangle;
        if (Larm1 == null)
        {
            Larm1 = this.transform.GetChild(0).GetComponent<LineRenderer>();
            Larm2 = this.transform.GetChild(1).GetComponent<LineRenderer>();
            Rarm1 = this.transform.GetChild(6).GetComponent<LineRenderer>();
            Rarm2 = this.transform.GetChild(7).GetComponent<LineRenderer>();
        }
        
        //Larm1.colorGradient = gradientW;
        // Larm2.colorGradient = gradientW;
        // Rarm1.colorGradient = gradientR;
        // Rarm2.colorGradient = gradientR;


         /*if (Langle > armAngle)
         {
             Larm1.colorGradient = gradientW;
             Larm2.colorGradient = gradientW;
         }

         else if (Langle <= armAngle)
         {
             //Larm1.colorGradient = gradientR;
             //Larm2.colorGradient = gradientR;
         }

         if (Rangle > armAngle)
         {
             Rarm1.colorGradient = gradientW;
             Rarm2.colorGradient = gradientW;
         }
         else if (Rangle <= armAngle)
         {
             //Rarm1.colorGradient = gradientR;
             //Rarm2.colorGradient = gradientR;
         }*/
    }

    private float frontData;
    private float sideData;
    private float data;
    public void LineColor(int color)
    {
        if (lines == null) return;
        if (color == 0)
        {
            Larm1.colorGradient = gradientB;
            Larm2.colorGradient = gradientB;
            Rarm1.colorGradient = gradientB;
            Rarm2.colorGradient = gradientB;
        }
        if (color == 1)
        {
            Larm2.colorGradient = gradientW;
            Larm1.colorGradient = gradientW;
            Rarm1.colorGradient = gradientW;
            Rarm2.colorGradient = gradientW;
        }
    }
    
    public void LineColor_OperationOnly(int index, int color)
    {
        if (lines == null) return;
        switch (index)
        {
            case 0:
                LineColor_Kai(true, false, color);
                break;
            case 1:
                LineColor_Kai(false, true, color);
                break;
            case 2:
                LineColor_Kai(true, false, color);
                break;
            case 3:
                LineColor_Kai(false, true, color);
                break;
        }
    }

    public void LineColor_Kai(bool L, bool R, int color)
    {
        // switch (color)
        // {
        //     case 0:
        //         Larm1.colorGradient = gradientB;
        //         Larm2.colorGradient = gradientB;
        //         Rarm1.colorGradient = gradientB;
        //         Rarm2.colorGradient = gradientB;
        //         break;
        //     case 1:
        //         Larm1.colorGradient = gradientW;
        //         Larm2.colorGradient = gradientW;
        //         Rarm1.colorGradient = gradientW;
        //         Rarm2.colorGradient = gradientW;
        //         break;
        // }
        
        if (L && !R)
        {
            // Larm1.colorGradient = gradientB;
            // Larm2.colorGradient = gradientB;
            // Rarm1.colorGradient = _gradientAlpha;
            // Rarm2.colorGradient = _gradientAlpha;
            switch (color)
            {
                case 0:
                    Larm1.colorGradient = gradientB;
                    Larm2.colorGradient = gradientB;
                    Rarm1.colorGradient = _gradientAlpha;
                    Rarm2.colorGradient = _gradientAlpha;
                    break;
                case 1:
                    Larm1.colorGradient = gradientW;
                    Larm2.colorGradient = gradientW;
                    Rarm1.colorGradient = _gradientAlpha;
                    Rarm2.colorGradient = _gradientAlpha;
                    break;
            }
        }
        else if (R && !L)
        {
            // Rarm1.colorGradient = gradientB;
            // Rarm2.colorGradient = gradientB;
            // Larm1.colorGradient = _gradientAlpha;
            // Larm2.colorGradient = _gradientAlpha;
            switch (color)
            {
                case 0:
                    Rarm1.colorGradient = gradientB;
                    Rarm2.colorGradient = gradientB;
                    Larm1.colorGradient = _gradientAlpha;
                    Larm2.colorGradient = _gradientAlpha;
                    break;
                case 1:
                    Rarm1.colorGradient = gradientW;
                    Rarm2.colorGradient = gradientW;
                    Larm1.colorGradient = _gradientAlpha;
                    Larm2.colorGradient = _gradientAlpha;
                    break;
            }
        }
    }
/*    public void InitData()
    {
        frontData = 0;
        sideData = 0;
    }
    public void SumData(int captureMode)
    {
        data = 0;
        if (Langle > armAngle)
            data += 1;
        if (Rangle > armAngle)
            data += 1;

        if (captureMode == 0)
            frontData += data;
        else if (captureMode == 1)
            sideData += data;
    }*/

/*    public int GetData(int captureMode)
    {
        if (captureMode == 0)
            return (int)(frontData / 30 / 2 * 100);
        else if (captureMode == 1)
            return (int)(sideData / 30 / 2 * 100);
        else // total
            return (int)((frontData + sideData) / 30 / 4 * 100);
    }*/

    /*   public int IsArmAngleOK()
       {
           int result = 0;
           if (Langle > armAngle)
               result++; // good
           if (Rangle > armAngle)
               result++; // perfect
           if (!playCollider.check[7] || !playCollider.check[8])
               result = 0;
           return result;
       }*/
}
