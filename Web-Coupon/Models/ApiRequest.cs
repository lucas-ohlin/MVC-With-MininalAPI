using static Web_Coupon.StaticDetails;

namespace Web_Coupon.Models {
    public class ApiRequest {

        public APIType ApiType { get; set; }

        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }

    }
}
