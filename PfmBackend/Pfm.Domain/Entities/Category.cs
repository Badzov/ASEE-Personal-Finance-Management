using Pfm.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Entities
{
    public class Category
    {
        [Key]
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string? ParentCode { get; set; }

        // Navigation

        public virtual Category? Parent { get; set; }

        public virtual ICollection<Category>? Subcategories { get; set; } 

        public virtual ICollection<Transaction>? Transactions { get; set; }

        public Category(string code, string name, string? parentCode = null)
        {
            Code = code;
            Name = name;
            ParentCode = parentCode;
            Validate();
        }

        public void UpdateName(string name)
        {
            Name = name;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Code))
                throw new DomainException("invalid-code", "Category code is required");

            if (string.IsNullOrWhiteSpace(Name))
                throw new DomainException("invalid-name", "Category name is required");
        }
    }
}
