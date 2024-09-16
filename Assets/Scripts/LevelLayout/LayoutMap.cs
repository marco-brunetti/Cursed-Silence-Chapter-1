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
    public List<LayoutDecorator> decorators;
    public List<LayoutItems> layoutItems;
    public List<LayoutType> nextShapes;
}

public record LayoutDecorator
{
	public int id { get; set; }
}

public record LayoutItems
{
	public int id { get; set; }
}