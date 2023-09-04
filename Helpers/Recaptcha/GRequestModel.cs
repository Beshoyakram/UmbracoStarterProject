using System.Runtime.Serialization;

namespace TD.SLA.Web.Helpers.Recaptcha
{
    public class GRequestModel
    {
        public string path { get; set; }
        public string secret { get; set; }
        public string response { get; set; }
        public string remoteip { get; set; }

        public GRequestModel(IConfiguration config, string recaptchaToken, string remip, bool isInvisible)
        {
            if (isInvisible)
            {
                secret = config["GoogleRecaptcha:InvisibleVersionSecret"];
            }
            else
            {
                secret = config["GoogleRecaptcha:Secret"];
            }

            path = config["GoogleRecaptcha:ApiUrl"];

            response = recaptchaToken;
            remoteip = remip;
        }
    }

    //Google's response property naming is 
    //embarrassingly inconsistent, that's why we have to 
    //use DataContract and DataMember attributes,
    //so we can bind the class from properties that have 
    //naming where a C# variable by that name would be
    //against the language specifications... (i.e., '-').
    [DataContract]
    public class GResponseModel
    {
        [DataMember]
        public bool success { get; set; }
        [DataMember]
        public string challenge_ts { get; set; }
        [DataMember]
        public string hostname { get; set; }
        //Could create a child object for 
        //error-codes
        [DataMember(Name = "error-codes")]
        public string[] error_codes { get; set; }
    }
}
