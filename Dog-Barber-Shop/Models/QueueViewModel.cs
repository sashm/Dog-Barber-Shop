using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dog_Barber_Shop.Models
{
    public class QueueViewModel
    {
        public int customerID { get; set; }

        public string customerName { get; set;}

        public DateTime queueTime { get; set; }

        public DateTime creationQueueTime { get; set; }

        public IEnumerable<QueueViewModel> Customers { get; set; }

        public class SomeViewModel
        {
            public SomeViewModel()
            {
                samples = new List<QueueViewModel>();
                QueueViewModel = new QueueViewModel();
            }
            public List<QueueViewModel> samples { get; set; }
            public QueueViewModel QueueViewModel { get; set; }
        }
       
    }
}
