using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Application.Exceptions
{
    public class DeveloperNotFoundException : Exception
    {
        public DeveloperNotFoundException(int id) 
            : base($"Developer with id: {id} not found")
        {
        }
    }
}
