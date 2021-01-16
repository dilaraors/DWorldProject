using DWorldProject.Models.IyziPay;

namespace DWorldProject.Services
{
    public class IyziPayFinalizeService : PaymentResource
    {
        public static IyziPayFinalizeService Retrieve(RetrieveCheckoutFormRequest request, Options options)
        {
            return RestHttpClient.Create().Post<IyziPayFinalizeService>(options.BaseUrl + "/payment/iyzipos/checkoutform/auth/ecom/detail", GetHttpHeaders(request, options), request);
        }
    }
}
