namespace UserManagementService.GlobleCustomExceptionHandler
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string message): base(message) { }
    }
}
