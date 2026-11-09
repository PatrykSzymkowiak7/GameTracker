using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Application.Exceptions
{
    public class RegistrationException : Exception
    {
        public IReadOnlyCollection<string> Errors { get; }

        public RegistrationException(IEnumerable<string> errors)
            : base("User registration failed.")
        {
            Errors = errors.ToList();
        }
    }
}
