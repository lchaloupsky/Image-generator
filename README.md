# Image Generator

## Overview
Image Generator is a tool for generating synthetic images for natural language text descriptions in English. Principle of this tool is based on rules, relations and patterns between objects in text and not on the more widely used way with machine learning using GANs.

## Instalation
### Requirements
* .NET Framework 4.6.1 or Mono 4.6 and higher
* Internet connection
* Enough free space on disk (Dataset option)

### Download
```git
git clone https://github.com/lchaloupsky/Image-generator.git
```
### Run
Go to `Image Generator` directory:
* For Windows only open `.exe` file in `.\bin\Release\` directory
* For Mono use `mono ./bin/Image\ Generator.exe` command

## Usage
For both below described usages there are additional options for generating. These options can be turned in the left panel of the tool.
* You can use image captioning for finding better images
* Second option is the resolution change for the generated image

All downloaded images used for the generation process are stored in the `Images` folder.

### Normal usage
This usage shows result right in the application. Simply enter image description and wait until final image is generated. You can see actual state of generating above the description input.
 
After the process finishes, you can download generated image in the location you specify.

### Dataset usage
Second option is loading a dataset for which you want to generate images. Each description should be on separate line. 
First column should be identificator of the image. Supported file types are:
* `.txt`
* `.token`

After choosing a dataset you have to specify a location where the new images should be generated. In the generating phase is displayed actual state of how many images are already generated. The state is showed in progress bar with textual description under it.

### Tests
Tests are divided into multiple directories for each subapp and they can be found in all directories ending with *Tests*.

### Logs
If there are any exceptions in the generation process, then they are written into text files in the `Logs` directory.
