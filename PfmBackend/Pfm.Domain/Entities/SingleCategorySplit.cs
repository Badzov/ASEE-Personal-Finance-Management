using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pfm.Domain.Entities
{
    public class SingleCategorySplit
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CatCode { get; set; }
        public double Amount { get; set; }

        // Navigation
        public virtual Transaction Transaction { get; set; }
        public virtual Category Category { get; set; }
    }
}
