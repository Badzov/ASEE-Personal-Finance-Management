using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Services
{
    public class CategoryHierarchyService
    {
        private readonly Dictionary<string, Category> _categoryMap;

        public CategoryHierarchyService(IEnumerable<Category> categories)
        {
            _categoryMap = categories.ToDictionary(c => c.Code);
        }

        public string? GetTopLevelParentCode(string code)
        {
            if (code == null)
            {
                return null;
            }

            while (_categoryMap.TryGetValue(code, out var category) && category.ParentCode != null)
            {
                code = category.ParentCode;
            }
            return code;
        }

        public bool IsInCategoryTree(string targetCode, string childCode)
        {
            while (childCode != null)
            {
                if (childCode == targetCode)
                {
                    return true;
                }

                childCode = _categoryMap.TryGetValue(childCode, out var cat) ? cat.ParentCode : null;
            }
            return false;
        }
    }
}
