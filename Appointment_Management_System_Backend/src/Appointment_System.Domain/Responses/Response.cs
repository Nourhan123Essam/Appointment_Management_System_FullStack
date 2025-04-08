using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Responses
{
    public record Response(bool Flag = false, string Message = null);

}
