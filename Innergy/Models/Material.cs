using System.Collections.Generic;

namespace Innergy.Models
{
    public class Material
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<Warehouse> Warehouses { get; set; }
    }
}
