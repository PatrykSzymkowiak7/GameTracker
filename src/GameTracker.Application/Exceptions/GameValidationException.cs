using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Application.Exceptions
{
    public class GameValidationException : Exception 
    {
        public GameValidationException(string message) 
            : base(message)
        {
        }
    }
}
