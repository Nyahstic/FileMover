# File Mover 
This program will automatically copy files into folders depending on their extension, I made this as a personal project to help me copy files from the USB stick into a computer

# Compiling
You'll need .Net 7.0 to build this.
- If you have, just run `dotnet build` to make the project

# Configuration
The program will automactly create a file containing some example configuration, the format is JSON and needs only these things:

| Name   | Description                                                  | Example              |
|--------|--------------------------------------------------------------|----------------------|
| source | The path where the program will listen to filesystem changes | "C:\source\location" |
| target |      The path where the program will copy the files into     | "Z:\target\location" |
| \.ext  |          Specifies where that extention will end up          | "\place"             |
