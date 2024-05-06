using CoolWebApi.Utils.Entities.Contracts;

namespace CoolWebApi.Utils.Entities.Misc
{
    public class DocumentType : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}