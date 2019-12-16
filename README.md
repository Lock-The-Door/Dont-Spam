# Don't Spam (Prefix: $)
A discord bot that kindly reminds your discord server members to not spam!

## Commands
### Spam (Include group name in command usage)
The group of commands that manages the spam in your server
#### Start (channel)
Starts sending "dont spam!!" to the channel specified every:
- 1/10 secs,
- 6/min,
- 360/hr,
- or 8640/day

#### Stop
Stops sending "dont spam!!" to the channel that is being spammed

### Punish (Don't include group name in command usage)
#### Punish (User) (Time in seconds)
Starts sending "don't spam!!" to the user specified for the time specified every:
- 1/5 secs,
- 12/min,
- 720/hr,
- or 17280/day

#### Stack (User) (Time in seconds)
When the bot is already spamming the user specified, this command will keep spamming for the time specified on top of the time left that it is already counting down.

#### Override (User)
Stops sending "dont spam!!" to the user specified even if the time is not up and notify the user
