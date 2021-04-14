using DWorldProject.Services;
using DWorldProject.Utils;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DWorldProject.Models.IyziPay
{
    public class CheckoutFormSample : ICheckoutFormSample
    {
        private readonly IConfiguration _config;

        public CheckoutFormSample(IConfiguration config) { _config = config; }
        public ServiceResult<CheckoutFormInitializeResource> Should_Initialize_Checkout_Form()
        {
            var serviceResult = new ServiceResult<CheckoutFormInitializeResource>();

            CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest();
            request.Locale = "tr";
            request.ConversationId = "123456789";
            request.Price = "1";
            request.PaidPrice = "1.2";
            request.Currency = "TRY";
            request.BasketId = "B67832";
            request.PaymentGroup = "0";
            request.CallbackUrl = "https://localhost:44325/api/IyziPay/Finalize";

            List<int> enabledInstallments = new List<int>();
            enabledInstallments.Add(2);
            enabledInstallments.Add(3);
            enabledInstallments.Add(6);
            enabledInstallments.Add(9);
            request.EnabledInstallments = enabledInstallments;

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = "John";
            buyer.Surname = "Doe";
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem firstBasketItem = new BasketItem();
            firstBasketItem.Id = "BI101";
            firstBasketItem.Name = "Binocular";
            firstBasketItem.Category1 = "Collectibles";
            firstBasketItem.Category2 = "Accessories";
            firstBasketItem.ItemType = "PHYSICAL";
            firstBasketItem.Price = "0.3";
            basketItems.Add(firstBasketItem);

            BasketItem secondBasketItem = new BasketItem();
            secondBasketItem.Id = "BI102";
            secondBasketItem.Name = "Game code";
            secondBasketItem.Category1 = "Game";
            secondBasketItem.Category2 = "Online Game Items";
            secondBasketItem.ItemType = "PHYSICAL";
            secondBasketItem.Price = "0.5";
            basketItems.Add(secondBasketItem);

            BasketItem thirdBasketItem = new BasketItem();
            thirdBasketItem.Id = "BI103";
            thirdBasketItem.Name = "Usb";
            thirdBasketItem.Category1 = "Electronics";
            thirdBasketItem.Category2 = "Usb / Cable";
            thirdBasketItem.ItemType = "PHYSICAL";
            thirdBasketItem.Price = "0.2";
            basketItems.Add(thirdBasketItem);
            request.BasketItems = basketItems;

            var config = _config.GetSection("IyziPayOptions").Get<AppSettings>();

            Options opt = new Options();
            opt.ApiKey = config.ApiKey;
            opt.BaseUrl = config.BaseUrl;
            opt.SecretKey = config.SecretKey;
            IyziPayInitializeService checkoutFormInitialize = IyziPayInitializeService.Create(request, opt);

            var res = checkoutFormInitialize;
            serviceResult.Data = res;
            return serviceResult;
        }

        public ServiceResult<PaymentResource> Should_Retrieve_Checkout_Form_Result(CheckoutFormFinalizeModel model)
        {
            var serviceResult = new ServiceResult<PaymentResource>();
            RetrieveCheckoutFormRequest request = new RetrieveCheckoutFormRequest();
            //request.ConversationId = model.ConversationId;
            request.ConversationId = "123456789";
            request.Token = model.Token;

            var config = _config.GetSection("IyziPayOptions").Get<AppSettings>();

            Options opt = new Options();
            opt.ApiKey = config.ApiKey;
            opt.BaseUrl = config.BaseUrl;
            opt.SecretKey = config.SecretKey;

            IyziPayFinalizeService checkoutForm = IyziPayFinalizeService.Retrieve(request, opt);

            serviceResult.Data = checkoutForm;
            return serviceResult;
        }
    }
}
