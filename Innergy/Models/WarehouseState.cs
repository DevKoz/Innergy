using System.Collections.Generic;
using System.Linq;

namespace Innergy.Models
{
    public class WarehouseState
    {
        public string Name { get; set; }
        private int totalAmount;
        public int TotalAmount
        {
            get
            {
                if (totalAmount == 0)
                {
                    totalAmount = MaterialStates.Sum(x => x.Value);
                }
                return totalAmount;
            }
        }

        public Dictionary<string, int> MaterialStates { get; set; }
    }
}
