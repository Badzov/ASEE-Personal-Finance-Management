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
        [StringLength(8)]
        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        public string TransactionId { get; set; }

        [Required]
        [StringLength(4)]
        public string CatCode { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double Amount { get; set; }

        // Navigation

        public virtual Transaction Transaction { get; set; }

        [ForeignKey(nameof(CatCode))]
        public virtual Category Category { get; set; }
    }
}
