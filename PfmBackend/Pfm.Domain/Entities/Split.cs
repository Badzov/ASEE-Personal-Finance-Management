using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Entities
{
    public class Split
    {

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string TransactionId { get; set; }

        [Required]
        public string CatCode { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // Navigation

        public virtual Transaction Transaction { get; set; }

        [ForeignKey(nameof(CatCode))]
        public virtual Category Category { get; set; }
    }
}
