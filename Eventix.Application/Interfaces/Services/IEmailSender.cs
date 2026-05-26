using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services
{
    public interface IEmailSender
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetUrl, CancellationToken ct);
    }
}
