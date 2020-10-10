# Video Tag Player

Inspired by Udemy's tag functionality for online videos, here is a version that can do the same thing for offline videos and audios. This application is completely free!

## Components

This app can be considered mostly a wrapper around [LibVLCSharp](https://code.videolan.org/videolan/LibVLCSharp/-/tree/3.x). Tag files are saved with " (Note).yaml" suffix along side video file.

## Feature

* Add/Modify/Remove tags add play points in a video/audio

## ToDo

* Add keyboard navigation (skip forward a few seconds)
* (Feature, Organization) It would be nice to allow some sort of "nested" tagging - tag subsections of a video to make the hierarchy a bit clearer. This is useful for videos which have different presentation sections, and during a particular section our tags don't really make much sense on the video as a whole, for instance a particular "property" might only make sense in a current "example". Obviously one better solution would be to split the videos into smaller chunks. In fact we could implement this as such - instead of have "nested" notes, we allow a new kind of marker - **section marker** that denotes starts and ends of a particular subsection. For now a single layer of grouping (i.e. we allow only one layer of sections) would suffice. ¶In fact, a workaround for now is to use an explicit tagged note "(Section Marker)" with empty contents to denote such.
