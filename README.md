KSPSaveEdit
===========

A Kerbal Space Program save editor

![Screenshot](http://cloud.github.com/downloads/wfraser/KSPSaveEdit/screenshot1.png "Screenshot")

About
-----

KSPSaveEdit lets you edit the ships in your Kerbal Space Program save files.
I created this mainly as a way to quickly get rid of all the "unknown" ship parts and debris floating around, but I figured, hey, while I'm at it, why not allow editing arbitrary attributes of the ships too?

It displays all the attributes of all the ships in your save.
Most of these you shouldn't edit, or you'll mess up the game!
When you hit Save, KSPSaveEdit will make a backup of your save called "persistent_backup.sfs" right next to the original, just in case.

To easily get rid of junk ships, click the "type" column header to sort by vessel type.
Then either ctrl-click or shift-click to select all the ships you want to get rid of.
Then click Delete Selected, followed by Save, and re-load KSP. The junk will be gone!

Compatibility
-------------

This is a WPF program, and will work on any version of Windows with .NET Framework 4.5, which means __Windows 8__ (except Windows RT), __Windows 7__, and __Windows Vista__.

It's been developed on Windows 8 using Visual Studio 2012.

License
-------

This program is copyright (c) 2012 William R. Fraser

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
