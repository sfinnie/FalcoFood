# Hello World with Falco

Hello world using the [Falco](https://www.falcoframework.com) F# framework.  Broadly comparable to [Saturn](https://saturnframework.org/) though newer and looks like a one-person effort.

## Setup

Was simple and error-free using the [Getting Started](https://www.falcoframework.com/docs/get-started.html) instructions.  Basic "Hello World" up and running after 4 commands.

# Generating Alternative response formats

The docs have [an entire section](https://www.falcoframework.com/docs/response.html) on writing various response formats.  

## Running

* Run server in hot reload mode:

        $ dotnet watch

  * Run server in "prod" mode:

        $ dotnet run 
  
## Notes on docs

1. Would be good if docs mentioned using `dotnet watch` in dev mode.
2. Response examples don't show how to add routing entries.  This is minor, and could be argued against since the routing section is listed before responses.
2. Possible error in redirect docs.  Submitted [issue #98](https://github.com/pimbrouwers/Falco/issues/98)


