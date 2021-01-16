using DWorldProject.Services;

namespace DWorldProject.Models.IyziPay
{
    public interface ICheckoutFormSample
    {
        ServiceResult<CheckoutFormInitializeResource> Should_Initialize_Checkout_Form();
        ServiceResult<PaymentResource> Should_Retrieve_Checkout_Form_Result(CheckoutFormFinalizeModel model);
    }
}
