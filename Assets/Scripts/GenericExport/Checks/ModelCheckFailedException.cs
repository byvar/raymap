using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Checks
{
    public class ModelCheckFailedException : Exception
    {
        public ModelCheckFailedException(string message) : base(message) { }
    }
}
