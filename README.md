# Voicepack Packer
Quick and dirty program to generate voicepacks for NASB.

## Usage
Simply drag and drop a properly formatted folder onto the EXE, and it will do the rest!

![alt text](https://github.com/DeadlyKitten/NickVoicepackPacker/blob/master/Resources/demo.gif?raw=true)

## Format
Setting up a folder to create a voicepack is pretty easy, just do the following:
- create a folder with a name that matches the id of the character (or object) you wish to create a voicepack for.
- add audio files to the folder.
  - files should be in ogg, mp3, or wav format.
  - the name of the file should correspond with the id of the action you'd like to trigger the sound.
- add folders to have an action randomly choose between them.
  - like loose files, folders should be named with the id of the action you'd like to trigger the sound.
  - names of audio files inside these folders doesn't matter.

Folder structure
```
CharacterId (root folder)
  ↳ MoveId1.wav
    MoveId2.mp3
    MoveId3 (folder)
      ↳ clip1.ogg
        clip2.ogg
        clip3.wav 
```
  
------

  ### Examples
  
  Folders used to contain multiple sounds per action:
  
  ![alt text](https://github.com/DeadlyKitten/NickVoicepackPacker/blob/master/Resources/screenshot_contents_1.png?raw=true)
  
  Multiple audio files inside of a folder:
  
  ![alt text](https://github.com/DeadlyKitten/NickVoicepackPacker/blob/master/Resources/Screenshot_contents_2.png?raw=true)
  
  -----
  
  #### Advanced Editing
  
  Nick Voicepack Packer generates a `.voicepack` file, which is simply a `.zip` file renamed to prevent end users extracting voice packs and installing them incorrectly.

  If you'd like to manually edit anything inside of the packed file, a `.zip` file also gets generated.

  **PLEASE NOTE:** If you end up using the `.zip` to manually edit your voice pack, please rename it to `.voicepack` once you're done. This makes installation significantly more clear for the end user.


  This program generates the bare minimum of what the mod requires to function. If you have a basic knowledge of the JSON format, you can further customize your voicepack by making edits to the package.json in the output zip.
  In particular, there are 2 fields you can adjust as of this writing:
  - "weight": This property is attached to clips that are a part of a group, and control how often each clip plays. The values are all relative to each other.
    - Ex. Making them all the same will cause them to play equally as often, but making one `1` and another `0.5` will cause the latter to play half as often.
  - "volume": Does the obvious. Default value is 1, and it is multiplied by the sfx and master volume during gameplay.
