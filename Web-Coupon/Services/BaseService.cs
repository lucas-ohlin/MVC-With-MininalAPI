using Web_Coupon.Models;

namespace Web_Coupon.Services {
    public class BaseService : IBaseService {

        public ResponseDTO responseModel { get; set; }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public Task<T> SendAsync<T>(ApiRequest apiRequest) {
            throw new NotImplementedException();
        }

    }
}
