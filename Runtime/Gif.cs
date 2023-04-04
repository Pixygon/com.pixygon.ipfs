using System.Collections.Generic;
using UnityEngine;

public class Gif : Object {
    public List<GifData> Data;
    public Gif(List<GifData> g) {
        this.Data = g;
    }
}

public class GifData {
    public Sprite sprite;
    public float delay;
    public GifData(Sprite s, float d) {
        this.sprite = s;
        this.delay = d;
    }
}