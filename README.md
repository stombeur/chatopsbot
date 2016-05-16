# chatopsbot

Microsoft Bot Framework based bot that can queue VSTS builds from Slack or any other Microsoft Bot Connector based channel.

Clone and add Web.Secret.config with your bot appid and secret, and the url and PAT (token) to your VSTS instance.

Follow the steps in [Microsoft Bot Connector - Getting Started](http://docs.botframework.com/connector/getstarted/#navtitle) to create a bot instance in Azure and connect it to e.g. Slack.

Currently supported commands for the bot:

* help - list the available commands
* whoami - some info on you in the current channel (for debugging)
* state - some settings that the bot remembers for you during your conversation (currently only the selected project id)
* list projects - list all the VSTS projects you can access
* set project projectid - select that project to work with
* list builds (projectid) - list all available builds in that project
* list builds - list all available builds in the project in state
* queue build (buildid) - queue a build in the current project
* queue build (buildid) (projectid) - queue q build in that project

Todos:

* Connect to luis to make the bot more chatty and less botty
* use build and project names instead of ids
* implement moar commands