# MyDog
This is a Unity Game using CMU PocketSphinx Android Plugin
Currently it only supports Android build

1. To build English version:
in SpeechRecognizerDemo.cs enable #define USE_ENGLISH and disable USE_CHINESE
rename folder sync_en to sync and put it under StreamingAssets folder

2. To build Chinese version:
in SpeechRecognizerDemo.cs enable #define USE_CHINESE and disable USE_ENGLISH
rename folder sync_chinese to sync and put it under StreamingAssets folder
