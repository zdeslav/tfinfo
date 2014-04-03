WORKITEMS
---------

{% for item in WorkItems -%}
* [{{ item.Type }} {{ item.Id }} [{{ item.State | Upcase }}]]({{ item.FullUri }}): - {{ item.Title }}
{% for rel in item.Related -%}
    * {{ rel.Type }}: [{{ rel.Id }}]({{ rel.FullUri }})
{% endfor -%}
{% endfor -%}


CHANGESETS
----------

{% for ch in Changes -%}
* [{{ ch.Id }}]({{ ch.FullUri }}) - {{ ch.Comment }}
    * Author: {{ ch.Author }} @ {{ ch.CreatedAt }}
{% for rel in ch.Related -%}
    * Related: {{ rel.Type }} #{{ rel.Id }} [{{ rel.State | Upcase }} - {{ rel.Title }}
{% endfor -%}
    * Files:
{% for file in ch.Changes -%}
        * {{ file.Type }}  - `{{ file.Path }}`
{% endfor -%}
{% endfor -%}


