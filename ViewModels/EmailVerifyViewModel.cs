using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class EmailVerifyViewModel
    {
        public string EmailToken { get; set; }

        public bool IsEmailVerified { get; set; }
    }
}