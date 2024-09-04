# Lethal Constellations

### Separate your moons list by constellations. 
- This mod will dynamically change your moons list depending on a configurable constellations listing.

**NOTE:** This mod is still in early development. Please report any issues you find as you are essentially a beta tester.

### Current Features:
- If a moon is not inside the current constellation it will be hidden from the moons page and locked (using LLL)
- Contains two config files. A static config and a generated one.
	- Static config will dictate what is generated in the generated config.
	- Generated config is generated after first lobby load.
		- Currently the generated config cannot be modified using LethalConfig.
- Customizable keywords/text.
	- Dont like the word constellation? Change it in the config!
	- This mod will attempt to replace any pre-existing keywords from the config with radarbooster names as a fail safe.
		- Please avoid overwriting any base-game keywords or other mods' keywords.
		- If a terminal keyword already exists for any of the names you designate, they will be changed.
	- Customizable text for all different types of terminal commands relating to the constellations.
	- Optional shortcut keywords can also be added to each constellation.
- Current Constellation will be updated on lobby load, works between save files!
- Moon prices can also be modified by this mod or left to be handled elsewhere.
- Set routing to a specific constellation to cost credits or allow for your constellations to be free.
- Set a constellation to be a one-time-purchase, this purchase will be remembered for each save file.
	- In order to sync between players this feature requires [LethalNetworkAPI](https://thunderstore.io/c/lethal-company/p/xilophor/LethalNetworkAPI/) to be present.
- Routing to a new constellation will take you to that constellation's default moon.
	- This essentially makes the cost of routing to this moon however much it costs to route to your constellation.
- Set which constellations the company can be routed to in the constellations config
- Hide special constellations from the menu or if you cant afford it (with configuration options)
- Allow for a moon to remain hidden while being assigned to a constellation.
- This mod does not do any patching, all game patching is done by OpenLib & LethalLevelLoader.
- Compatibility with [LethalMoonUnlocks](https://thunderstore.io/c/lethal-company/p/explodingMods/LethalMoonUnlocks/)

### For other mod devs looking to add compatibility:
- Subscribe to the RouteConstellationSuccess event in NewEvents.cs to call your own code when routing to a new constellation.
- Access ConstellationStuff in Collections.cs for a listing of all constellations and their ClassMapper.cs properties

### If you have any ideas on how to make this mod better please feel free to reach out!

- [Icon background](https://chandra.harvard.edu/photo/2024/25th/more.html) 
	- Credit: X-ray: NASA/CXC/Ludwig Maximilian Univ./T. Preibisch et al.; Infrared: NASA/ESA/CSA/STScI; Image processing: NASA/CXC/SAO/N. Wolk