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
        Name : string
        Description : string
    }

// -------------------------------------
// "Database" - a simple, static list of 
// recipes will suffice for now.
// -------------------------------------

let recipes : Recipe list =
    [
        { Name = "Beans on Toast"; Description = "old favourite" }
        { Name = "Curry"; Description = "any sort will do" }
    ]

// -------------------------------------
// Views
// -------------------------------------

///Master page template: takes care of overall structure, styling, etc.
let template (title : string) (content : XmlNode list) =
    Elem.html [ Attr.lang "en"] [
        Elem.head [] [
            Elem.title [] [ Text.raw title ]
            Elem.link [
                Attr.href "https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css"
                Attr.rel "stylesheet"
            ]
        ]
        Elem.body [] content
    ]


let recipeListItemView (recipe: Recipe) =
    Elem.li [] [ Text.raw recipe.Name ]

let recipesListView (recipes : Recipe list) =
    let listItems = List.map recipeListItemView recipes
    let firstRecipe = List.head recipes
 
    template "Recipe list" [
        Elem.h1 [] [Text.raw "Available recipes"]
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