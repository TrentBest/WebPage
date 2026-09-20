namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>
/// Diegetic AEC Remote Office presentation. The same semantic objects drive the
/// crude plan, clickability, and later elevation representation.
/// </summary>
public static class SpatialAECRemoteOfficeGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpatialBuildingProgram program,
        SpatialModelSourcingManifest sourcing,
        SpatialOntologyInterrogation interrogation,
        SpatialViewpoint viewpoint,
        string userName,
        Action<string> focus,
        Action returnToPlan,
        Action<double> scaleAll,
        Action<string, double, double> resizeRoom,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "radial-gradient(circle at 50% 35%,#132b36 0,#03070d 52%,#010204 100%)")
            .Style("color", "#fff")
            .Style("font-family", "Consolas,'Courier New',monospace");

        root.Content(Header(receiver, program, sourcing, userName));

        if (viewpoint.Mode == SpatialPresentationMode.Elevation && viewpoint.FocusId is not null)
            root.Content(Elevation(receiver, program, viewpoint, focus, returnToPlan));
        else
            root.Content(Plan(receiver, program, sourcing, interrogation, viewpoint, focus));

        root.Content(ProgramPanel(receiver, program, scaleAll, resizeRoom));
        root.Content(WorkshopGui.Button(receiver).Label("← EXIT AEC OFFICE")
            .Style("position","fixed").Style("right","1rem").Style("bottom","1rem").Style("z-index","50")
            .Style("padding",".55rem .8rem").Style("border","1px solid #ff38d166")
            .Style("background","rgba(1,4,10,.92)").Style("color","#ff38d1")
            .Style("font-family","inherit").Style("font-size",".45rem")
            .Style("letter-spacing",".1em").Style("cursor","pointer").OnClick(exit));

        return root;
    }

    private static ElementBuilder Header(object receiver, SpatialBuildingProgram program, SpatialModelSourcingManifest sourcing, string userName)
        => WorkshopGui.Element(receiver,"div")
            .Style("position","fixed").Style("left","1rem").Style("top","1rem").Style("z-index","40")
            .Style("padding",".65rem .8rem").Style("border","1px solid #00eaff66")
            .Style("background","rgba(1,4,10,.9)")
            .Content(WorkshopGui.Element(receiver,"div").Style("color","#00eaff").Style("font-size",".58rem").Style("letter-spacing",".18em").Text("AEC REMOTE OFFICE"))
            .Content(WorkshopGui.Element(receiver,"div").Style("margin-top",".35rem").Style("color","#ffd34d").Style("font-size",".42rem").Style("letter-spacing",".12em").Text($"{program.Name.ToUpperInvariant()} // {program.RoomCount} ROOMS"))
            .Content(WorkshopGui.Element(receiver,"div").Style("margin-top",".35rem").Style("color","#aebfc8").Style("font-family","system-ui,sans-serif").Style("font-size",".65rem").Text($"VISITOR // {userName}"))
            .Content(WorkshopGui.Element(receiver,"div").Style("margin-top",".4rem").Style("color","#52e05a").Style("font-size",".36rem").Style("letter-spacing",".08em").Text($"{sourcing.Candidates.Count(x => x.Viable)} MODEL CANDIDATES READY // CREW {sourcing.Scouts.Count}"));

    private static ElementBuilder Plan(object receiver, SpatialBuildingProgram program, SpatialModelSourcingManifest sourcing, SpatialOntologyInterrogation interrogation, SpatialViewpoint viewpoint, Action<string> focus)
    {
        var svg = WorkshopGui.Element(receiver,"svg")
            .Attribute("viewBox","0 0 120 80").Attribute("preserveAspectRatio","xMidYMid meet")
            .Style("position","absolute").Style("inset","0").Style("width","100%").Style("height","100%")
            .Style("background","#01050a");

        svg.Child(WorkshopGui.Element(receiver,"rect").Attribute("x","12").Attribute("y","10").Attribute("width","96").Attribute("height","60")
            .Attribute("fill","#07131a").Attribute("stroke","#00eaff").Attribute("stroke-width",".5"));

        // The entrance is intentionally adjacent to reception.
        svg.Child(WorkshopGui.Element(receiver,"path").Attribute("d","M55 70 L65 70 L65 60")
            .Attribute("fill","none").Attribute("stroke","#ffd34d").Attribute("stroke-width",".8"));
        svg.Child(WorkshopGui.Element(receiver,"rect").Attribute("x","16").Attribute("y","14").Attribute("width","20").Attribute("height","9")
            .Attribute("fill","#00eaff").Attribute("fill-opacity",".08").Attribute("stroke","#00eaff").Attribute("stroke-width",".35"));
        svg.Child(Label(receiver,18,20,"RECEPTION"));

        svg.Child(Room(receiver,40,14,34,20,"CONFERENCE","conference-table",focus));
        svg.Child(WorkshopGui.Element(receiver,"rect").Attribute("x","47").Attribute("y","20").Attribute("width","20").Attribute("height","7")
            .Attribute("rx","1").Attribute("fill","#ffd34d10").Attribute("stroke","#ffd34d").Attribute("stroke-width",".35")
            .Style("cursor","pointer").OnClick(()=>focus("conference-table")));
        svg.Child(Label(receiver,49,24,"ONTOLOGY TABLE"));
        svg.Child(WorkshopGui.Element(receiver,"text").Attribute("x","49").Attribute("y","27").Attribute("fill","#52616a").Attribute("font-size","1.5")
            .Text($"{interrogation.TablePresentation.Count} ONTOLOGY LAYERS // {sourcing.Candidates.Count(x => x.Viable)} MODELS READY"));

        svg.Child(Room(receiver,77,14,25,16,"DIRECTOR","director-desk",focus));
        svg.Child(Room(receiver,16,27,20,15,"MODEL LIBRARY","trophy-case",focus));
        svg.Child(Room(receiver,40,37,28,25,"OPEN OFFICE","chair",focus));
        svg.Child(Room(receiver,71,37,30,25,"COLLABORATION","chair-2",focus));

        // Visitor marker remains the camera anchor.
        svg.Child(WorkshopGui.Element(receiver,"circle").Attribute("cx","60").Attribute("cy","35").Attribute("r","1.2")
            .Attribute("fill","#fff").Attribute("stroke","#ff38d1").Attribute("stroke-width",".35")
            .Style("filter","drop-shadow(0 0 4px #ff38d1)").Style("pointer-events","none"));

        return svg;
    }

    private static ElementBuilder Room(object receiver,double x,double y,double w,double h,string label,string focusId,Action<string> focus)
        => WorkshopGui.Element(receiver,"g")
            .Content(WorkshopGui.Element(receiver,"rect").Attribute("x",x).Attribute("y",y).Attribute("width",w).Attribute("height",h)
                .Attribute("fill","#07131a").Attribute("stroke","#00eaff").Attribute("stroke-opacity",".55").Attribute("stroke-width",".35")
                .Style("cursor","pointer").OnClick(()=>focus(focusId)))
            .Content(Label(receiver,x+1,y+5,label))
            .Content(WorkshopGui.Element(receiver,"text").Attribute("x",x+1).Attribute("y",y+h-2).Attribute("fill","#52616a").Attribute("font-size","2")
                .Text("CLICK TO INSPECT"));

    private static ElementBuilder Label(object receiver,double x,double y,string text)
        => WorkshopGui.Element(receiver,"text").Attribute("x",x).Attribute("y",y)
            .Attribute("fill","#ffd34d").Attribute("font-size","2.4").Attribute("letter-spacing",".4").Text(text);

    private static ElementBuilder Elevation(object receiver, SpatialBuildingProgram program, SpatialViewpoint viewpoint, Action<string> focus, Action returnToPlan)
    {
        var subject = viewpoint.FocusId!;
        if (subject == "conference-table")
        {
            return ConferenceElevation(receiver, interrogation, sourcing, returnToPlan);
        }

        var room = program.ResolveRoom(subject switch
        {
            "conference-table" => "conference",
            "chair" => "open-office",
            "chair-2" => "collaboration",
            "trophy-case" => "model-library",
            "reception-desk" => "reception",
            "director-desk" => "director",
            _ => "conference"
        });

        var svg = WorkshopGui.Element(receiver,"svg")
            .Attribute("viewBox","0 0 120 80").Attribute("preserveAspectRatio","xMidYMid meet")
            .Style("position","absolute").Style("inset","0").Style("width","100%").Style("height","100%")
            .Style("background","linear-gradient(180deg,#0b1b25,#02050a)");

        svg.Child(WorkshopGui.Element(receiver,"path").Attribute("d","M5 66 L115 66").Attribute("stroke","#00eaff").Attribute("stroke-width",".5"));
        svg.Child(WorkshopGui.Element(receiver,"path").Attribute("d","M12 66 L20 26 L100 26 L108 66").Attribute("fill","none").Attribute("stroke","#00eaff").Attribute("stroke-opacity",".5").Attribute("stroke-width",".4"));

        if (subject.Contains("chair",StringComparison.OrdinalIgnoreCase))
        {
            svg.Child(WorkshopGui.Element(receiver,"rect").Attribute("x","53").Attribute("y","44").Attribute("width","14").Attribute("height","16")
                .Attribute("fill","#ff38d112").Attribute("stroke","#ff38d1").Attribute("stroke-width",".55"));
            svg.Child(WorkshopGui.Element(receiver,"path").Attribute("d","M56 60 L56 66 M64 60 L64 66").Attribute("stroke","#ff38d1").Attribute("stroke-width",".55"));
            svg.Child(WorkshopGui.Element(receiver,"text").Attribute("x","60").Attribute("y","18").Attribute("text-anchor","middle").Attribute("fill","#ffd34d").Attribute("font-size","3")
                .Text("SEAT ELEVATION"));
        }
        else if (subject == "trophy-case")
        {
            svg.Child(WorkshopGui.Element(receiver,"rect").Attribute("x","38").Attribute("y","30").Attribute("width","44").Attribute("height","27")
                .Attribute("fill","#00eaff08").Attribute("stroke","#00eaff").Attribute("stroke-width",".55"));
            for(var x=44;x<=76;x+=8)
                svg.Child(WorkshopGui.Element(receiver,"rect").Attribute("x",x.ToString("0.##")).Attribute("y","37").Attribute("width","4").Attribute("height","10")
                    .Attribute("fill","#ffd34d12").Attribute("stroke","#ffd34d").Attribute("stroke-width",".3"));
            svg.Child(WorkshopGui.Element(receiver,"text").Attribute("x","60").Attribute("y","18").Attribute("text-anchor","middle").Attribute("fill","#ffd34d").Attribute("font-size","3")
                .Text("WALL DETAIL ELEVATION"));
        }
        else
        {
            svg.Child(WorkshopGui.Element(receiver,"rect").Attribute("x","35").Attribute("y","44").Attribute("width","50").Attribute("height","13")
                .Attribute("fill","#00eaff0a").Attribute("stroke","#00eaff").Attribute("stroke-width",".55"));
            svg.Child(WorkshopGui.Element(receiver,"rect").Attribute("x","51").Attribute("y","39").Attribute("width","18").Attribute("height","5")
                .Attribute("fill","#ffd34d0c").Attribute("stroke","#ffd34d").Attribute("stroke-width",".45"));
            svg.Child(WorkshopGui.Element(receiver,"text").Attribute("x","60").Attribute("y","18").Attribute("text-anchor","middle").Attribute("fill","#ffd34d").Attribute("font-size","3")
                .Text("OBJECT ELEVATION"));
        }

        svg.Child(WorkshopGui.Element(receiver,"text").Attribute("x","60").Attribute("y","24").Attribute("text-anchor","middle").Attribute("fill","#fff").Attribute("font-size","2.5")
            .Text($"{room.Name.ToUpperInvariant()} // {room.WidthFeet:0.#}' x {room.DepthFeet:0.#}' x {room.HeightFeet:0.#}'"));

        svg.Child(WorkshopGui.Element(receiver,"circle").Attribute("cx","60").Attribute("cy","62").Attribute("r","1.1")
            .Attribute("fill","#fff").Attribute("stroke","#00eaff").Attribute("stroke-width",".35"));

        return WorkshopGui.MultiPanel(receiver)
            .Style("position","absolute").Style("inset","0")
            .Content(svg)
            .Content(WorkshopGui.Button(receiver).Label("← RETURN TO PLAN")
                .Style("position","fixed").Style("left","1rem").Style("bottom","1rem").Style("z-index","50")
                .Style("padding",".45rem .7rem").Style("border","1px solid #00eaff66")
                .Style("background","rgba(1,4,10,.92)").Style("color","#00eaff")
                .Style("font-family","inherit").Style("font-size",".42rem").Style("cursor","pointer").OnClick(returnToPlan))
            .Content(WorkshopGui.Button(receiver).Label("INSPECT / INTERACT")
                .Style("position","fixed").Style("right","1rem").Style("bottom","1rem").Style("z-index","50")
                .Style("padding",".45rem .7rem").Style("border","1px solid #ffd34d66")
                .Style("background","rgba(1,4,10,.92)").Style("color","#ffd34d")
                .Style("font-family","inherit").Style("font-size",".42rem").Style("cursor","pointer").OnClick(()=>focus(subject)));
    }

    private static ElementBuilder ConferenceElevation(object receiver, SpatialOntologyInterrogation interrogation, SpatialModelSourcingManifest sourcing, Action returnToPlan)
    {
        var panel = WorkshopGui.MultiPanel(receiver)
            .Style("position","absolute").Style("inset","0")
            .Content(WorkshopGui.Element(receiver,"div")
                .Style("position","absolute").Style("left","50%").Style("top","52%")
                .Style("transform","translate(-50%,-50%)").Style("width","min(720px,70vw)")
                .Style("height","min(420px,48vh)").Style("border","1px solid #00eaff88")
                .Style("background","linear-gradient(180deg,#0b1b25,#02050a)")
                .Content(WorkshopGui.Element(receiver,"div").Style("position","absolute").Style("left","8%").Style("right","8%").Style("top","18%").Style("height","64%")
                    .Style("border","1px solid #ffd34d66").Style("background","rgba(255,211,77,.03)"))
                .Content(WorkshopGui.Element(receiver,"div").Style("position","absolute").Style("left","10%").Style("top","23%").Style("color","#ffd34d").Style("font-size",".45rem").Style("letter-spacing",".14em").Text("CONFERENCE TABLE // ONTOLOGY INTERROGATION"))
                .Content(WorkshopGui.Element(receiver,"div").Style("position","absolute").Style("left","10%").Style("right","10%").Style("top","34%").Style("display","grid").Style("grid-template-columns","repeat(3,1fr)").Style("gap",".3rem")
                    .Content(WorkshopGui.Element(receiver,"div").Style("grid-column","1 / -1").Style("color","#00eaff").Style("font-size",".38rem").Text("DEFAULT ONTOLOGY // PHYSICALIZED ON THE TABLE")))
                .Content(OntologyTable(receiver, interrogation))
                .Content(ModelCrew(receiver, sourcing))
                .Content(WorkshopGui.Element(receiver,"div").Style("position","absolute").Style("left","50%").Style("bottom","6%").Style("transform","translateX(-50%)").Style("color","#fff").Style("font-size",".38rem").Style("letter-spacing",".08em").Text("ASK QUESTIONS // REFINE MEANING // THEN GENERATE GEOMETRY")))
            .Content(WorkshopGui.Button(receiver).Label("← RETURN TO FLOOR PLAN")
                .Style("position","fixed").Style("left","1rem").Style("bottom","1rem").Style("z-index","50")
                .Style("padding",".45rem .7rem").Style("border","1px solid #00eaff66")
                .Style("background","rgba(1,4,10,.92)").Style("color","#00eaff")
                .Style("font-family","inherit").Style("font-size",".42rem").Style("cursor","pointer").OnClick(returnToPlan));
        return panel;
    }

    private static ElementBuilder OntologyTable(object receiver, SpatialOntologyInterrogation interrogation)
    {
        var panel = WorkshopGui.Element(receiver,"div")
            .Style("position","absolute").Style("left","10%").Style("right","10%").Style("top","41%")
            .Style("display","grid").Style("grid-template-columns","repeat(3,1fr)").Style("gap",".25rem");

        foreach (var layer in interrogation.TablePresentation)
            panel.Content(WorkshopGui.Element(receiver,"div").Style("padding",".28rem").Style("border","1px solid #00eaff33")
                .Style("background","rgba(0,234,255,.025)")
                .Content(WorkshopGui.Element(receiver,"div").Style("color","#00eaff").Style("font-size",".31rem").Text($"L{layer.Index} // {layer.Name}"))
                .Content(WorkshopGui.Element(receiver,"div").Style("color","#ffd34d").Style("font-size",".42rem").Text(layer.Value.ToString())));
        return panel;
    }

    private static ElementBuilder ModelCrew(object receiver, SpatialModelSourcingManifest sourcing)
    {
        var panel = WorkshopGui.Element(receiver,"div")
            .Style("position","absolute").Style("left","10%").Style("right","10%").Style("bottom","18%")
            .Style("display","flex").Style("gap",".3rem").Style("flex-wrap","wrap");

        foreach (var scout in sourcing.Scouts)
            panel.Content(WorkshopGui.Element(receiver,"div").Style("padding",".3rem .4rem").Style("border","1px solid #52e05a33")
                .Style("color","#52e05a").Style("font-size",".3rem").Text($"{scout.Name} // {scout.Specialty}"));
        return panel;
    }

    private static ElementBuilder ProgramPanel(object receiver, SpatialBuildingProgram program, Action<double> scaleAll, Action<string,double,double> resizeRoom)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position","fixed").Style("right","1rem").Style("top","1rem").Style("z-index","45")
            .Style("width","min(330px,28vw)").Style("max-height","76vh").Style("overflow-y","auto")
            .Style("padding",".7rem").Style("border","1px solid #ffd34d44")
            .Style("background","rgba(1,4,10,.9)");

        panel.Content(WorkshopGui.Element(receiver,"div").Style("color","#ffd34d").Style("font-size",".42rem").Style("letter-spacing",".12em").Text("BUILDING PROGRAM"));
        panel.Content(WorkshopGui.Element(receiver,"div").Style("margin-top",".35rem").Style("color","#aebfc8").Style("font-size",".36rem").Text($"ROOMS // {program.RoomCount} // ROOM AREA // {program.FinishedRoomAreaSquareFeet:0} SF"));

        foreach(var room in program.Rooms)
        {
            panel.Content(WorkshopGui.Element(receiver,"div").Style("margin-top",".45rem").Style("padding",".35rem").Style("border","1px solid #00eaff22")
                .Content(WorkshopGui.Element(receiver,"div").Style("color","#00eaff").Style("font-size",".36rem").Text($"{room.Quantity} × {room.Name}"))
                .Content(WorkshopGui.Element(receiver,"div").Style("color","#fff").Style("font-size",".34rem").Text($"{room.WidthFeet:0.#}' × {room.DepthFeet:0.#}' × {room.HeightFeet:0.#}'")));
        }

        foreach(var factor in new[]{.9,1d,1.1})
            panel.Content(WorkshopGui.Button(receiver).Label($"RESIZE ALL // {factor:0.0}X")
                .Style("margin-top",".25rem").Style("padding",".3rem .4rem").Style("border","1px solid #ffd34d44")
                .Style("background","rgba(255,211,77,.04)").Style("color","#ffd34d").Style("font-family","inherit").Style("font-size",".34rem").Style("cursor","pointer")
                .OnClick(()=>scaleAll(factor)));

        return panel;
    }
}
