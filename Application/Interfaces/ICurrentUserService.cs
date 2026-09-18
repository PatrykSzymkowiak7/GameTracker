using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
    }
}
