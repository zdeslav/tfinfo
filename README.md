TfInfo
======

TfInfo.exe is a command line tool to extract data from TFS server and convert it
to a readable format in a simple way.

This is done by specifying the template file which will be used to process data
retrieved from TFS.

Usage
-----

~~~
tfinfo.exe -c collection -p project -b branch [-t template] [-m N] [--nowi|nocs]
~~~
 
Parameters:

~~~
  -c, --collection    Required. TFS project collection
  -p, --project       Required. Name of the TFS projects
  -b, --branch        Required. Name of the TFS branch
  -t, --template      (Default: summary.txt) template file for output
  --nowi              Skip retrieving work items
  --nocs              Skip retrieving changesets
  -m, --max_age       NOT IMPLEMENTED! Show only entries from last N days
  --wiq               NOT IMPLEMENTED! Query to select work items to be shown
  --csq               NOT IMPLEMENTED! Query to select changesets to be shown
  --help              Display this help screen.
~~~

Templates
---------

TfInfo comes with a set of predefined templates (take a look at `templates`
directory). It is also possible to specify a different template by providing
`-t, --template`.

If an absolute path is specified, then template is looked up at specified 
location. If a relative path is specified, it is relative to `templates` 
subdirectory of the directory containing `tfinfo.exe`.

### Template syntax

TfInfo uses [DotLiquid] library to process templates. It is a port of Ruby's
[Liquid] template engine. 

You can find more details about Liquid syntax 
[here](https://github.com/Shopify/liquid/wiki/Liquid-for-Designers).

The differences introduced by DotLiquid are described 
[here](https://github.com/formosatek/dotliquid/wiki/DotLiquid-for-Designers).

The only major difference is that instead of [default naming convention], TfInfo
uses C# naming convention, so if you want to display a property called 
`SomeField`, you will use `{{ SomeField }}` and not `{{ some_field }}`

### Writing templates

Here's an example of a simple template which produces markdown formatted list
of workitems and changesets:

~~~
Workitems
---------

{% for item in WorkItems -%}
* `{{ item.Type }} {{ item.Id }}` **[{{ item.State | Upcase }}]**: {{ item.Title }}
{% for rel in item.Related -%}
    * {{ rel.Type }} {{ rel.Id }}
{% endfor -%}
{% endfor -%}

Changesets
----------

{% for ch in Changes -%}
* `{{ ch.Id }}` - {{ ch.Comment }} *[{{ ch.FileCount }} files -  {{ ch.Author }} @ {{ ch.CreatedAt }}]*
{% for rel in ch.Related -%}
    * Related: {{ rel.Type }} `#{{ rel.Id }}` **[{{ rel.State | Upcase }}]** - {{ rel.Title }}
{% endfor -%}
{% endfor -%}
~~~

Please check other provided templates.

### Object model

Template files take as input a set of .NET objects. `WorkItems` root object is
a list of `WorkItemInfo` objects, and `Changes` root object is a list of 
`ChangesetInfo` objects. The attributes of these objects are then used to 
provide data for templates. Here's the [model](d:\dev\tfinfo\tfinfo\Model.cs) 
which is currently used:

~~~{.cs}
    public class WorkItemInfo
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Uri { get; set; }
        public string FullUri { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
        public string Tags { get; set; }
        public string Author { get; set; }
        public string Iteration { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WorkItemInfo> Related { get; }     
        public string RelatedIds { get; } // comma separated list of IDs
    }

    public class Change
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ChangesetInfo
    {
        public string Id { get; set; }
        public string Uri { get; set; }
        public string FullUri { get; set; }
        public string Comment { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Change> Changes { get; } // list of changed files
        public List<WorkItemInfo> Related { get; }
        public List<Property> Notes { get; } 
        public int FileCount { get; } 
        public string AuthorInitials { get; } 
    }
~~~

[DotLiquid]: https://github.com/formosatek/dotliquid
[Liquid]: http://liquidmarkup.org/
[default naming convention]: https://github.com/formosatek/dotliquid/wiki/DotLiquid-for-Designers#filter-and-output-casing