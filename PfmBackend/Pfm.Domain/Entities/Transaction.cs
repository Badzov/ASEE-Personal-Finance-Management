using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pfm.Domain.Enums;

namespace Pfm.Domain.Entities
{
    public class Transaction
    {
        [Key]
        [Required]
        public string Id { get; set; } 

        public string? BeneficiaryName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TransactionDirection Direction { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        public MccCode? Mcc { get; set; }

        [Required]
        public TransactionKind Kind { get; set; }

        public string? CatCode { get; set; }

        // Navigation

        [ForeignKey(nameof(CatCode))]
        public virtual Category Category { get; set; }

        public virtual ICollection<Split> Splits { get; set; }
    }

}
