namespace Folder_Creator.Models
{
    /// <summary>
    /// Message to represent an operation error.
    /// </summary>
    /// <param name="ErrorType">The string name of the error type.</param>
    /// <param name="ErrorMessage">the error message.</param>
    public record class OperationErrorMessage(string ErrorType, string ErrorMessage);
    
    /// <summary>
    /// Message to represent a notification.
    /// </summary>
    /// <param name="MessageText">The notification message.</param>
    public record class NotificationMessage(string MessageText);
}
