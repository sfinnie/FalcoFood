module FalcoFood.Program

open System.Xml
open Falco
open Falco.Routing
open Falco.HostBuilder
open Falco.Markup
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting.Internal

// -------------------------------------
// Types
// -------------------------------------

type Recipe =
    {
        Id : int
        Name : string
        Description : string
    }

// -------------------------------------
// "Database" - a simple, static list of 
// recipes will suffice for now.
// -------------------------------------

let recipes : Recipe list =
    [
        { Id = 1; Name = "Beans on Toast"; Description = "old favourite" }
        { Id = 2; Name = "Curry"; Description = "any sort will do" }
    ]

// ----------------------------------------------------------------
// Extensions for using htmx.  Note there's
// a Falco.Htmx package (https://github.com/dpraimeyuu/Falco.Htmx)
// though it's currently experimental.  It's also not available
// as a NuGet package, so installation is a bit more tricky.
// This app only uses a very small part of htmx, so it's
// easier to build custom.
// ----------------------------------------------------------------

module Attr =
    let hxGet = Attr.create "hx-get"
    let hxTarget = Attr.create "hx-target"
    let hxSwap = Attr.create "hx-swap"

// -------------------------------------
// Views
// -------------------------------------

///Master page template: takes care of overall structure, styling, etc.
let template (title : string) (content : XmlNode list) =
    Elem.html [ Attr.lang "en"] [
        Elem.head [] [
            Elem.title [] [ Text.raw title ]
            Elem.meta [ Attr.charset "utf-8" ]
            Elem.meta [
                Attr.name "viewport"
                Attr.content "width=device-width, initial-scale=1.0"
            ]
            Elem.link [
                Attr.href "https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"
                Attr.rel "stylesheet"
            ]
            Elem.script [
                Attr.src "https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"
            ] []
            Elem.script [
                Attr.src "https://unpkg.com/htmx.org@1.6.1"
            ] []
        ]
        Elem.body [] [
            // Encompassing page
            Elem.div [ Attr.class' "vh-100" ] [
                
                // Header panel
                Elem.div [ Attr.class' "container p-3 bg-primary text-white text-center" ] [
                    Elem.h1 [] [Text.raw "Falco Food"]
                    Elem.p [] [
                        Text.raw "A sample application using the "
                        Elem.a [ Attr.href "https://www.falcoframework.com"; Attr.class' "text-white" ] [
                            Text.raw "Falco web framework."
                        ]
                        Text.raw "  Also uses "
                        Elem.a [ Attr.href "https://getbootstrap.com/"; Attr.class' "text-white" ] [
                            Text.raw "Bootstrap"
                        ]
                        Text.raw " for styling, and "
                        Elem.a [ Attr.href "https://htmx.org/"; Attr.class' "text-white" ] [
                            Text.raw "htmx"
                        ]
                        Text.raw " for partial page updates."
                    ]
                ]
                
                // Main content panel
                Elem.div [ Attr.class' "container mt-5" ] [
                    Elem.div [ Attr.class' "row"; Attr.id "content-col" ] [
                        
                        //Recipe list panel
                        Elem.div [ Attr.class' "col-lg-5" ] [
                            Elem.div [ Attr.class' "card h-100" ] [
                                Elem.div [ Attr.class' "card-header" ] [
                                    Elem.h3 [] [ Text.raw "Available Recipes" ]
                                ]
                                Elem.div [ Attr.id "recipe-list"; Attr.class' "card-body" ] content
                            ]
                        ]
                        
                        //Recipe details panel
                        Elem.div [ Attr.class' "col-lg-7" ] [
                            Elem.div [ Attr.class' "card h-100" ] [
                                Elem.div [ Attr.class' "card-header" ] [
                                    Elem.h3 [] [ Text.raw "Recipe Details" ]
                                ]
                                Elem.div [ Attr.id "recipe-details"; Attr.class' "card-body" ] []
                            ]
                            
                        ]
                    ]
                ]
            ]
        ]
    ]

let recipeUri (recipe : Recipe) =
    (sprintf "/recipe/%d" recipe.Id)

let recipeListItemView (recipe: Recipe) =
    let uri = recipeUri recipe
    
    Elem.li [] [
        Elem.a [ Attr.href ""; Attr.hxGet uri; Attr.hxTarget "#recipe-details"; Attr.hxSwap "innerHTML" ] [
            Text.raw recipe.Name
        ]
    ]

let recipesListView (recipes : Recipe list) =
 
    template "Recipe list" [
        Elem.ul [] (List.map recipeListItemView recipes)
    ]

// -------------------------------------
// View Handler Functions
// -------------------------------------

/// GET /recipes
let listRecipes : HttpHandler =
    Response.ofHtml (recipesListView recipes)

// -------------------------------------
// Exception Handler
// -------------------------------------

let exceptionHandler : HttpHandler =
    Response.withStatusCode 500 
    >> Response.ofPlainText "Server error"

[<EntryPoint>]
let main args =   
    webHost args {
        
        use_static_files
        
        use_if    FalcoExtensions.IsDevelopment DeveloperExceptionPageExtensions.UseDeveloperExceptionPage
        use_ifnot FalcoExtensions.IsDevelopment (FalcoExtensions.UseFalcoExceptionHandler exceptionHandler)
        
        endpoints [
            get "/" (Response.redirectTemporarily "/recipes")
            get "/recipes" listRecipes
        ]
    }
    0 //exit code