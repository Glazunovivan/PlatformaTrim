using PlatformaTrim.Core;
using System;

namespace PlatformaTrim.Models
{
    public class UserData
    {
        public string Fullname { get; set; } = string.Empty;

        public string NumberCertificate { get; set; } = string.Empty;

        public DateTime DateStart { get; set; } = DateTime.Now.AddYears(1);

        public DateTime DateEnd { get; set; } = DateTime.Now;
    }
}
