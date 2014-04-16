RELEASE NOTES
-------------

{% for iter in Iterations -%}
{% if iter.WorkItems != empty -%}
## {{ iter.Name }}

{% for item in iter.WorkItems -%}
{% if item.State == "Closed" -%}
##### {{ item.Type }} {{ item.Id }}: {{ item.Title }}

* Created by: {{ item.Author }}
{% for rel in item.Related -%}
* {{ rel.Type }}: [{{ rel.Id }}]({{ rel.FullUri }})
{% endfor -%}
* Notes: {{ item.Description }}

{% endif -%}
{% endfor -%}


{% endif -%}
{% endfor %}
