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
    public LayoutStyle style;
    public List<LayoutDecorator> decorators;
    public List<LayoutItems> layoutItems;
    public List<LayoutShape> nextShapes;
}

public record LayoutDecorator
{
    int id;
}

public record LayoutItems
{
    int id;
}