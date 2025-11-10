#  Dual-grid System in Unity

#### This repo is a demo of my dual-grid tilemap implementation in Unity 2022.3.42f1c1.

I followed Jess’s YouTube tutorial (https://www.youtube.com/watch?v=jEWFSv3ivTg&t=233s
), which presents an efficient way to cut down the number of tiles. While the video clearly explains the overall idea, I also dug into the references Jess shared - [ThinMatrix’s video](https://youtu.be/buKQjkad2I0?si=9xot1uUw3PvNWvT9&t=234) and [Oskar Stålberg’s original dual-grid proposal](https://x.com/OskSta/status/1448248658865049605) to understand the details. Then I built my own implementation.

Highlights:
* Only six tiles for the dual-grid setup
* Adapted for my project’s needs
* Full learning notes and implementation details included


### Mechanism

Regular tile cut:
<img src="https://github.com/jess-hammer/dual-grid-tilemap-system-godot/assets/59108399/ac3c9ab6-b399-4142-8425-3de6d67249a0" width="350" title="Inward blob cut">

On-the-dual tile cut:
<img src="https://github.com/jess-hammer/dual-grid-tilemap-system-godot/assets/59108399/5399d1b6-7169-4ff8-8a17-1ba8e483fce3" width="350" title="Inward blob cut">


If you have any questions or ideas for improvements feel free to contact me at yia0223@gmail.com

