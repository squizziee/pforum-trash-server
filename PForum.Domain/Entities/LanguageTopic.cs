using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PForum.Domain.Entities
{
    public class LanguageTopic
    {
        public Guid Id { get; set; }
        public string LanguageName { get; set; } = string.Empty;
        public string LanguageDescription { get; set; } = string.Empty;
        public string LanguageLogoUrl { get; set; } = string.Empty;
        public IEnumerable<Topic> Topics { get; set; }
    }
}
