using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Error
    {
        public int ErrorID { get; set; }
        public int Level { get; set; }
        public string Message { get; set; }
    }
}