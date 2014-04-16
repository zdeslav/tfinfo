using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace tfinfo
{
    [LiquidType("Id", "Name", "StartDate", "FinishDate", "Uri", "WorkItems")]
    public class Iteration
    {
        public Iteration() { WorkItems = new List<WorkItemInfo>(); }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string Uri { get; set; }
        public List<WorkItemInfo> WorkItems { get; private set; }		

    }

    [LiquidType("Type", "Id", "Uri", "FullUri", "Title", "State", "Tags", 
        "Iteration", "Author", "CreatedAt", "Related", "RelatedIds", 
        "Changesets", "Description")]
    public class WorkItemInfo
    {
        public WorkItemInfo() 
        { 
            Related = new List<WorkItemInfo>();
            Changesets = new List<ChangesetInfo>();
        }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Uri { get; set; }
        public string FullUri { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
        public string Tags { get; set; }
        public string Author { get; set; }
        public string Iteration { get; set; }
        public string Description { get; set; }
        public int IterationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WorkItemInfo> Related { get; private set; }
        public List<ChangesetInfo> Changesets { get; private set; }		
        public string RelatedIds // comma separated list of IDs
        {   
            get { return string.Join(",", Related.Select(r => r.Id)); }
        }
    }

    [LiquidType("Type", "Path", "FullPath")]
    public class Change
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
    }

    [LiquidType("Name", "Value")]
    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    [LiquidType("Id", "Uri", "FullUri", "Comment", "Author", "CreatedAt", 
        "Changes", "Related", "Notes", "FileCount", "AuthorInitials")]
    public class ChangesetInfo
    {
        public ChangesetInfo()
        {
            Changes = new List<Change>();
            Related = new List<WorkItemInfo>();
            Notes = new List<Property>();
        }
        public string Id { get; set; }
        public string Uri { get; set; }
        public string FullUri { get; set; }
        public string Comment { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Change> Changes { get; private set; }
        public List<WorkItemInfo> Related { get; private set; }
        public List<Property> Notes { get; private set; }
        public int FileCount { get { return Changes.Count; } }
        public string AuthorInitials 
        {
            get { return string.Concat(Author.ToCharArray().Where(c => Char.IsUpper(c))); } 
        }
    }
}
