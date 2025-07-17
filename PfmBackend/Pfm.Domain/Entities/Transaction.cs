using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Pfm.Domain.Enums;
using Pfm.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace Pfm.Domain.Entities
{
    public class Transaction
    {
        public string Id { get; set; }
        public string? BeneficiaryName { get; set; }
        public DateTime Date { get; set; }
        public DirectionsEnum Direction { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public string Currency { get; set; }
        public MccCodeEnum? Mcc { get; set; }
        public TransactionKindsEnum Kind { get; set; }
        public string? CatCode { get; set; }

        // Navigation
        public virtual Category Category { get; set; }
        public virtual ICollection<SingleCategorySplit> Splits { get; set; }


        public void Validate()
        {
            if (Amount <= 0)
            {
                throw new BusinessRuleException("invalid-amount", "Transaction amount must be positive");
            }

            if (Mcc.HasValue && !Enum.IsDefined(typeof(MccCodeEnum), Mcc.Value))
                throw new BusinessRuleException("invalid-mcc", $"Invalid MCC code: {Mcc.Value}");
        }
    }

}
