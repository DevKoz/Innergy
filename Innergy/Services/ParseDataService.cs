using Innergy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innergy.Services
{
    public class ParseDataService
    {
        public string ParseData(string input)
        {
            var correctMaterials = GetCorrectMaterials(input);

            var result = PrepareWarehousesState(correctMaterials);

            return result;
        }

        public string PrepareWarehousesState(List<Material> correctMaterials)
        {
            var warehousesState = GetWarehousesStates(correctMaterials);

            warehousesState = OrderWarehousesStates(warehousesState);

            var result = PrepareWarehousesStateString(warehousesState);

            return result;
        }

        public List<Material> GetCorrectMaterials(string input)
        {
            var correctMaterials = new List<Material>();
            var lines = input.Split('\n');

            foreach (var line in lines)
            {
                var material = GetMaterial(line);
                if (material != null)
                {
                    correctMaterials.Add(material);
                }
            }

            return correctMaterials;
        }

        private string PrepareWarehousesStateString(List<WarehouseState> warehousesState)
        {
            var sb = new StringBuilder();

            foreach (var warehouseState in warehousesState)
            {
                sb.AppendLine(warehouseState.Name + " (total  " + warehouseState.TotalAmount + ")");
                foreach (var materialState in warehouseState.MaterialStates)
                {
                    sb.AppendLine(materialState.Key + ": " + materialState.Value);
                }

                if (!warehouseState.Equals(warehousesState.Last()))
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private List<WarehouseState> OrderWarehousesStates(List<WarehouseState> warehousesState)
        {
            warehousesState = warehousesState.OrderByDescending(x => x.TotalAmount).ThenByDescending(x => x.Name).ToList();

            foreach (var warehouseState in warehousesState)
            {
                var items = from materialState in warehouseState.MaterialStates
                            orderby materialState.Key ascending
                            select materialState;
                warehouseState.MaterialStates = items.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            }

            return warehousesState;
        }

        private List<WarehouseState> GetWarehousesStates(List<Material> correctMaterials)
        {
            var result = new List<WarehouseState>();

            foreach (var correctMaterial in correctMaterials)
            {
                foreach (var warehouse in correctMaterial.Warehouses)
                {
                    var warehouseState = result.FirstOrDefault(x => new String(x.Name.Where(y => Char.IsLetterOrDigit(y)).ToArray()) == new String(warehouse.Name.Where(y => Char.IsLetterOrDigit(y)).ToArray()));

                    if (warehouseState == null)
                    {
                        warehouseState = new WarehouseState()
                        {
                            Name = warehouse.Name,
                            MaterialStates = new Dictionary<string, int>()
                            {
                                { correctMaterial.Id, warehouse.Amount }
                            }
                        };

                        result.Add(warehouseState);
                    }
                    else
                    {
                        if (warehouseState.MaterialStates.ContainsKey(correctMaterial.Id))
                        {
                            warehouseState.MaterialStates[correctMaterial.Id] += warehouse.Amount;
                        }
                        else
                        {
                            warehouseState.MaterialStates.Add(correctMaterial.Id, warehouse.Amount);
                        }
                    }
                }
            }

            return result;
        }

        private Material GetMaterial(string line)
        {
            var mainElements = line.Split(';');

            if (mainElements.Length != 3)
            {
                //ignore item if no valid number of parameters
                return null;
            }

            var warehouses = GetWarehouses(mainElements[2]);

            if (warehouses == null)
            {
                //ignore item if no warehouses
                return null;
            }

            var material = new Material();
            material.Name = mainElements[0];
            material.Id = mainElements[1];

            material.Warehouses = warehouses;

            return material;
        }

        private List<Warehouse> GetWarehouses(string warehousePart)
        {
            var warehousesElements = warehousePart.Split('|');

            if (warehousesElements.Length == 0)
            {
                //ignore item if no warehouses
                return null;
            }

            var warehouses = new List<Warehouse>();

            foreach (var warehouseElements in warehousesElements)
            {
                var warehouse = GetWarehouse(warehouseElements);

                if (warehouse == null)
                {
                    return null;
                }

                warehouses.Add(warehouse);
            }

            return warehouses;
        }

        private Warehouse GetWarehouse(string warehouseElements)
        {
            var elements = warehouseElements.Split(',');

            if (elements.Length != 2 || string.IsNullOrWhiteSpace(elements[0]))
            {
                return null;
            }

            int amount;

            if (!int.TryParse(elements[1], out amount))
            {
                return null;
            }

            var warehouse = new Warehouse()
            {
                Name = elements[0],
                Amount = amount
            };

            return warehouse;
        }
    }
}
