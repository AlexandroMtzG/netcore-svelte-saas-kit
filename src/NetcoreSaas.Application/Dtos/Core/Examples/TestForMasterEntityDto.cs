using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetcoreSaas.Application.Dtos.Core.Examples
{
    public class TestForMasterEntityDto : MasterEntityDto
    {
        public int Integer { get; set; }
        public decimal Decimal { get; set; }
        [Required]
        public string String { get; set; }
        public byte[] File { get; set; }
        public DateTime Date { get; set; }
        public List<int> Numbers { get; set; }
        public List<string> Strings { get; set; }
        public List<DateTime> Dates { get; set; }
        public List<byte[]> Files { get; set; }
        public TestForMasterEntityDto()
        {
            Numbers = new List<int>();
            Strings = new List<string>();
            Dates = new List<DateTime>();
            Files = new List<byte[]>();
        }
    }
}