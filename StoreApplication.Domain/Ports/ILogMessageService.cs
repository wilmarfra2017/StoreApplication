namespace StoreApplication.Domain.Ports;

public interface ILogMessageService
{
    string ValidationFailed { get; }
    string RoleAdmin { get; }
    string ClaimId { get; }    
    string OrderCreatedSuccess { get; }
    string ProductCreatedSuccess { get; }
    string ParamsPageGreaterZero { get; }
    string NoProductsFound { get; }
    string InternalServerError { get; }
    string User { get; }
    string AdminCredentialsNotValid { get; }
    string ErrorSavingChanges { get; }
}
