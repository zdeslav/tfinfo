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


