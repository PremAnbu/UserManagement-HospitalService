namespace UserManagementService.Interface
{
    public interface IEmail
    {
        Task<bool> EmailSenderAsync(string to, string subject, string htmlMessage);
    }
}
