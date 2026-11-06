namespace MAHAK.Integration.Api.Errors;

public sealed class RequestValidationException(string message) : Exception(message);

public sealed class SchemaConfirmationRequiredException(string message) : Exception(message);

public sealed class AccountingRecordNotFoundException(string message) : Exception(message);

public sealed class AccountingDataIntegrityException(string message) : Exception(message);
