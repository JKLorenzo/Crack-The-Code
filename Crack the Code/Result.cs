using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crack_the_Code
{
    public class Result
    {
        public string Name { get; set; }
        public string Remarks { get; set; }
        public string RemainingTime { get; set; }
        public string Date { get; set; }
        public Result(string name, string remarks, string remainingtime, string date)
        {
            Name = name;
            Remarks = remarks;
            RemainingTime = remainingtime;
            Date = date;
        }
    }
}
