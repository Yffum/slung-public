# Introduction
Slung is an open source casual arcade game for phones and tablets. It is also my first published game! I developed Slung to learn about the experience of developing a game simultaneously on iOS and Android.

# Download Links
Update (2025-06-18): Slung is not currently available on the iOS App Store, because I'm not working on mobile development right now and my Apple dev account has lapsed.

~iOS App Store:
https://apps.apple.com/us/app/slung/id6465792615~

Google Play Store:
https://play.google.com/store/apps/details?id=com.Arsom.Slung

![slung title screen](https://i.imgur.com/pcCJ3Vy.png)

# Description
Slung is a fast-paced casual slingshot game! Pop the bubbles before they touch the ground!

### Features:
- Intuitive gameplay for anyone
- Difficult to master
- Satisfying synth sound effects
- Pastel rainbows

# Dependencies
This project requires Unity 2022.3.7f1, and might not work with later versions.

For saving highscores locally, I relied on the [Easy Save Unity package by Moodkie](https://assetstore.unity.com/packages/tools/utilities/easy-save-the-complete-save-data-serializer-system-768) to serialize data. Due to licensing, this plugin is not included in this repo. If you want saves to work, you can either purchase a license, or re-implement the save/load functions (see [Save Data](#save-data)).

This repo relies on Git LFS (which is installed automatically with GitHub Desktop).

# Usage
After cloning the repo, simply open "Slung.sln", or add the cloned repo directory to Unity Hub.

Go to File -> Open Scene,  and then select MainScene.Unity

### Save Data
Saving is not implemented in this public repo due to licensing. If you have purchased [a license for Easy Save](https://assetstore.unity.com/packages/tools/utilities/easy-save-the-complete-save-data-serializer-system-768), you can add the plugin to the project and then go to Controllers/GameController.cs, and uncomment the body of the SaveUserData() and LoadUserData() functions. This will enable local highscore saves. Alternatively, you can implement your own saving/loading process using these functions. The class which contains the data to be serialized and saved/loaded is Tools/UserData.cs

### Platforms
The main branch is for iOS, and the Android branch is for Android. The iOS branch has some optimizations not present in the Android branch (as seen in the commits), but there wasn't a detectable performance impact. Git doesn't save the Unity build target, so you have to select your platform in the build menu manually after switching branches.

# Support
For questions, contact me at arsomgames@gmail.com, or visit my [support page](https://sites.google.com/view/arsom/slung-support).

