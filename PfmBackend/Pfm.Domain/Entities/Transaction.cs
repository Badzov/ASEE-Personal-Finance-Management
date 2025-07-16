using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Pfm.Domain.Enums;
using Pfm.Domain.Exceptions;

namespace Pfm.Domain.Entities
{
    public class Transaction
    {
        [Key]
        [Required]
        [StringLength(8)]
        public string Id { get; set; }

        [StringLength(50)]
        public string? BeneficiaryName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TransactionDirection Direction { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double Amount { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        public MccCode? Mcc { get; set; }

        [Required]
        public TransactionKind Kind { get; set; }

        [StringLength(4)]
        public string? CatCode { get; set; }

        // Navigation

        [ForeignKey(nameof(CatCode))]
        public virtual Category Category { get; set; }

        public virtual ICollection<Split> Splits { get; set; }


        public void Validate()
        {
            if (Amount <= 0)
            {
                throw new BusinessRuleException("invalid-amount", "Transaction amount must be positive");
            }

            if (Mcc.HasValue && !Enum.IsDefined(typeof(MccCode), Mcc.Value))
                throw new BusinessRuleException("invalid-mcc", $"Invalid MCC code: {Mcc.Value}");
        }
    }

}
