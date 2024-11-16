Aseprite v1.3.9.2-x64 | A pixel art program
Copyright (C) 2001-2024 Igara Studio S.A.

Usage:
  Aseprite.exe [OPTIONS] [FILES]...

Options:
      --shell                           Start an interactive console to execute scripts
  -b, --batch                           Do not start the UI
  -p, --preview                         Do not execute actions, just print what will be
                                        done
      --save-as <filename>              Save the last given sprite with other format
      --palette <filename>              Change the palette of the last given sprite
      --scale <factor>                  Resize all previously opened sprites
      --dithering-algorithm <algorithm> Dithering algorithm used in --color-mode
                                        to convert images from RGB to Indexed
                                          none
                                          ordered
                                          old
      --dithering-matrix <id>           Matrix used in ordered dithering algorithm
                                          bayer2x2
                                          bayer4x4
                                          bayer8x8
                                          filename.png
      --color-mode <mode>               Change color mode of all previously
                                        opened sprites:
                                          rgb
                                          grayscale
                                          indexed
      --shrink-to width,height          Shrink each sprite if it is
                                        larger than width or height
      --data <filename.json>            File to store the sprite sheet metadata
      --format <format>                 Format to export the data file
                                        (json-hash, json-array)
      --sheet <filename.png>            Image file to save the texture
      --sheet-type <type>               Algorithm to create the sprite sheet:
                                          horizontal
                                          vertical
                                          rows
                                          columns
                                          packed
      --sheet-pack                      Same as -sheet-type packed
      --sheet-width <pixels>            Sprite sheet width
      --sheet-height <pixels>           Sprite sheet height
      --sheet-columns <columns>         Fixed # of columns for -sheet-type rows
      --sheet-rows <rows>               Fixed # of rows for -sheet-type columns
      --split-layers                    Save each visible layer of sprites
                                        as separated images in the sheet
      --split-tags                      Save each tag as a separated file
      --split-slices                    Save each slice as a separated file
      --split-grid                      Save each grid tile as a separated file
      --layer <name> or
      --import-layer <name>             Include just the given layer in the sheet
                                        or save as operation
      --all-layers                      Make all layers visible
                                        By default hidden layers will be ignored
      --ignore-layer <name>             Exclude the given layer in the sheet
                                        or save as operation
      --tag <name> or
      --frame-tag <name>                Include tagged frames in the sheet
      --play-subtags                    Play subtags and repeats when saving the frames of an animated sprite
      --frame-range from,to             Only export frames in the [from,to] range
      --ignore-empty                    Do not export empty frames/cels
      --merge-duplicates                Merge all duplicate frames into one in the sprite sheet
      --border-padding <value>          Add padding on the texture borders
      --shape-padding <value>           Add padding between frames
      --inner-padding <value>           Add padding inside each frame
      --trim                            Trim whole sprite for --save-as
                                        or individual frames for --sheet
      --trim-sprite                     Trim the whole sprite (for --save-as and --sheet)
      --trim-by-grid                    Trim all images by its correspondent grid boundaries before exporting
      --extrude                         Extrude all images duplicating all edges one pixel
      --crop x,y,width,height           Crop all the images to the given rectangle
      --slice <name>                    Crop the sprite to the given slice area
      --filename-format <fmt>           Special format to generate filenames
      --tagname-format <fmt>            Special format to generate tagnames in JSON data
      --script <filename>               Execute a specific script
      --script-param name=value         Parameter for a script executed from the
                                        CLI that you can access with app.params
      --list-layers                     List layers of the next given sprite
                                        or include layers in JSON data
      --list-layer-hierarchy            List layers with groups of the next given sprite
                                        or include layers hierarchy in JSON data
      --list-tags                       List tags of the next given sprite
                                        or include frame tags in JSON data
      --list-slices                     List slices of the next given sprite
                                        or include slices in JSON data
      --oneframe                        Load just the first frame
      --export-tileset                  Export only tilesets from visible tilemap layers
  -v, --verbose                         Explain what is being done
      --debug                           Extreme verbose mode and
                                        copy log to desktop
      --noinapp                         Disable "in game" visibility on Steam
                                        Doesn't count playtime
      --disable-wintab                  Don't load wintab32.dll library
  -?, --help                            Display this help and exits
      --version                         Output version information and exit

Find more information in Aseprite web site: https://www.aseprite.org/

