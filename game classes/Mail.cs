using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuusha.Mail
{
   public class Mail
    {
        public string Sender
        { get; set; }

        public string Title
        { get; set; }

        public string Body
        { get; set; }

        public DateTime TimeSent
        { get; set; }

        public Item AttachedItem
        { get; set; }

        public double RequiredPayment
        { get; set; }

        public bool HasBeenRead
        { get; set; }

        public DateTime TimeRead
        { get; set; }
    }
}
