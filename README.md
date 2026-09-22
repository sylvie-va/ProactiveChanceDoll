# Proactive Chance Dolls
No more foregoing Shrines of Chance because one person in your lobby has a Chance Doll. Nor will you be forced to buy out Shrines of Chance because you have the Chance Dolls.

# Functionality
When using a Shrine of Chance, a check is made to see if any players in the lobby have Chance Dolls and, if they do, Chance Dolls are temporarily placed into your inventory and then removed.

# Compatibility
- Chance Dolls are not touched, so any mods editing Chance Dolls should still work.
- Shrine of Chance events are not directly edited, but rather hooked onto and added to the stack of.
- Items are not dropped, but directly added and removed from the inventory of the player who is interacting with the Shrine of Chance. As such, item share mods should not break with this mod.

# Contact
sylvieqq on Discord.

# Building the project:

1. Check `~/ProactiveChanceDollProject/ProactiveChanceDoll.csproj` for build dependencies under game-libs & libs.

2. run `dotnet build ProactiveChanceDoll.slnx`
