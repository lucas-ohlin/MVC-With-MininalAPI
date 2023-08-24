namespace Web_Coupon {
    public static class StaticDetails {

        public static string CouponAPIBase { get; set; }

        public enum APIType {
            GET, 
            POST, 
            PUT, 
            DELETE
        }

    }
}
