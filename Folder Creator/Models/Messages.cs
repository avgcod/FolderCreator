namespace Folder_Creator.Models
{
    public record class OperationErrorMessage(string ErrorType, string ErrorMessage);
    public record class OperationErrorInfoMessage(string ErrorType, string ErrorMessage);
    public record class NotificationMessage(string MessageText);
}
