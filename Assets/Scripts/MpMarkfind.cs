using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class MpMarkfind : MonoBehaviour
{
    public Transform Lhand;
    public Transform Rhand;
    public Transform Rshoulder;
    public Transform Relbow;
    public Transform Rwrist;
    public float Rradians;
    public float Rangle;
    public Transform Lshoulder;
    public Transform Lelbow;
    public Transform Lwrist;
    public float Lradians;
    public float Langle;
    public float hipAngle;
    public Text angleText;
    public Vector3 Rhip, Lhip;
    public float RsAngle,LsAngle;

    public Color startColorB = new Color(0, 117, 255);
    public Color endColorB = Color.blue;
    public Color startColorW = Color.white;
    public Color endColorW = Color.white;
    
    Transform[] allchildren;
    public MeshRenderer[] pointMats, pointMats_L, pointMats_R;

    private bool matCheck = false;

    private void Start()
    {
        pointMats = new MeshRenderer[6];
        pointMats_L = new MeshRenderer[3];
        pointMats_R = new MeshRenderer[3];
    }

    void Update()
    {
        if (transform.GetChild(0).gameObject.activeInHierarchy)
        { 
            allchildren = this.transform.GetComponentsInChildren<Transform>(true); 
        }
        if (allchildren == null) return;
        for (int i = 0; i < allchildren.Length; i++)
        {
            Rshoulder = allchildren[13];
            Relbow = allchildren[15];
            Rwrist = allchildren[17];
            Lshoulder = allchildren[12];
            Lelbow = allchildren[14];
            Lwrist = allchildren[16];
            Lhand = allchildren[20];
            Rhand = allchildren[21];
            Lhip = allchildren[24].localPosition;
            Rhip  = allchildren[25].localPosition;
            
            pointMats[0] = Rshoulder.GetComponent<MeshRenderer>();
            pointMats[1] = Relbow.GetComponent<MeshRenderer>();
            pointMats[2] = Rwrist.GetComponent<MeshRenderer>();
            pointMats[3] = Lshoulder.GetComponent<MeshRenderer>();
            pointMats[4] = Lelbow.GetComponent<MeshRenderer>();
            pointMats[5] = Lwrist.GetComponent<MeshRenderer>();

            pointMats_L[0] = Lshoulder.GetComponent<MeshRenderer>();
            pointMats_L[1] = Lelbow.GetComponent<MeshRenderer>();
            pointMats_L[2] = Lwrist.GetComponent<MeshRenderer>();

            pointMats_R[0] = Rshoulder.GetComponent<MeshRenderer>();
            pointMats_R[1] = Relbow.GetComponent<MeshRenderer>();
            pointMats_R[2] = Rwrist.GetComponent<MeshRenderer>();
            
            // var Lknee = allchildren[26].position;
            // var hipRadians = Mathf.Atan2(Lshoulder.position.y - Lhip.y, Lshoulder.position.x - Lhip.x) - Mathf.Atan2(Lknee.y - Lhip.y, Lknee.x - Lhip.x);
            // hipAngle = Mathf.Abs(hipRadians * 180.0f / Mathf.PI);
            // if (hipAngle > 180.0)
            // {
            //     hipAngle = 360 - hipAngle;
            // }
            // angleText.text = hipAngle.ToString();
            // Debug.Log(hipAngle);

            Rradians = Mathf.Atan2(Rwrist.localPosition.y - Relbow.localPosition.y, Rwrist.localPosition.x - Relbow.localPosition.x) - Mathf.Atan2(Rshoulder.localPosition.y - Relbow.localPosition.y, Rshoulder.localPosition.x - Relbow.localPosition.x);
            Rangle = Mathf.Abs(Rradians * 180.0f / Mathf.PI);
            if (Rangle > 180.0)
            {
                Rangle = 360 - Rangle;
            }
            Lradians = Mathf.Atan2(Lwrist.localPosition.y - Lelbow.localPosition.y, Lwrist.localPosition.x - Lelbow.localPosition.x) - Mathf.Atan2(Lshoulder.localPosition.y - Lelbow.localPosition.y, Lshoulder.localPosition.x - Lelbow.localPosition.x);
            Langle = Mathf.Abs(Lradians * 180.0f / Mathf.PI);
            if (Langle > 180.0)
            {
                Langle = 360 - Langle;
            }

            var RsRadians = Mathf.Atan2(Relbow.localPosition.y - Rshoulder.localPosition.y, Relbow.localPosition.x - Rshoulder.localPosition.x) - Mathf.Atan2(Rhip.y - Rshoulder.localPosition.y, Rhip.x - Rshoulder.localPosition.x);
            RsAngle = Mathf.Abs(RsRadians * 180.0f / Mathf.PI);
            if (RsAngle > 180.0)
            {
                RsAngle = 360 - RsAngle;
            }
            var LsRadians = Mathf.Atan2(Lelbow.localPosition.y - Lshoulder.localPosition.y, Lelbow.localPosition.x - Lshoulder.localPosition.x) - Mathf.Atan2(Lhip.y - Lshoulder.localPosition.y, Lhip.x - Lshoulder.localPosition.x);
            LsAngle = Mathf.Abs(LsRadians * 180.0f / Mathf.PI);
            if (LsAngle > 180.0)
            {
                LsAngle = 360 - LsAngle;
            }
        }
        
        
    }
    
    public void PointColor(int color)
    {
        if (allchildren == null)return;
        switch (color)
        {
            case 0:
            {
                foreach (var mat in pointMats)
                {
                    mat.material.color = startColorB;
                }
                break;
            }
            case 1:
            {
                foreach (var mat in pointMats)
                {
                    mat.material.color = startColorW;
                }
                break;
            }
        }
        
        if (!matCheck)
        {
            matCheck = true;
            foreach (var mat in pointMats)
            {
                mat.enabled = true;
            }
        }
    }

    public void PointColor_OperationOnly(int index, int color)
    {
        if (allchildren == null) return;
        switch (index)
        {
            case 0:
                PointColor_kai(true, false, color);
                break;
            case 1:
                PointColor_kai(false, true, color);
                break;
            case 2:
                PointColor_kai(true, false, color);
                break;
            case 3:
                PointColor_kai(false, true, color);
                break;
        }
    }

    public void PointColor_kai(bool L, bool R, int color)
    {
        if (allchildren == null)return;
        switch (color)
        {
            case 0:
            {
                foreach (var mat in pointMats)
                {
                    mat.material.color = startColorB;
                }
                break;
            }
            case 1:
            {
                foreach (var mat in pointMats)
                {
                    mat.material.color = startColorW;
                }
                break;
            }
        }
        
        if (L && !R)
        {
            foreach (var mat in pointMats_L)
            {
                mat.enabled = true;
            }
            foreach (var mat in pointMats_R)
            {
                mat.enabled = false;
            }
        }
        else if (R && !L)
        {
            foreach (var mat in pointMats_R)
            {
                mat.enabled = true;
            }
            foreach (var mat in pointMats_L)
            {
                mat.enabled = false;
            }
        }
    }
}