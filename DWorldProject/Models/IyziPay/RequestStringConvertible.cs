using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DWorldProject.Models.IyziPay
{
    public interface RequestStringConvertible
    {
        String ToPKIRequestString();
    }
}
