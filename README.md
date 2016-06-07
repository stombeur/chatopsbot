# chatopsbot

Microsoft Bot Framework based bot that can queue VSTS builds from Slack or any other Microsoft Bot Connector based channel.

Clone and add Web.Secret.config with your bot appid and secret, and the url and PAT (token) to your VSTS instance.

Follow the steps in [Microsoft Bot Connector - Getting Started](http://docs.botframework.com/connector/getstarted/#navtitle) to create a bot instance in Azure and connect it to e.g. Slack.

Currently supported commands for the bot:

* help - list the available commands
* help [command] - more info on [command]
* state - some settings that the bot remembers for you during your conversation and some info about your conversation
* set - change your user settings
* alias - Create an alias for another command. Run an aliased command. List all known aliases.
* project - List all available vsts projects.
* build - Start or Cancel a vsts build. List all available vsts builds.

####build
|switch|description|
|---|---|
|--id            |the build id or name |
|--start         |start a build (the default switch if none is specified)  |
|--list          |list all builds for a project  |
|--cancel        |cancel all builds  |
|--project       |the project id or name | 
|value pos. 0    |pass the build id or name as the first parameter  |

Examples: 

```
build --list
build 42
build --cancel 42
build --project abc --id 42
```

####project
|switch|description|
|---|---|
|--list |list available projects (the default switch if none is specified)|

Examples: 

```
project --list
project
```

####state
|switch|description|
|---|---|
| |show your state settings|
|--clear |clear all your state settings|

Examples: 

```
state
state --clear
```

####set
|switch|description|
|---|---|
|--tfsuser |set your tfs username to request builds on your behalf|
|--project|set your default tfs project|

Examples: 

```
set --tfsuser me@my.com
set --project abcdef
```


####alias
|switch|description|
|---|---|
|--name          |the name of the alias
|--command       |the aliased command
|--create        |create a new alias or update an existing one (this is the default switch if none is specified). Requires you to also specify a name and command
|--run           |run an aliased command. You can also run the alias by just passing the alias without 'alias --run'. The command 'alias --run <aliasName>' is equivalent with the command '<aliasName>'
|--list          |list all known aliases
|--clear         |clear all known aliases
|value pos. 0    |the aliased command can also be passed without the --command

Examples:

````
alias --name my-alias --command "project --list"
alias --run --name "my-alias"
my-alias
alias --list
````


Todos:

* ~~Connect to luis to make the bot more chatty and less botty~~ Tried it and the language is too specific for luis.
* use build and project names instead of ids
* implement moar commands
* ~~improve the command parsing~~ Now using the [Commandline](https://github.com/gsscoder/commandline) library
