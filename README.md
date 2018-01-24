# RLVR: Soccar in full 3D virtual reality

### What‽
[Rocket League](https://www.rocketleague.com/) mixes cars and soccer but is currently only played on a normal 2D screen. RLVR is an experiment to bring Rocket League to its proper platform, full 3D virtual reality! Be an egg, and drive your rocket-boosted car around the map, seeing the environment live on a VR headset.

### How?
Not gonna lie, this requires a bit of setup... You'll need Rocket League, python, Unity 3D, the Google Cardboard Unity SDK, plus a cardboard-style viewer (e.g. Google Cardboard, Utopia 360, etc... there are [lots](https://www.amazon.com/Cell-Phone-VR-Headsets/b?node=14775002011)) and an Android phone to put in it.

##### 1) Getting the server working
a) Install Rocket League (surprise...)
b) See [drssoccer55's original RLBot](https://github.com/drssoccer55/RLBot) for details on how to get the RL Bot working.
After checking out this repo locally, you'll need to launch RL, then patch the .dll with RLBot_Injector.
c) run `python runner.py` to launch RL bot into a game. The config edits here will run a match with three cars:
- you
- `botserver.py` opponent which will send game data to local port 3451 
- a normal all-star bot opponent 

At this stage you can verify it works by also running `python testClient.py` - that will print out game state very quickly to console.

##### 2) Get the client working
a) Install the [Unity 3D](https://unity3d.com/unity) app; the personal version should be fine, and free!
b) Install the [Google VR Unity packages](https://developers.google.com/vr/develop/unity/get-started) - they are used in the app, but not included in this repository.
d) Select GameState, and change the local IP property to be that of the computer your server is running on (try checking `ipconfig`, it might look something like `192.168.x.y`)
e) If your server from part 1 is working, you can hit play in Unity and it should connect and start drawing the game inside the Unity UI.

Assuming the above works, you can install it on your phone:
f) Attach your phone to your computer, and turn on USB debugging.
g) File -> Build & Run will compile it to an .apk and run it on your phone.
h) Put your phone in the headset, and enjoy! 

Once fully working, you should be able to still control your car as normal on server machine running Rocket League. You'll still see the game there, but it'll also be rendered to the headset.

##### Notes
Note that if the server or client run into problems, everything will probably break. You can only connect one client at a time, and stopping it will require restarting the server too; so the eventual pattern will be:
* Run Rocket League
* Run RLBot_Injector to inject bot code
* Run `python runner.py` to start the server
* Run the RLVR app on your phone to connect and display
* Play RL on the server machine, but watch in VR :)


##### Future work
Not much effort has been put into making the game world look nice in VR. Some things I'll add if I get the time, or up for grabs if people want to beat me to it:
* Add basic HUD: Boost counter, game timer and score
* Use proper 3D models for the field, ball and cars.
* Switch to UDP rather than TCP sockets.
* Fix bugs: 3D rotation doesn't work well on the walls
* Get the server bot to spectate, or worst case, drive inside the goals.


If you're interested in playing around with the code to get your own stuff working, take a look at `botServer.py` on the sending side and `listener.cs` on the receiving side. So long as those two stay in sync, you can send any game state you like from the bot to the Unity app, and use it to update the VR game world.
