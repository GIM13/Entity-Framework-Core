using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTO
{
    class SalesCustomerDTO
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("parts")]
        public List<PartDTO> Parts { get; set; }
    }
}
