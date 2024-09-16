using Newtonsoft.Json;
using System.Collections.Generic;

public record LayoutMap
{
    [JsonProperty("layouts")]
    public List<Layout> Layouts;
}

public record Layout
{
    public bool enable;
    public int zone;
    public List<LayoutItem> items;
    public List<LayoutType> nextShapes;
}

public record LayoutItem
{
	public int id { get; set; }
}