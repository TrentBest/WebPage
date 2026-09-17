# Workshop GUI Builders

The Workshop GUI system is a recursive builder architecture, not a CSS abstraction.

```text
Platform-neutral builder
        |
        v
     GuiNode
        |
   +----+----------------+
   |    |                |
   v    v                v
 Web   WPF        Unity UI Toolkit
        |
        v
     Oculus / VR
```

## Core contract

`ICoreGuiBuilder<TProduct>` mirrors the useful part of the WPF builder contract:

- `Build()` manifests a builder.
- `GetBuilderId()` provides stable builder identity.
- `Parent` preserves recursive builder hierarchy.

The shared Workshop builder produces a platform-neutral `GuiNode` tree. It does not create a WPF `FrameworkElement`, a Blazor `RenderFragment`, or a Unity object.

## Recursive composition

A builder may contain another builder:

```csharp
var experience = WebGuiBuilder
    .Panel("experience")
    .Child("Panel", "header", header => header
        .Child("Text", "title", title => title.Text("The Singularity Workshop")))
    .Child("Button", "enter", button => button
        .Child("Text", "label", label => label.Text("Enter Workshop")));
```

The same semantic tree can then be manifested by a platform adapter.

## Web manifestation

`WebGuiBuilder` is the first concrete platform family.

`BlazorGuiRenderer` translates the resulting `GuiNode` tree into Blazor render instructions. This keeps the semantic builder separate from the web renderer.

Manual `RenderTreeBuilder` work is intentionally isolated to this adapter rather than spread through the application.

## Vocabulary

The initial manifest contains:

- Panel
- Stack
- Grid
- Button
- Image
- Text
- Warning

This vocabulary is deliberately small. More controls should be added when they represent a reusable semantic primitive, not merely a visual styling trick.

## Alignment with the WPF family

The WPF builders in `RevitFamilyManagerBuilders` establish several useful principles that this architecture is adopting:

1. Builders are compositional units.
2. Builders retain parent/child hierarchy.
3. A builder has a stable identity.
4. `Build()` is the manifestation boundary.
5. Layouts and controls are both builders.
6. Platform-specific construction belongs at the manifestation boundary.

The long-term target is:

```text
                 Workshop GUI Grammar
                         |
          +--------------+--------------+
          |              |              |
        Web             WPF        Unity UI Toolkit
          |              |              |
       Blazor          WPF/        Unity UI
                     Revit          Toolkit
                         |
                    desktop/VR
```

The GUI definition should not need to know which surface ultimately presents it.