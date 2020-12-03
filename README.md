# A set of ASP.NET Core Tag Helpers for [prismic.io](https://prismic.io)

## Getting started

First you will need to install the package.

```shell
    dotnet add package adaptivewebworks.prismic.aspnetcore.mvc
```

Once installed you will need to add the following to you `_ViewImports.cshtml`

```csharp
@addTagHelper *, AdaptiveWebworks.Prismic.AspNetCore.Mvc
```

## Supported Fragments
Each of the supported fragment tag helpers, allow you to work directly with a fragment or with a field in a document. If both fragments and document attributes are supplied the fragment will take priority over a field.

### Link Resolution
Some of the tag helpers expects a `DocumentLinkResolver` as a constructor dependency this can be setup in your service collection as follows.

```csharp
    services.AddSingleton<prismic.DocumentLinkResolver>(DocumentLinkResolver.For(link => "/"))
```

You can improve this by implementing your own class that extends `DocumentLinkResolver` and overriding any of the virtual methods.

### Structured text
We can make rendering of Rich Text much easier by using the structured text tag helper. You can use the tag helper with a fragment like this.

```html
<structured-text prismic-fragment="@Model.StructuredTextProperty" />
```

When working with a Prismic Document you can do the following.

```html
<structured-text prismic-document="@Model" prismic-field="document_type.field_name" />
```

### Link resolver tag helper
We can make rendering of document links less cumbersome by using the link resolver tag helper. You can use the tag helper with a `ILink` fragmen like this.

```html
<a prismic-href="@Model.Link" >A link to a document</a>
```

When working with a prismic document and field you can create a link as follows.

```html
<a prismic-document="@Model" prismic-field="document_type.field_name">A link to a document</a>
```

## Creating your own custom fragment tag helpers
The package provides an abstract class `PrismicFragmentTagHelper<TFragment>` that can be inherited from to accomodate your custom requirements, while retaining the fragment and document field.

You will need to register you namespace in your `_ViewImports.cshtml` like in the getting started section.

```csharp
@addTagHelper *, Your.Project.Namespace
```

## Samples
TO DO.