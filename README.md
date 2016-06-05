# chatopsbot

Microsoft Bot Framework based bot that can queue VSTS builds from Slack or any other Microsoft Bot Connector based channel.

Clone and add Web.Secret.config with your bot appid and secret, and the url and PAT (token) to your VSTS instance.

Follow the steps in [Microsoft Bot Connector - Getting Started](http://docs.botframework.com/connector/getstarted/#navtitle) to create a bot instance in Azure and connect it to e.g. Slack.

Currently supported commands for the bot:

* help - list the available commands
* help command - more info on the command
*
* state - some settings that the bot remembers for you during your conversation and some info about your conversation
* alias - Create an alias for another command. Run an aliased command. List all known aliases.
* project - List all available vsts projects. Choose a default project.
* build - Start or Cancel a vsts build. List all available vsts builds.

Todos:

* ~~Connect to luis to make the bot more chatty and less botty~~ Tried it and the language is too specific for luis.
* use build and project names instead of ids
* implement moar commands
* ~~improve the command parsing~~ Now using the [Commandline](https://github.com/gsscoder/commandline) library