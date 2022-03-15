# Livesplit-Server-Autosplitter

## What is it?

This is a autosplitter solution for all of the Linux users, using the Livesplit Server component to communicate with Livesplit

This currently immitates the ASL functionnality outside of Livesplit and then do whatever it needs to do with the Server

You can make your autosplitters and add them to the Autosplitters directory and then change the code to actually use that one you made (see the ones already here for examples)

## How to use it?

Run the program with elevated privileges (so that it can read the memory from the game's process)

You will need the Livesplit Server (v1.8.17+) to be running on the default port

There is a CLI which allows you to modify the settings while it's running, type help when running the program for more info

This could be used with other timers by changing the code related to the socket  
This could also be way more polished and might be in the future but hey, it works! :p
