using System.Collections.Generic;

namespace VolunteersProject.Email
{
    public interface IEmailService
    {
        List<EmailMessage> ReceiveEmail(int maxCount = 10);

        void Send(EmailMessage emailMessage);
    }
}