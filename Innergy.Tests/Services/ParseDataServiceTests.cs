using NUnit.Framework;
using Innergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innergy.Models;

namespace Innergy.Services.Tests
{
    [TestFixture()]
    public class ParseDataServiceTests
    {
        [Test()]
        public void ParseDataTest()
        {
            string input = "# Material inventory initial state as of Jan 01 2018\n" +
                "# New materials\n" +
                "Cherry Hardwood Arched Door -PS; COM - 100001; WH - A,5 | WH - B,10\n" +
                "Maple Dovetail Drawerbox; COM - 124047; WH - A,15\n" +
                "Generic Wire Pull; COM - 123906c; WH - A,10 | WH - B,6 | WH - C,2\n" +
                "Yankee Hardware 110 Deg.Hinge; COM - 123908; WH - A,10 | WH - B,11\n" +
                "# Existing materials, restocked\n" +
                "Hdw Accuride CB0115 - CASSRC - Locking Handle Kit -Black; CB0115 - CASSRC; WH - C,13 | WHB,5\n" +
                "Veneer - Charter Industries - 3M Adhesive Backed -Cherry 10mm - Paper Back; 3M - Cherry - 10mm; WH - A,10 | WH - B,1\n" +
                "Veneer - Cherry Rotary 1 FSC; COM - 123823; WH - C,10\n" +
                "MDF, CARB2, 1 1 / 8\";COM-101734;WH-C,8";

            var expectedresult = " WH - A (total  50)\r\n 3M - Cherry - 10mm: 10\r\n COM - 100001: 5\r\n COM - 123906c: 10\r\n COM - 123908: 10\r\n COM - 124047: 15\r\n\r\n WH - C (total  33)\r\n CB0115 - CASSRC: 13\r\n COM - 123823: 10\r\n COM - 123906c: 2\r\nCOM-101734: 8\r\n\r\n WH - B (total  33)\r\n 3M - Cherry - 10mm: 1\r\n CB0115 - CASSRC: 5\r\n COM - 100001: 10\r\n COM - 123906c: 6\r\n COM - 123908: 11\r\n";

            var service = new ParseDataService();
            var result = service.ParseData(input);

            Assert.AreEqual(result, expectedresult);
        }

        [Test()]
        public void ParseDataTestWithEmptyInput()
        {
            var service = new ParseDataService();
            var result = service.ParseData(string.Empty);

            Assert.AreEqual(string.Empty, result);
        }

        [Test()]
        public void ParseDatawithOneRecord()
        {
            var input = "Generic Wire Pull; COM - 123906c; WH - A,10 | WH - B,6 | WH - C,2";

            var service = new ParseDataService();
            var result = service.ParseData(input);
            var expectedresult = " WH - A (total  10)\r\n COM - 123906c: 10\r\n\r\n WH - B (total  6)\r\n COM - 123906c: 6\r\n\r\n WH - C (total  2)\r\n COM - 123906c: 2\r\n";

            Assert.AreEqual(result, expectedresult);
        }

        [Test()]
        public void PrepareWarehousesStateTest()
        {
            var input = new List<Material>()
            {
                new Material()
                {
                    Id = "1m",
                    Name = "1mm",
                    Warehouses = new List<Warehouse>()
                    {
                        new Warehouse()
                        {
                            Name = "w1",
                            Amount = 5
                        },
                        new Warehouse()
                        {
                            Name = "w2",
                            Amount = 4
                        },
                        new Warehouse()
                        {
                            Name = "w3",
                            Amount = 2
                        },
                    }
                },
                new Material()
                {
                    Id = "2n",
                    Name = "2nn",
                    Warehouses = new List<Warehouse>()
                    {
                        new Warehouse()
                        {
                            Name = "w1",
                            Amount = 1
                        },
                        new Warehouse()
                        {
                            Name = "w2",
                            Amount = 10
                        },
                        new Warehouse()
                        {
                            Name = "w3",
                            Amount = 1
                        },
                    }
                }
            };

            var service = new ParseDataService();
            var result = service.PrepareWarehousesState(input);

            var expetedResult = "w2 (total  14)\r\n1m: 4\r\n2n: 10\r\n\r\nw1 (total  6)\r\n1m: 5\r\n2n: 1\r\n\r\nw3 (total  3)\r\n1m: 2\r\n2n: 1\r\n";

            Assert.AreEqual(expetedResult, result);
        }

        [Test()]
        public void GetCorrectMaterialsTest()
        {
            string input = "# Material inventory initial state as of Jan 01 2018\n" +
                "# New materials\n" +
                "Cherry Hardwood Arched Door -PS; COM - 100001; WH - A,5 | WH - B,10\n" +
                "Maple Dovetail Drawerbox; COM - 124047; WH - A,15\n" +
                "Generic Wire Pull; COM - 123906c; WH - A,10 | WH - B,6 | WH - C,2\n" +
                "Yankee Hardware 110 Deg.Hinge; COM - 123908; WH - A,10 | WH - B,11\n" +
                "# Existing materials, restocked\n" +
                "Hdw Accuride CB0115 - CASSRC - Locking Handle Kit -Black; CB0115 - CASSRC; WH - C,13 | WHB,5\n" +
                "Veneer - Charter Industries - 3M Adhesive Backed -Cherry 10mm - Paper Back; 3M - Cherry - 10mm; WH - A,10 | WH - B,1\n" +
                "Veneer - Cherry Rotary 1 FSC; COM - 123823; WH - C,10\n" +
                "MDF, CARB2, 1 1 / 8\";COM-101734;WH-C,8";

            var service = new ParseDataService();
            var result = service.GetCorrectMaterials(input);

            Assert.AreEqual(8, result.Count());
        }
    }
}