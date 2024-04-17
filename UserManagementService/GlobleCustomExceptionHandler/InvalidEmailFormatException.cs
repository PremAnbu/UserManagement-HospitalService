namespace UserManagementService.GlobleCustomExceptionHandler
{
    public class InvalidEmailFormatException : Exception
    {
        public InvalidEmailFormatException(string message) : base(message) { }
    }
}
