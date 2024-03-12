The logic followed by those scripts is:

AttrBar.cs: The script responsible for filling the status bar considering the values on the PlayerStats class;

DynamicAttrBar.cs: The script responsible for changing the size of the sprites of the status bar accoding the MAX stat value;

PlayerStats.cs: The script responsible for set the stats to be readen by the AttrBar class.

This is a demo of how to make the dynamic change on the size of the bars, the main purpose of the asset. The sprites are already edited and sliced in the proper way, if you want to implement that on your game you only need to change the playerStats scripts to the stat script you use for your character and read the values from there. About the animations you can add the animation component to the UI elements and for example do a animation named "Break" for the border or the sprite that is above the others and play it when the character losses all his HP.