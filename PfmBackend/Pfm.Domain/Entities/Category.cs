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
        public string Code { get; set; }
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
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdateParentCode(string parentCode)
        {
            ParentCode = parentCode;
        }

    }
}
