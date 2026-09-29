using Microsoft.AspNetCore.Components;
namespace TheSingularityWorkshop.Workshop.Gui;
public sealed class WebGuiBuilder : ICoreGuiBuilder<RenderFragment>
{
    private readonly GuiBuilder _semanticBuilder;
    private readonly string _kind;
    private readonly string _id;
    private WebGuiBuilder(GuiBuilder semanticBuilder,string kind,string id,WebGuiBuilder? parent)
    { _semanticBuilder=semanticBuilder;_kind=kind;_id=id;Parent=parent; }
    public static WebGuiBuilder Create(string kind,string id)=>new(GuiBuilder.Create(kind,id),kind,id,null);
    public static WebGuiBuilder Panel(string id)=>Create("Panel",id);
    public static WebGuiBuilder Button(string id)=>Create("Button",id);
    public static WebGuiBuilder Stack(string id)=>Create("Stack",id);
    public static WebGuiBuilder Grid(string id)=>Create("Grid",id);
    public static WebGuiBuilder Text(string id,string text)=>Create("Text",id).Text(text);
    public static WebGuiBuilder Image(string id,string source)=>Create("Image",id).Image(source);
    public object? Parent{get;}
    public string Kind=>_kind;
    public string GetBuilderId()=>_id;
    public WebGuiBuilder Text(string text){_semanticBuilder.Text(text);return this;}
    public WebGuiBuilder Image(string source){_semanticBuilder.Image(source);return this;}
    public WebGuiBuilder Property(string name,string value){_semanticBuilder.Property(name,value);return this;}
    public WebGuiBuilder Class(string value)=>Property("class",value);
    public WebGuiBuilder Style(string name,string value)=>Property($"style:{name}",value);
    public WebGuiBuilder Width(string value)=>Property("layout:width",value);
    public WebGuiBuilder Height(string value)=>Property("layout:height",value);
    public WebGuiBuilder Command(string commandId)=>Property("command",commandId);
    public WebGuiBuilder Child(string kind,string id,Action<WebGuiBuilder>? configure=null)
    {
        _semanticBuilder.Child(kind,id,child=>configure?.Invoke(new WebGuiBuilder(child,kind,id,this)));
        return this;
    }
    public GuiNode BuildNode()=>_semanticBuilder.Build();
    public RenderFragment Build()=>BlazorGuiRenderer.Render(BuildNode());
}
