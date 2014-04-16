using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.Server;

namespace tfinfo
{
    public class TfsInfo
    {
        public IList<WorkItemInfo> WorkItems { get; set; }
        public IList<ChangesetInfo> Changes { get; set; }
        public IList<Iteration> Iterations { get; set; }

        internal static TfsInfo Collect(Options options) 
        {
            Uri collectionUri = new Uri(options.Collection);
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(collectionUri);

            var changes = GetChangesets(tpc, options);
            var workItems = GetWorkitems(tpc, options, changes);
            var iterations = GetIterations(tpc, options, workItems);

            return new TfsInfo() { WorkItems = workItems, Changes = changes, Iterations = iterations };		
        }

        private static List<Iteration> GetIterations(TfsTeamProjectCollection tpc, Options options, IList<WorkItemInfo> workitems)
        {
            var iterations = new List<Iteration>();

            WorkItemStore workItemStore = tpc.GetService<WorkItemStore>();
            Project teamProject = workItemStore.Projects[options.Project];
            var css = tpc.GetService<ICommonStructureService>();

            foreach (Node iter in teamProject.IterationRootNodes)
            {
                var i = new Iteration() { Id = iter.Id, Uri = iter.Uri.ToString(), Name = iter.Name };
                var info = css.GetNode(i.Uri);
                i.StartDate = info.StartDate;
                i.FinishDate = info.FinishDate;
                i.WorkItems.AddRange(workitems.Where(wi => wi.IterationId == i.Id));

                iterations.Add(i);
            }

            return iterations.OrderByDescending(i => i.StartDate).ToList();
        }

        private static IList<ChangesetInfo> GetChangesets(TfsTeamProjectCollection tpc, Options options)
        {
            var changes = new List<ChangesetInfo>();
            if (options.NoChangesets) return changes;

            VersionControlServer versionControl = tpc.GetService<VersionControlServer>();
            var css = versionControl.QueryHistory(options.Branch, RecursionType.Full);
            foreach (var cs in css) 
            {
                ChangesetInfo change = new ChangesetInfo()
                {
                    Id = cs.ChangesetId.ToString(),
                    Uri = cs.ArtifactUri.ToString(),
                    FullUri = cs.ArtifactUri.ToString() + "?url=" + tpc.Uri.AbsoluteUri,
                    Comment = cs.Comment,
                    Author = cs.CommitterDisplayName,
                    CreatedAt = cs.CreationDate
                };

                changes.Add(change);

                foreach (var note in cs.CheckinNote.Values)
                {
                    change.Notes.Add(new Property()
                    {
                        Name = note.Name,
                        Value = note.Value
                    });
                }

                foreach (var wi in cs.AssociatedWorkItems)
                {
                    change.Related.Add(new WorkItemInfo()
                    {
                        Id = wi.Id.ToString(),
                        Type = wi.WorkItemType,
                        State = wi.State,
                        Title = wi.Title,
                    });
                }

                foreach (var file in versionControl.ArtifactProvider.GetChangeset(cs.ArtifactUri).Changes)
                {
                    change.Changes.Add(new Change()
                    {
                        Type = file.ChangeType.ToString(),
                        FullPath = file.Item.ServerItem,
                        Path = file.Item.ServerItem.Substring(options.Branch.Length)
                    });

                }
            }

            return changes;
        }

        private static IList<WorkItemInfo> GetWorkitems(TfsTeamProjectCollection tpc, Options options, IList<ChangesetInfo> changesets)
        {
            var workItems = new Dictionary<string, WorkItemInfo>();

            if (options.NoWorkItems) return workItems.Values.ToList();

            WorkItemStore workItemStore = tpc.GetService<WorkItemStore>();
            Project teamProject = workItemStore.Projects[options.Project];
            string wiql = string.Format("SELECT * FROM WorkItems WHERE [System.TeamProject] = '{0}' ORDER BY [System.Id] ", options.Project);
            WorkItemCollection wic = workItemStore.Query(wiql);

            foreach (WorkItem wi in wic)
            {
                var info = new WorkItemInfo()
                {
                    Type = wi.Type.Name,
                    Id = wi.Id.ToString(),
                    Uri = wi.Uri.ToString(),
                    FullUri = wi.Uri + "?url=" + tpc.Uri.AbsoluteUri,
                    Title = wi.Title,
                    State = wi.State,
                    Tags = wi.Tags,
                    Author = wi.CreatedBy,
                    CreatedAt = wi.CreatedDate,
                    Description = wi.Description,
                    IterationId = wi.IterationId,
                    Iteration = wi.IterationPath
                };
                workItems[info.Id] = info;

                foreach (Link li in wi.Links)
                {
                    var ex = li as ExternalLink;
                    if (ex != null)
                    {
                        info.Related.Add(new WorkItemInfo()
                        {
                            Type = li.ArtifactLinkType.Name,
                            Uri = ex.LinkedArtifactUri,
                            FullUri = ex.LinkedArtifactUri + "?url=" + tpc.Uri.AbsoluteUri,
                            Id = ex.LinkedArtifactUri.Split('/').Last(),
                            Title = ex.Comment
                        });
                    }
                }
            }

            foreach (var cs in changesets)
            {
                foreach (var wi in cs.Related)
                {
                    WorkItemInfo rel;
                    if(workItems.TryGetValue(wi.Id, out rel))
                        rel.Changesets.Add(cs);
                }
            }

            return workItems.Values.ToList();
        }
    }
}
