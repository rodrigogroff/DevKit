using System;

namespace Master.Infra.Entity.Database
{
    public partial class User
    {
        public long id { get; set; }
        public string stEmail { get; set; }
        public string stName { get; set; }
        public Boolean? bActive { get; set; }
        public Boolean? bAdmin { get; set; }
        public Boolean? bTokenized { get; set; }
        public string stSocialID { get; set; }
        public string stPassword { get; set; }
        public DateTime? dtLastLogin { get; set; }
        public DateTime? dtCreation { get; set; }
        public DateTime? dtTokenExpires { get; set; }
        public string stToken { get; set; }
        public int? nuTrig1_Email { get; set; }
        public int? nuTrig2_Email { get; set; }
        public int? nuTrig3_Email { get; set; }
        public int? nuTrig4_Email { get; set; }
        public int? nuTrig5_Email { get; set; }
        public int? nuTrig6_Email { get; set; }
        public int? nuTrig7_Email { get; set; }
        public int? nuTrig8_Email { get; set; }
        public int? nuTrig9_Email { get; set; }
        public int? nuTrig10_Email { get; set; }
        public int? nuTrig1_SocialID { get; set; }
        public int? nuTrig2_SocialID { get; set; }
        public int? nuTrig3_SocialID { get; set; }
        public int? nuTrig4_SocialID { get; set; }

        public void compile()
        {
            compile_email_trigs();
            compile_socialID_trigs();
        }

        public void compile_email_trigs()
        {
            var tot_trigs = (int)stEmail.Length / 3;

            nuTrig1_Email = 0; nuTrig2_Email = 0; nuTrig3_Email = 0; nuTrig4_Email = 0; nuTrig5_Email = 0; nuTrig6_Email = 0; 
            nuTrig7_Email = 0; nuTrig8_Email = 0; nuTrig9_Email = 0; nuTrig10_Email = 0;

            for (int i = 0; i < tot_trigs; i++)
            {
                var st = ((int) stEmail[i * 3 + 0]).ToString() + ((int)stEmail[i * 3 + 1]).ToString() + ((int)stEmail[i * 3 + 2]).ToString();

                var res = Convert.ToInt32(st);
                
                switch (i)
                {
                    case 0: nuTrig1_Email = res; break;
                    case 1: nuTrig2_Email = res; break;
                    case 2: nuTrig3_Email = res; break;
                    case 3: nuTrig4_Email = res; break;
                    case 4: nuTrig5_Email = res; break;
                    case 5: nuTrig6_Email = res; break;
                    case 6: nuTrig7_Email = res; break;
                    case 7: nuTrig8_Email = res; break;
                    case 8: nuTrig9_Email = res; break;
                    case 9: nuTrig10_Email = res; break;
                }                
            }
        }

        public string GetEmailTrigsWhere()
        {
            return  "\"nuTrig1_Email\" = " + nuTrig1_Email + " and "+
                    "\"nuTrig2_Email\" = " + nuTrig2_Email + " and "+
                    "\"nuTrig3_Email\" = " + nuTrig3_Email + " and "+
                    "\"nuTrig4_Email\" = " + nuTrig4_Email + " and "+
                    "\"nuTrig5_Email\" = " + nuTrig5_Email + " and "+
                    "\"nuTrig6_Email\" = " + nuTrig6_Email + " and "+
                    "\"nuTrig7_Email\" = " + nuTrig7_Email + " and "+
                    "\"nuTrig8_Email\" = " + nuTrig8_Email + " and "+
                    "\"nuTrig9_Email\" = " + nuTrig9_Email + " and "+
                    "\"nuTrig10_Email\" = " + nuTrig10_Email;
        }

        public string GetSocialIDTrigsWhere()
        {
            return "\"nuTrig1_SocialID\" = " + nuTrig1_SocialID + " and " +
                    "\"nuTrig2_SocialID\" = " + nuTrig2_SocialID + " and " +
                    "\"nuTrig3_SocialID\" = " + nuTrig3_SocialID + " and " +
                    "\"nuTrig4_SocialID\" = " + nuTrig4_SocialID;                    
        }

        public void compile_socialID_trigs()
        {
            var tot_trigs = (int)stSocialID.Length / 3;

            nuTrig1_SocialID = 0; nuTrig2_SocialID = 0; nuTrig3_SocialID = 0; nuTrig4_SocialID = 0;

            for (int i = 0; i < tot_trigs; i++)
            {
                var st = ((int)stSocialID[i * 3 + 0]).ToString() + ((int)stSocialID[i * 3 + 1]).ToString() + ((int)stSocialID[i * 3 + 2]).ToString();
                var res = Convert.ToInt32(st);                

                switch (i)
                {
                    case 0: nuTrig1_SocialID = res; break;
                    case 1: nuTrig2_SocialID = res; break;
                    case 2: nuTrig3_SocialID = res; break;
                    case 3: nuTrig4_SocialID = res; break;                    
                }
            }
        }
    }
}
