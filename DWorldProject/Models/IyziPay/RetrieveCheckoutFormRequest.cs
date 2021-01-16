using System;

namespace DWorldProject.Models.IyziPay
{
    public class RetrieveCheckoutFormRequest : BaseRequest
    {
        public string Token { set; get; }

        public override string ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .Append("token", Token)
                .GetRequestString();
        }
    }
}

