using DWorldProject.Models.IyziPay;

namespace DWorldProject.Services
{
    public class IyziPayInitializeService : CheckoutFormInitializeResource
    {
        public static IyziPayInitializeService Create(CreateCheckoutFormInitializeRequest request, Options options)
        {
            return RestHttpClient.Create().Post<IyziPayInitializeService>(options.BaseUrl + "/payment/iyzipos/checkoutform/initialize/auth/ecom", GetHttpHeaders(request, options), request);
        }
    }
}
