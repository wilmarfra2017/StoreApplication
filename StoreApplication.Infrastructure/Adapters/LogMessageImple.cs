using StoreApplication.Domain.Ports;
using StoreApplication.Infrastructure.Resources;

namespace StoreApplication.Infrastructure.Adapters;

public class LogMessageImple : ILogMessageService
{
    public string ValidationFailed => AppMessages.CREDENTIALS_FAILED;
    public string RoleAdmin => AppMessages.ADMIN;
    public string ClaimId => AppMessages.CLAIM_ID;    
    public string JwtTokenExpitarion => AppMessages.JWT_TOKEN_EXPIRATION;
    public string OrderCreatedSuccess => AppMessages.ORDER_CREATED_SUCCESS;
    public string ProductCreatedSuccess => AppMessages.PRODUCT_CREATED_SUCCESS;
    public string ParamsPageGreaterZero => AppMessages.PARAMS_PAGE_GREATER_ZERO;
    public string NoProductsFound => AppMessages.NO_PRODUCTS_FOUND;
    public string InternalServerError => AppMessages.INTERNAL_SERVER_ERROR;
    public string User => AppMessages.USER;
    public string AdminCredentialsNotValid => AppMessages.ADMIN_CREDENTIAL_NOT_VALID;
    public string ErrorSavingChanges => AppMessages.ERROR_SAVING_CHANGES;
}
