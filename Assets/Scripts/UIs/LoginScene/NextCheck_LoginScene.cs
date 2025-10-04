using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIs
{
    public class NextCheck_LoginScene : MonoBehaviour
    {
        //public Sprite NormalCheckmark, GreenCheckmark;

        private void OnEnable()
        {
            dateOfBirth.text = "";
            recentJob_Label.text = "";
            Gender.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            inputFieldPW.text = "";
            inputFieldConfirm.text = "";

            isDoB = false;
            isRJ = false;
            isPW = false;
            isConfirm = false;
        }

        [Header("1/2")]
        #region 1/2

        public TMP_InputField dateOfBirth;
        public TMP_Text recentJob_Label;
        public ToggleGroup Gender;
        
        public Toggle[] checkmarks;

        public Button BTN_Next;

        [Header("조건 체크")]
        [SerializeField] private bool isDoB, isRJ;

        public void DateOfBirthChanged()
        {
            DateTime checkDT = DateTime.Parse(dateOfBirth.text);

            //TODO : 나중에 로직 강화 (나이제한 등)
            if (string.IsNullOrEmpty(dateOfBirth.text))
            {
                checkmarks[0].isOn = false;
                isDoB = false;
                return; 
            }//빈칸체크
       
            if(checkDT > DateTime.Now) { //checkDT가 나와야하기 때문에 제외
                checkmarks[0].isOn = false;
                isDoB = false;
                return; 
            }//이전시간 체크

            checkmarks[0].isOn = true;
            NetworkManager.Instance._playerData.dateOfBirth = checkDT;
            isDoB = true;
            
            NextChecking();
        }

        public void RecentJob()
        {
            if (string.IsNullOrEmpty(recentJob_Label.text) && recentJob_Label.text == "Enter")
            {
                isRJ = false;
                //Checkmarks[1].sprite = NormalCheckmark;
                checkmarks[1].isOn = false;
            }
            else
            {
                isRJ = true;
                //Checkmarks[1].sprite = GreenCheckmark;
                checkmarks[1].isOn = true;
            }
            NextChecking();
        }

        public void Gender_check()
        {
            //isGen = true;
            NextChecking();
        }

        public void NextChecking()
        {
            if (isDoB && isRJ)
            {
                BTN_Next.interactable = true;
            }
        }

        #endregion
        
        
        [Space(30)]
        [Header("2/2")]
        #region 2/2

        public TMP_InputField inputFieldPW;
        public TMP_InputField inputFieldConfirm;

        public Toggle[] Checkmarks_2;

        public Button BTN_Next_2;

        public Toggle[] playerCheckToggle;

        public Toggle eye_pass, eye_confirm;
        
        //이하 Regex는 비밀번호 제한이 필요할 때 사용하시오.
        Regex engLittleRegex = new Regex(@"[a-z]"); //영어 소문자
        Regex engLargeRegex = new Regex(@"[A-Z]"); //영어 대문자
        Regex PW_specialRegex = new Regex(@"[\.\,\*\!\@\'\\\""\^]"); //비밀번호용 특수문자 제한
        Regex PW_blacklistSpecialRegex = new Regex(@"[\~\#\$\%_\-\&\(\)\{\}\[\]\=\+\|\/\:\;\?\<\>\;\:\|\\]"); //비밀번호용 특수문자 제한

        [Header("조건 체크")] [SerializeField] private bool isPW, isConfirm, isCheck1, isCheck2;

        public void PWChecking()
        {
            Boolean isMatchLittleEng = engLittleRegex.IsMatch(inputFieldPW.text);
            Boolean isMatchBigEng = engLargeRegex.IsMatch(inputFieldPW.text);
            
            if (string.IsNullOrEmpty(inputFieldPW.text) || inputFieldPW.text.Length < 4 /*&& (isMatchLittleEng && isMatchBigEng)*/)
            {
                Checkmarks_2[0].isOn = false;
                isPW = false;
                return;
            }
            
            Checkmarks_2[0].isOn = true;
            isPW = true;
            NextChecking_2();
        }

        public void PWConfirming()
        {
            if (string.IsNullOrEmpty(inputFieldConfirm.text) || inputFieldPW.text != inputFieldConfirm.text )
            {
                Checkmarks_2[1].isOn = false;
                isConfirm = false;
                return;
            }
            else
            {
                Checkmarks_2[1].isOn = true;
                isConfirm = true;
            }
            NextChecking_2();
        }

        public void Terms_privacy()
        {
            isCheck1 = playerCheckToggle[0].isOn;
            NextChecking_2();
        }

        public void ResearchPledge()
        {
            isCheck2 = playerCheckToggle[1].isOn;
            NextChecking_2();
        }

        public void NextChecking_2()
        {
            if (isPW && isConfirm && isCheck1 && isCheck2)
            {
                BTN_Next_2.interactable = true;
            }
            else
            {
                BTN_Next_2.interactable = false;
            }
        }

        public void ToggleEye_pass()
        {
            inputFieldPW.contentType = !eye_pass.isOn ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            inputFieldPW.ForceLabelUpdate();
        }
        
        public void ToggleEye_Confirm()
        {
            inputFieldConfirm.contentType = !eye_confirm.isOn ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            inputFieldConfirm.ForceLabelUpdate();
        }
        
        #endregion

        public void SaveDatas()
        {
            IEnumerable<Toggle> activeToggles = Gender.ActiveToggles();
            NetworkManager.Instance._playerData.dateOfBirth = DateTime.Parse(dateOfBirth.text);
            NetworkManager.Instance._playerData.job = recentJob_Label.text;
            NetworkManager.Instance._playerData.gender = activeToggles.First().name;
        }

        public void SavePW()
        {
            NetworkManager.Instance._playerData.PW = inputFieldPW.text;
            NetworkManager.Instance._firebaseLoader.RegisterInput(NetworkManager._instance._playerData.PatientID, inputFieldPW.text, 
                dateOfBirth.text, recentJob_Label.text, NetworkManager._instance._playerData.gender);
            
            List<string> exercise = new List<string>();
            foreach (var data in NetworkManager._instance.exerciseDatas_all)
            {
                exercise.Add(data.Title);
            }
            NetworkManager.Instance._firebaseLoader.TodaysActivityCollector(exercise);
            //회원가입시 전체 운동 리스트 만들기
        }
    }
}


